namespace AggregationServiceClient
{
    using System;
    using System.Linq;
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Collections.Generic;
    using AggregationService.Domain.Models;

    class ServiceConsumer
    {
        readonly HttpClient _client;
        readonly Uri baseUri;
        
        public ServiceConsumer(string uri)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.ConnectionClose = true;

            this.baseUri = new Uri(uri);
        }

        public IEnumerable<Collection> GetCollections()
        {
            IEnumerable<Collection> collections = default(IEnumerable<Collection>);

            try
            {
                using (var response = _client.GetAsync(baseUri.AddSegment("collections")))
                {
                    HttpResponseMessage message = response.Result;
                    message.EnsureSuccessStatusCode();

                    collections = message.Content.ReadAsAsync<IEnumerable<Collection>>().Result;
                }
            }
            catch
            {
                throw;
            }

            return collections;
        }

        public bool CreateCollection(string collectionName)
        {
            Collection collection = new Collection()
            {
                Name = collectionName
            };
            
            try
            {
                using (var response = _client.PostAsJsonAsync(baseUri.AddSegment("collections"), collection))
                {
                    HttpResponseMessage message = response.Result;
                    message.EnsureSuccessStatusCode();

                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        public bool CreateSource(string input)
        {
            Source source;
            Collection collection;
            IEnumerable<Collection> collections;
            string[] sourceFields = input.Split(',');
            string uri, collectionName;

            try
            {
                if (sourceFields.Length != 3)
                    throw new ArgumentException("One or more arguments missed");

                foreach (string field in sourceFields)
                {
                    if (string.IsNullOrEmpty(field))
                        throw new ArgumentException("One or more arguments missed");
                }

                if (!Int32.TryParse(sourceFields[0].Trim(), out int type))
                    throw new ArgumentException("Invalid source type");

                uri = sourceFields[1].Trim();
                collectionName = sourceFields[2].Trim();

                collections = GetCollections();
                collection = collections.Where(x => x.Name == collectionName).FirstOrDefault();

                if (collection == null)
                    throw new ArgumentException("Invalid collection name");

                switch ((SourceType)type)
                {
                    case SourceType.RSS:
                        source = new RssFeed()
                        {
                            Uri = uri,
                            CollectionId = collection.Id
                        };
                        break;
                    default:
                        throw new ArgumentException("Invalid source type");
                }

                using (var response = _client.PostAsJsonAsync(baseUri.AddSegment("sources"), source))
                {
                    HttpResponseMessage message = response.Result;
                    message.EnsureSuccessStatusCode();

                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        public IDictionary<string, int> GetSupportedSourceTypes()
        {
            IDictionary<string, int> sourceTypes = default(IDictionary<string, int>);

            try
            {
                using (var response = _client.GetAsync(baseUri.AddSegment("supportedsourcetypes")))
                {
                    HttpResponseMessage message = response.Result;
                    message.EnsureSuccessStatusCode();

                    sourceTypes = message.Content.ReadAsAsync<IDictionary<string, int>>().Result;
                }
            }
            catch
            {
                throw;
            }

            return sourceTypes;
        }

        public IEnumerable<Content> GetContentsByCollection(string collectionName)
        {
            IEnumerable<Content> contents = default(IEnumerable<Content>);
            IEnumerable<Collection> collections;
            Collection collection;

            try
            {
                collections = GetCollections();
                collection = collections.Where(x => x.Name == collectionName).FirstOrDefault();

                if (collection == null)
                    throw new ArgumentException("Invalid collection name");

                using (var response = _client.GetAsync(baseUri.AddSegment($"contents/bycollection/{collection.Id}")))
                {
                    HttpResponseMessage message = response.Result;
                    message.EnsureSuccessStatusCode();

                    var formatter = new JsonMediaTypeFormatter
                    {
                        SerializerSettings = { TypeNameHandling = TypeNameHandling.Auto }
                    };

                    contents = message.Content.ReadAsAsync<IEnumerable<Content>>(new[] { formatter }).Result;
                }
            }
            catch
            {
                throw;
            }

            return contents;
        }
    }

    static class UriExtensions
    {
        public static Uri AddSegment(this Uri OriginalUri, string Segment)
        {
            UriBuilder ub = new UriBuilder(OriginalUri);
            ub.Path = ub.Path + ((ub.Path.EndsWith("/")) ? "" : "/") + Segment;

            return ub.Uri;
        }
    }
}
