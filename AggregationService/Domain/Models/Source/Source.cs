namespace AggregationService.Domain.Models
{
    using System;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public abstract class Source
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Uri { get; set; }

        public abstract SourceType Type { get; }

        [JsonIgnore]
        public DateTimeOffset? LastlyModified { get; set; }
        [JsonIgnore]
        public DateTime? Expires { get; set; }

        [ForeignKey("Collection")]
        public int CollectionId { get; set; }

        [JsonIgnore]
        public Collection Collection { get; set; }

        [JsonIgnore]
        public virtual ICollection<Content> Contents { get; set; }
    }

    public enum SourceType
    {
        RSS
    }
}