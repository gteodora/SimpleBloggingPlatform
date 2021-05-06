using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleBloggingPlatform.DAO;
using SimpleBloggingPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SimpleBloggingPlatform.Utils;
using SimpleBloggingPlatform.Services;

namespace SimpleBloggingPlatform.Controllers
{
    public class PostsController : MyControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<PostsController> logger;
        private readonly IPostService postService;
        private readonly ITagService tagService;

        public PostsController(ApplicationDbContext context, ILogger<PostsController> logger, IPostService postService, ITagService tagService)
        {
            this.context = context;
            this.logger = logger;
            this.postService = postService;
            this.tagService = tagService;
        }

        //GET: api/posts
        [HttpGet]
        public ActionResult<PostsRequestData> GetAlll()
        {
            try
            {
                var posts = postService.GetPostsAsync();
                var postsRequest = new PostsRequestData(posts.Result.OrderByDescending(post => {
                    if (post.UpdatedAt != null)
                    {
                        return post.UpdatedAt;
                    }
                    else 
                        return post.CreatedAt; 
                    } ));
            
                return Ok(postsRequest);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        //GET: api/posts/:slug
        [HttpGet("{slug}")]
        public ActionResult<PostRequestData> GetPostBySlug(string slug)
        {
            try
            {
                var post = postService.GetPostBySlugAsync(slug).Result;
                if (post == null)
                {
                    logger.LogWarning("GET blog post with slug ({slug}) NOT FOUND", slug);
                    return NotFound("Tag with slug {slug} does not exists.");
                }
                var postRequestData = new PostRequestData { BlogPost = post };
                return Ok(postRequestData);  
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
             }
    
        }

        [HttpPost]
        public IActionResult CreatePost(PostRequestData post)
        {
            if (post == null)
            {
                logger.LogWarning("POST blog posts BAD REQUEST");
                return BadRequest();
            }
            //Requirement of props Title, Description and Body  is obtained by [Requiered] Anotation
            if (context.Posts == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
            try
            {
                var newSlug = Utilities.Slugify(post.BlogPost.Title);
                if (context.Posts.FirstOrDefault(p => p.Title.Equals(post.BlogPost.Title) || p.Slug.Equals(newSlug)) != null)
                {
                    logger.LogWarning("POST blog posts with slug ({newSlug}) BAD REQUEST", post);
                    return BadRequest("The Slug field is taken.");
                }
                var isPost = true;
                var newPost = postService.CreatePostAsync(post.BlogPost, isPost).Result;
                return CreatedAtAction(nameof(GetPostBySlug), new { slug = newPost.Slug }, newPost); 
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        // GET: api/posts/filter?tag=smt
        [HttpGet("Filter")]
        public async Task<ActionResult<IEnumerable<Post>>> Filter(string tag)
        {
            try
            {
                var foundTag = tagService.FindByName(tag);
                if (foundTag == null)
                {
                    logger.LogWarning("GET blog posts with tag name ({tag}) NOT FOUND", foundTag);
                    return NotFound("Tag with name {tag} does not exists.");
                }
                var postTags = await context.PostTags.Where(pt => pt.TagId == foundTag.Id).
                    Select(pt => new PostTag { TagId = pt.TagId, Tag = foundTag, PostSlug = pt.PostSlug}).ToListAsync();
                
                var result = postTags.Select(pt => pt.Post = postService.GetPostBySlugAsync(pt.PostSlug).Result ).ToList();
                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        //PUT: /api/Posts/{slug}
        [HttpPut("{slug}")]
        public async Task<ActionResult<Post>> Put(string slug, PostRequestData? postRequest)
        {
            if (postRequest == null || postRequest.BlogPost==null)
            {
                return BadRequest("Empty body request.");
            }
            var post = postRequest.BlogPost;
            if (slug != post.Slug)
            {
                return BadRequest();
            }
            
            try
            {
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                var oldPost = postService.GetPostBySlugAsync(slug).Result;
                context.Entry(post).State = EntityState.Detached;
                if (oldPost == null)
                {
                    return NotFound();
                }
                var newSlug = Utilities.Slugify(post.Title);
                if (!slug.Equals(newSlug)) //ako je unesen novi title, treba provjeriti je li yauyet
                {
                    var existingPost = postService.GetPostBySlugAsync(newSlug);
                    if (existingPost.Result != null)
                    {
                        return BadRequest("Already exists post's slug generated from the title.");
                    }
                }
                
                    var newPost = postService.PutPost(oldPost, post).Result;
                    return CreatedAtAction(nameof(GetPostBySlug), new { slug = newPost.Slug }, newPost);
                
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        //DELETE:
        [HttpDelete("{slug}")]
        public IActionResult DeletePost(string slug)
        {
            try
            {
                var post = postService.GetPostBySlugAsync(slug).Result; 
                if (post == null)
                {
                    return NotFound();
                }
                postService.RemoveAsync(post);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
            return NoContent();
        }
    }
}