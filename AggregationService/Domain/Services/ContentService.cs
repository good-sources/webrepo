namespace AggregationService.Domain.Services
{
    using System;
    using System.Linq;
    using System.Data.Entity;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AggregationService.Domain.Models;

    public class ContentService : Service<Content>, IContentService
    {
        public ContentService(DbContext context) : base(context) { }

        public async Task<IEnumerable<Content>> GetByCollectionAsync(Guid collectionId)
        {
            IEnumerable<Source> sources = await Read<Source>()
                .Include(s => s.Contents)
                .Where(x => x.CollectionId == collectionId).ToListAsync();

            foreach (Source source in sources)
            {
                IEnumerable<Content> validContents = await Reader.ValidateAsync(source);

                using (var transaction = BeginTransaction())
                {
                    if (validContents.Any())
                    {
                        await DeleteRangeAsync<Content>(source.Contents);
                        validContents.ToList().ForEach(content => content.SourceId = source.Id);
                        await CreateRangeAsync<Content>(validContents);
                    }
                    else
                    {
                        await CommitChangesAsync();
                    }

                    transaction.Commit();
                }
            }

            return await Read()
                .Where(x => x.Source.CollectionId == collectionId)
                .OrderByDescending(x => x.Published).ToListAsync();
        }
    }
}
