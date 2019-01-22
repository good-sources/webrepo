namespace AggregationService.Domain.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public abstract class FeedContent : Content
    {
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Link { get; set; }

        [StringLength(70)]
        public string Author { get; set; }
    }
}