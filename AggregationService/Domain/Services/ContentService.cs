namespace AggregationService.Domain.Services
{    
    using System.Linq;
    using System.Data.Entity;
    using System.Collections.Generic;
    using AggregationService.Domain.Models;

    public class ContentService : Service<Content>, IContentService
    {
        public ContentService() :
            base(new AggregationServiceContext())
        { }

        public IEnumerable<Content> GetByCollection(int collectionId)
        {
            IEnumerable<Source> sources = Read<Source>()
                .Include(s => s.Contents)
                .Where(x => x.CollectionId == collectionId).ToList<Source>();

            foreach (Source source in sources)
            {
                Reader.Validate(source, out IEnumerable<Content> validContents);

                using (var transaction = BeginTransaction())
                {
                    if (validContents.Any())
                    {
                        DeleteRange<Content>(source.Contents);
                        validContents.ToList().ForEach(content => content.SourceId = source.Id);
                        CreateRange<Content>(validContents);
                    }
                    else
                        CommitChanges();

                    transaction.Commit();
                }
            }

            return Read()
                .Where(x => x.Source.CollectionId == collectionId)
                .OrderByDescending(x => x.Published).ToList<Content>();
        }
    }
}