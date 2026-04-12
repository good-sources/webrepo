namespace AggregationService.Domain.Services
{
    using System;
    using System.Linq;
    using System.Data.Entity;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NLog;
    using AggregationService.Domain.Models;

    public class SourceService : Service<Source>, ISourceService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IReader _reader;

        public SourceService(DbContext context, IReader reader) : base(context)
        {
            _reader = reader;
        }

        public async Task<Guid> AddAsync(Source source)
        {
            Logger.Info("Adding source {SourceUri}", source.Uri);
            IEnumerable<Content> contents = await _reader.PullAsync(source);

            using (var transaction = BeginTransaction())
            {
                await CreateAsync(source);

                if (source.Id != Guid.Empty && contents.Any())
                {
                    contents.ToList().ForEach(content => content.SourceId = source.Id);
                    await CreateRangeAsync<Content>(contents);
                }

                transaction.Commit();
                Logger.Info("Source {SourceId} created with {ContentCount} initial content items", source.Id, contents.Count());
            }

            return source.Id;
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
