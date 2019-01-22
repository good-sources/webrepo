using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AggregationService.Domain.Models;

namespace AggregationServiceClient
{
    class OutputFormatter
    {
        public string FormatCollections(IEnumerable<Collection> collections)
        {
            string output = (collections.Count() > 0) ? "" : "No collections\n";

            foreach (Collection collection in collections)
            {
                output = output + collection.Name + "\n";
            }

            return output;
        }

        public string FormatRssFeed(RssFeed feed)
        {
            if (feed == null)
                return string.Empty;

            return $"Title: {feed.Title ?? string.Empty}\nDescription: {feed.Description ?? string.Empty}\nLink: {feed.Link ?? string.Empty}\n\n";
        }

        public string FormatSourceTypesDictionary(IDictionary<string, int> sourceTypes)
        {
            string output = string.Empty;

            foreach (KeyValuePair<string, int> sourceType in sourceTypes)
            {
                output = output + sourceType.Value + " for " + sourceType.Key + ", ";
            }

            if (output.Length > 2)
                output = output.Substring(0, output.Length - 2);

            return output;
        }

        public string FormatContents(IEnumerable<Content> contents, string collectionName)
        {
            string output = (contents.Count() > 0) ? $"Contents for {collectionName} collection:\n\n" : "No contents for the specified collection\n";

            foreach (Content content in contents)
            {
                if (content.GetType() == typeof(RssItem))
                {
                    output = output + FormatRssItem((RssItem)content);
                }

                if (content.Published.HasValue)
                    output = output + $"\nPublished: {content.Published}";

                output = output + "\n\n";
            }

            return output;
        }

        public string FormatRssItem(RssItem item)
        {
            if (item == null)
                return string.Empty;
            
            return $"Title: {item.Title ?? string.Empty}\nDescription: {item.Description ?? string.Empty}\nAuthor: {item.Author ?? string.Empty}\nLink: {item.Link ?? string.Empty}";
        }
    }
}