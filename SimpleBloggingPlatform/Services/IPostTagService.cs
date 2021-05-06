using SimpleBloggingPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBloggingPlatform.Services
{
    public interface IPostTagService
    {
        public void CreatePostTag(ref Post newPost, Tag tag, string tagName);
    }
}
