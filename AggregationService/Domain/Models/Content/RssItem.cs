namespace AggregationService.Domain.Models
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations.Schema;

    public class RssItem : FeedContent
    {
        public string Description { get; set; }

        [JsonIgnore]
        [ForeignKey("SourceId")]
        public RssFeed Feed
        {
            get => (RssFeed)Source;
            set => Source = value;
        }
    }
}