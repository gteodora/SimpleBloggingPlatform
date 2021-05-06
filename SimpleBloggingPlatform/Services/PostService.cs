using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleBloggingPlatform.DAO;
using SimpleBloggingPlatform.Models;
using SimpleBloggingPlatform.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq;

namespace SimpleBloggingPlatform.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext context;
        
        private readonly ITagService tagService;
        private readonly IPostTagService postTagService;

        public PostService(ApplicationDbContext context, ITagService tagService, IPostTagService postTagService)
        {
            this.context = context;
            this.tagService = tagService;
            this.postTagService = postTagService;
        }

        public async Task<Post> GetPostBySlugAsync(string slug)
        {
            var post = await context.Posts.Include(p => p.Tags).Where(p => p.Slug.Equals(slug)).FirstOrDefaultAsync();
            if(post == null)
            {
                return null;
            }
            post.TagList = new List<string>();
            foreach (Tag tag in post.Tags)
            {
                post.TagList.Add(tag.Name);
            }
            return post;
        }
        
        public async Task<IEnumerable<Post>> GetPostsAsync()
        {
            var posts = await context.Posts.Include(p => p.Tags).ToListAsync();
            foreach (Post post in posts)
            {
                post.TagList = new List<string>();
                foreach (Tag tag in post.Tags)
                {
                    post.TagList.Add(tag.Name);
                }
            }
            return posts;
        }

        public async Task<Post> CreatePostAsync(Post post, bool isPost)
        {
            var newSlug = Utilities.Slugify(post.Title);
            var createdAt = Utilities.GetZuluOfNow();
            var newPost = new Post
            {
                Slug = newSlug,
                Title = post.Title,
                Body = post.Body,
                Description = post.Description,
                CreatedAt = createdAt
            };
            if (!isPost)
            {
                newPost = post;
            }
            if (post.TagList != null)
                {
                if (isPost) {
                    newPost.Tags = new List<Tag>();
                    newPost.TagList = new List<string>();
                        }
                    context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    foreach (string tagName in post.TagList.ToArray())
                    {
                        var tag = tagService.FindByName(tagName); //find tag by name
                    //context.Entry(tag).State = EntityState.Detached;
                    if (tag != null) //check is that tag already in db, if yes-then only create connection between tag and post 
                        {
                        if (isPost)
                        {
                            postTagService.CreatePostTag(ref newPost, tag, tagName);
                        }
                    }
                        else //if tag does not exist in db, create tag and create connection between post and tag
                        {
                            var id = tagService.CreateTagAsync(tagName);
                            var createdTag = tagService.FindByName(tagName);
                        if (isPost)
                        {
                            postTagService.CreatePostTag(ref newPost, tag, tagName);
                        }
                        context.Entry(tag).State = EntityState.Detached;
                    }
                    }
                }
            context.AttachRange(newPost.Tags);
            context.Posts.Add(newPost);
            await context.SaveChangesAsync();
            return newPost;
        }

        public async void RemoveAsync(Post post)
        {
            context.Posts.Remove(post);
            await context.SaveChangesAsync();
        }
        public async Task<Post> PutPost(Post oldPost, Post post) { 
            
            var newSlug = Utilities.Slugify(post.Title);
            var tags = oldPost.Tags;
            var tagList = oldPost.TagList;
            var createdAt = oldPost.CreatedAt;
            var updatedAt = Utilities.GetZuluOfNow();
            RemoveAsync(oldPost);
            var newPost = new Post 
            { 
                Slug = newSlug, 
                Title = post.Title, 
                Description = post.Description, 
                Body = post.Body, 
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                TagList = tagList,
                Tags = tags
            };
            if (oldPost.Title.Equals(post.Title) && oldPost.Description.Equals(post.Description) && oldPost.Body.Equals(post.Body))
            {
                oldPost.UpdatedAt = updatedAt;
                newPost = oldPost;
            }
            var isPost = false;
            var updatedPost = CreatePostAsync(newPost, isPost).Result;
            return updatedPost;
        }
    }
}
