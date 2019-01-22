namespace AggregationService.Domain.Services
{    
    using System.Linq;
    using System.Collections.Generic;
    using AggregationService.Domain.Models;

    public class ContentService : Service<Content>, IContentService
    {
        public ContentService() :
            base(new AggregationServiceContext())
        { }

        public IEnumerable<Content> GetByCollection(int collectionId)
        {
            try
            {
                IEnumerable<Source> sources = Read<Source>()
                .Include("Contents")
                .Where(x => x.CollectionId == collectionId).ToList<Source>();

                foreach (Source source in sources)
                {
                    Reader.Validate(source, out IEnumerable<Content> validContents);

                    if (validContents.Count() > 0)
                    {
                        DeleteRange<Content>(source.Contents);
                        validContents.ToList().ForEach(content => content.SourceId = source.Id);
                        CreateRange<Content>(validContents);
                    }
                    else
                        CommitChanges();
                }

                return Read()
                    .Where(x => x.Source.CollectionId == collectionId)
                    .OrderByDescending(x => x.Published).ToList<Content>();
            }
            catch
            {
                throw;
            }
        }
    }
}