using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using System.Collections;

namespace SimpleBloggingPlatform.Models
{
    [Table("post")]
    public class Post
    {
       [Key]
        public string Slug { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Body { get; set; }
        [NotMapped]
        public List<string> TagList { get; set; }
        [Column("created_at")]
        public string  CreatedAt { get; set; } //UTC ce biti u bazi, pa cu to ovako prebaciti za JSON .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK");
        [Column("updated_at")]
        public string UpdatedAt { get;  set; }
        [JsonIgnore]
        public  ICollection<Tag> Tags { get; set; }
     //   [JsonIgnore]
     //   public ICollection<PostTag> PostTags { get; set; }


    }
}
