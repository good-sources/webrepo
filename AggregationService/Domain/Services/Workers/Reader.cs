namespace AggregationService.Domain.Services
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Net.Http.Headers;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NLog;
    using AggregationService.Domain.Models;

    internal static class Reader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly HttpClient _client;
        private static readonly Parser _parser;

        static Reader()
        {
            _client = new HttpClient(
                new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                });

            _client.Timeout = TimeSpan.FromSeconds(30);
            _client.DefaultRequestHeaders.ConnectionClose = true;
            _client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            _client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

            _parser = new Parser();
        }

        private static void ValidateUri(string uri)
        {
            if (!System.Uri.TryCreate(uri, UriKind.Absolute, out Uri parsedUri)
                || (parsedUri.Scheme != System.Uri.UriSchemeHttp && parsedUri.Scheme != System.Uri.UriSchemeHttps))
            {
                Logger.Warn("Invalid URI rejected: {Uri}", uri);
                throw new ArgumentException("Only HTTP and HTTPS URIs are allowed.");
            }
        }

        public static async Task<IEnumerable<Content>> PullAsync(Source source)
        {
            Logger.Info("Pulling content from source {SourceUri}", source.Uri);
            ValidateUri(source.Uri);
            using (var request = new HttpRequestMessage(HttpMethod.Get, source.Uri))
            using (HttpResponseMessage message = await _client.SendAsync(request))
            {
                ValidateSource(source, message);
                return _parser.Parse(source, await message.Content.ReadAsStringAsync());
            }
        }

        public static async Task<IEnumerable<Content>> ValidateAsync(Source source)
        {
            if (!source.Expires.HasValue || source.Expires <= DateTime.Now)
            {
                Logger.Debug("Validating source {SourceUri}, Expires={Expires}", source.Uri, source.Expires);
                ValidateUri(source.Uri);
                using (var request = new HttpRequestMessage(HttpMethod.Get, source.Uri))
                {
                    request.Headers.IfModifiedSince = source.LastlyModified;

                    using (HttpResponseMessage message = await _client.SendAsync(request))
                    {
                        if (message.StatusCode == HttpStatusCode.NotModified || message.IsSuccessStatusCode)
                        {
                            ValidateSource(source, message);

                            if (message.IsSuccessStatusCode)
                            {
                                return _parser.Parse(source, await message.Content.ReadAsStringAsync());
                            }
                        }
                        else
                        {
                            Logger.Warn("Source {SourceUri} returned non-success status {StatusCode}", source.Uri, message.StatusCode);
                            throw new HttpResponseException(message.StatusCode);
                        }
                    }
                }
            }

            return new List<Content>();
        }

        private static void ValidateSource(Source source, HttpResponseMessage message)
        {
            HttpContentHeaders headers = message.Content.Headers;
            DateTimeOffset? LastModified = headers.LastModified, Expires = headers.Expires;
            TimeSpan? MaxAge = message.Headers.CacheControl?.MaxAge;

            if (LastModified.HasValue)
            {
                source.LastlyModified = LastModified.Value;
            }

            if (MaxAge.HasValue)
            {
                source.Expires = DateTime.Now.AddSeconds(MaxAge.Value.TotalSeconds);
            }
            else
            {
                if (Expires.HasValue)
                {
                    source.Expires = Expires.Value.DateTime.ToLocalTime();
                }
            }
        }
    }
}