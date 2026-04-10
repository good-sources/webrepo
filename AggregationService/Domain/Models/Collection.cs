namespace AggregationService.Domain.Models
{
    using System;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Collection
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<Source> Sources { get; set; }
    }
}