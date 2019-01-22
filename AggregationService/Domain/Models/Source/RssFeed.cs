namespace AggregationService.Domain.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class RssFeed : Feed
    {       
        [StringLength(1000)]
        public string Description { get; set; }

        public override SourceType Type { get => SourceType.RSS; }
    }
}