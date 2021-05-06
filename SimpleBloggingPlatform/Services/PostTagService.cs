using SimpleBloggingPlatform.DAO;
using SimpleBloggingPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBloggingPlatform.Services
{
    public class PostTagService : IPostTagService
    {
        private readonly ApplicationDbContext context;
        public PostTagService(ApplicationDbContext context)
        {
            this.context = context;
           
        }
        public void CreatePostTag(ref Post newPost, Tag tag, string tagName)
        {
            newPost.TagList.Add(tagName);
            newPost.Tags.Add(tag);
            //return new
        }
    }
}
