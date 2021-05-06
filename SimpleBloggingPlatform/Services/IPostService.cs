using SimpleBloggingPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBloggingPlatform.Services
{
    public interface IPostService
    {
        public Task<Post> PutPost(Post oldPost, Post post);
        public Task<IEnumerable<Post>> GetPostsAsync();
        public void RemoveAsync(Post post);
        public Task<Post> CreatePostAsync(Post post, bool isPost);
        public Task<Post> GetPostBySlugAsync(string slug);
    }
}