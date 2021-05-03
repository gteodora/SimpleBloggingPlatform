using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimpleBloggingPlatform.Models
{
    [Table("post_tag")]
    public class PostTag
    {
        [Key]
        [Column("post_slug")]
        public string PostSlug { get; set; }
        public Post Post { get; set; }
        [Key]
        [Column("tag_id")]
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
