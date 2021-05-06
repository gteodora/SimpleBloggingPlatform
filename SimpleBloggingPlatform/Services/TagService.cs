using Microsoft.EntityFrameworkCore;
using SimpleBloggingPlatform.DAO;
using SimpleBloggingPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBloggingPlatform.Services
{
    public class TagService : ITagService
    {
        private readonly ApplicationDbContext context;
        private readonly IPostTagService postTagService;

        public TagService(ApplicationDbContext context, IPostTagService postTagService)
        {
            this.context = context;
            this.postTagService = postTagService;
        }
    
        public async Task<int> CreateTagAsync(string name)
        {
            var newTag = new Tag { Name = name };
            await context.Tags.AddAsync(newTag);
            int lastId = await context.SaveChangesAsync();
            context.Entry(newTag).State = EntityState.Detached;
            return lastId;
        }

        public Tag FindByName(string name)
        {
            return context.Tags.
                Where(t => t.Name.Equals(name)). /*tag name is unique in database*/
                Select(t => new Tag { Id = t.Id, Name = t.Name }).
                FirstOrDefault();
        }
    }
}
