namespace AggregationService.Tests.Domain.Services.Workers
{
    using AggregationService.Domain.Models;
    using AggregationService.Domain.Services;
    using NUnit.Framework;
    using System.Linq;
    using System.Xml;

    [TestFixture]
    public class ParserTests
    {
        private Parser _parser;

        private const string ValidRssDocument = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <rss version=""2.0"">
              <channel>
                <title>Test Feed</title>
                <link>https://example.com</link>
                <description>A test feed</description>
                <item>
                  <title>Article 1</title>
                  <link>https://example.com/1</link>
                  <author>author@example.com</author>
                  <description>First article</description>
                  <pubDate>Mon, 07 Jan 2019 12:00:00 +00:00</pubDate>
                </item>
                <item>
                  <title>Article 2</title>
                  <link>https://example.com/2</link>
                  <pubDate>Tue, 08 Jan 2019 08:00:00 +00:00</pubDate>
                </item>
              </channel>
            </rss>";

                    private const string EmptyChannelRssDocument = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <rss version=""2.0"">
              <channel>
                <title>Empty Feed</title>
                <link>https://example.com</link>
                <description>No items</description>
              </channel>
            </rss>";

        [SetUp]
        public void SetUp()
        {
            _parser = new Parser();
        }

        [Test]
        public void Parse_ReturnsCorrectRssItems_FromValidDocument()
        {
            var feed = new RssFeed { Uri = "https://example.com/feed" };

            var result = _parser.Parse(feed, ValidRssDocument).ToList();

            Assert.That(result, Has.Count.EqualTo(2));

            var first = (RssItem)result[0];
            Assert.That(first.Title, Is.EqualTo("Article 1"));
            Assert.That(first.Link, Is.EqualTo("https://example.com/1"));
            Assert.That(first.Author, Is.EqualTo("author@example.com"));
            Assert.That(first.Description, Is.EqualTo("First article"));
            Assert.That(first.Published, Is.Not.Null);

            var second = (RssItem)result[1];
            Assert.That(second.Title, Is.EqualTo("Article 2"));
            Assert.That(second.Link, Is.EqualTo("https://example.com/2"));
        }

        [Test]
        public void Parse_PopulatesFeedMetadata_FromChannelElement()
        {
            var feed = new RssFeed { Uri = "https://example.com/feed" };

            _parser.Parse(feed, ValidRssDocument);

            Assert.That(feed.Title, Is.EqualTo("Test Feed"));
            Assert.That(feed.Link, Is.EqualTo("https://example.com"));
            Assert.That(feed.Description, Is.EqualTo("A test feed"));
        }

        [Test]
        public void Parse_ReturnsEmptyList_WhenNoItems()
        {
            var feed = new RssFeed { Uri = "https://example.com/feed" };

            var result = _parser.Parse(feed, EmptyChannelRssDocument);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Parse_HandlesNullOptionalFields()
        {
            var feed = new RssFeed { Uri = "https://example.com/feed" };

            var result = _parser.Parse(feed, ValidRssDocument).ToList();

            var second = (RssItem)result[1];
            Assert.That(second.Author, Is.Null);
        }

        [Test]
        public void Parse_ThrowsXmlException_WhenDocumentIsMalformed()
        {
            var feed = new RssFeed { Uri = "https://example.com/feed" };

            Assert.That(() => _parser.Parse(feed, "<not valid xml"),
                Throws.TypeOf<XmlException>());
        }
    }
}
