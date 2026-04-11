namespace AggregationService.Domain.Services
{
    using System;
    using System.Linq;
    using System.Data.Entity;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NLog;
    using AggregationService.Domain.Models;

    public class ContentService : Service<Content>, IContentService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ContentService(DbContext context) : base(context) { }

        public async Task<IEnumerable<Content>> GetByCollectionAsync(Guid collectionId)
        {
            Logger.Info("Fetching contents for collection {CollectionId}", collectionId);
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
                    Logger.Debug("Updated contents for source {SourceId}", source.Id);
                }
            }

            return await Read()
                .Where(x => x.Source.CollectionId == collectionId)
                .OrderByDescending(x => x.Published).ToListAsync();
        }
    }
}
