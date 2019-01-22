namespace AggregationService.Domain.Models
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

    public abstract class Feed : Source
    {       
        [StringLength(300)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Link { get; set; }
    }
}