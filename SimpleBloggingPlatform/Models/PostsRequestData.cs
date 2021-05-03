using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBloggingPlatform.Models
{
    public class PostsRequestData
    {
        public IEnumerable<Post> blogPosts { get; set; } 
        public int postsCount { get; set; }

        public PostsRequestData(IEnumerable<Post> posts)
        {
            this.blogPosts = posts;
            this.postsCount = posts.Count();
        }
    }
}
