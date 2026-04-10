namespace AggregationService.Domain.Services
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Net.Http.Headers;
    using System.Collections.Generic;
    using AggregationService.Domain.Models;

    internal static class Reader
    {
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
                throw new ArgumentException("Only HTTP and HTTPS URIs are allowed.");
            }
        }

        public static void Pull(Source source, out IEnumerable<Content> contents)
        {
            ValidateUri(source.Uri);
            using (var request = new HttpRequestMessage(HttpMethod.Get, source.Uri))
            using (HttpResponseMessage message = _client.SendAsync(request).Result)
            {
                ValidateSource(source, message);
                contents = _parser.Parse(source, message.Content.ReadAsStringAsync().Result);
            }
        }

        public static void Validate(Source source, out IEnumerable<Content> validContents)
        {
            validContents = new List<Content>();

            if (!source.Expires.HasValue || source.Expires <= DateTime.Now)
            {
                ValidateUri(source.Uri);
                using (var request = new HttpRequestMessage(HttpMethod.Get, source.Uri))
                {
                    request.Headers.IfModifiedSince = source.LastlyModified;

                    using (HttpResponseMessage message = _client.SendAsync(request).Result)
                    {
                        if (message.StatusCode == HttpStatusCode.NotModified || message.IsSuccessStatusCode)
                        {
                            ValidateSource(source, message);

                            if (message.IsSuccessStatusCode)
                                validContents = _parser.Parse(source, message.Content.ReadAsStringAsync().Result);
                        }
                        else
                            throw new HttpResponseException(message.StatusCode);
                    }
                }
            }
        }

        private static void ValidateSource(Source source, HttpResponseMessage message)
        {
            HttpContentHeaders headers = message.Content.Headers;
            DateTimeOffset? LastModified = headers.LastModified, Expires = headers.Expires;
            TimeSpan? MaxAge = message.Headers.CacheControl?.MaxAge;

            if (LastModified.HasValue)
                source.LastlyModified = LastModified.Value;

            if (MaxAge.HasValue)
                source.Expires = DateTime.Now.AddSeconds(MaxAge.Value.TotalSeconds);
            else
            {
                if (Expires.HasValue)
                    source.Expires = Expires.Value.DateTime.ToLocalTime();
            }
        }
    }
}