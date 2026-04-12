namespace AggregationService.Tests.Formatting
{
    using System;
    using NUnit.Framework;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using AggregationService.Domain.Models;
    using AggregationService.Formatting;

    [TestFixture]
    public class PolymorphicSourceConverterTests
    {
        private PolymorphicSourceConverter _converter;

        [SetUp]
        public void SetUp()
        {
            _converter = new PolymorphicSourceConverter();
        }

        [Test]
        public void ReadJson_ReturnsRssFeed_WhenTypeIsRss()
        {
            var collectionId = Guid.NewGuid();
            var jsonStr = $@"{{""Uri"":""https://example.com/feed"",""Type"":0,""CollectionId"":""{collectionId}""}}";

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(_converter);
            var result = JsonConvert.DeserializeObject<Source>(jsonStr, settings);

            Assert.That(result, Is.TypeOf<RssFeed>());
            Assert.That(result.Uri, Is.EqualTo("https://example.com/feed"));
            Assert.That(result.CollectionId, Is.EqualTo(collectionId));
        }

        [Test]
        public void ReadJson_ThrowsArgumentException_WhenUriIsMissing()
        {
            var json = new JObject
            {
                ["Type"] = 0,
                ["CollectionId"] = Guid.NewGuid()
            };

            Assert.That(() => DeserializeViaConverter(json),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ReadJson_ThrowsArgumentException_WhenTypeIsMissing()
        {
            var json = new JObject
            {
                ["Uri"] = "https://example.com",
                ["CollectionId"] = Guid.NewGuid()
            };

            Assert.That(() => DeserializeViaConverter(json),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ReadJson_ThrowsArgumentException_WhenCollectionIdIsMissing()
        {
            var json = new JObject
            {
                ["Uri"] = "https://example.com",
                ["Type"] = 0
            };

            Assert.That(() => DeserializeViaConverter(json),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ReadJson_ThrowsNotSupportedException_WhenTypeIsUnknown()
        {
            var json = new JObject
            {
                ["Uri"] = "https://example.com",
                ["Type"] = 999,
                ["CollectionId"] = Guid.NewGuid()
            };

            Assert.That(() => DeserializeViaConverter(json),
                Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void WriteJson_ThrowsNotImplementedException()
        {
            Assert.That(() => _converter.WriteJson(null, null, null),
                Throws.TypeOf<NotImplementedException>());
        }

        [Test]
        public void CanConvert_ReturnsTrue_ForSourceType()
        {
            Assert.That(_converter.CanConvert(typeof(Source)), Is.True);
        }

        [Test]
        public void CanConvert_ReturnsFalse_ForOtherTypes()
        {
            Assert.That(_converter.CanConvert(typeof(string)), Is.False);
            Assert.That(_converter.CanConvert(typeof(Collection)), Is.False);
        }

        private Source DeserializeViaConverter(JObject json)
        {
            using (var reader = json.CreateReader())
            {
                return (Source)_converter.ReadJson(reader, typeof(Source), null, JsonSerializer.CreateDefault());
            }
        }
    }
}
