namespace AggregationService.Domain.Services
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using AggregationService.Domain.Models;

    public class SourceService : Service<Source>, ISourceService
    {
        public SourceService() :
            base(new AggregationServiceContext())
        { }

        public Guid Add(Source source)
        {
            try
            {
                Reader.Pull(source, out IEnumerable<Content> contents);

                Create(source);

                if (source.Id != Guid.Empty && contents.Count() > 0)
                {
                    contents.ToList().ForEach(content => content.SourceId = source.Id);
                    CreateRange<Content>(contents);
                }

                return source.Id;
            }
            catch
            {
                throw;
            }
        }

        public IDictionary<string, int> GetSupportedTypes()
        {
            IDictionary<string, int> types = new Dictionary<string, int>();

            foreach (SourceType type in Enum.GetValues(typeof(SourceType)))
            {
                types.Add(type.ToString(), Convert.ToInt32(type));
            }

            return types;
        }
    }
}