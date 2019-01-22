namespace AggregationService.Domain.Models
{
    using System;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;   

    public abstract class Content
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public DateTime? Published { get; set; }

        public Guid SourceId { get; set; }

        [JsonIgnore]
        public Source Source { get; set; }
    }
}