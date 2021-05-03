using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleBloggingPlatform.DAO;
using SimpleBloggingPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBloggingPlatform.Controllers
{
    public class TagsController : MyControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<TagsController> logger;

        public TagsController(ILogger<TagsController> logger, ApplicationDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<TagsRequestData>> GetAllAsync()
        {
            var tagList = await context.Tags.ToListAsync();
            List<string> tagNames = new List<string>();
            foreach(Tag tag in tagList)
            {
                tagNames.Add(tag.Name);
            }
            return new TagsRequestData { Tags = tagNames };
        }
    }
}
