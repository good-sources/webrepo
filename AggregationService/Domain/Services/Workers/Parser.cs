namespace AggregationService.Domain.Services
{
    using System.Xml;
    using System.Collections.Generic;
    using AggregationService.Domain.Models;

    internal class Parser
    {
        public IEnumerable<Content> Parse(Source source, string document)
        {
            switch (source.Type)
            {
                case SourceType.RSS:
                    return ParseRss((RssFeed)source, document);
                default:
                    return new List<Content>();
            }
        }

        private IEnumerable<Content> ParseRss(RssFeed feed, string document)
        {
            List<Content> contents = new List<Content>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(document);

            XmlNode root = doc.DocumentElement,
                channel = root.SelectSingleNode("channel");
            XmlNodeList items = channel.SelectNodes("item");

            feed.Title = channel.SelectSingleNode("title")?.InnerText;
            feed.Link = channel.SelectSingleNode("link")?.InnerText;
            feed.Description = channel.SelectSingleNode("description")?.InnerText;
            
            foreach (XmlNode item in items)
            {
                contents.Add(new RssItem()
                {
                    Published = DateTimeConverter.ConvertGenericDateTimeOffsetStringToUTC(item.SelectSingleNode("pubDate")?.InnerText),
                    Title = item.SelectSingleNode("title")?.InnerText,
                    Link = item.SelectSingleNode("link")?.InnerText,
                    Author = item.SelectSingleNode("author")?.InnerText,
                    Description = item.SelectSingleNode("description")?.InnerText
                });
            }

            return contents;
        }
    }
}