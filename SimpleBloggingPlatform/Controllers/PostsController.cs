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

namespace SimpleBloggingPlatform.Controllers
{
    public class PostsController : MyControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<PostsController> logger;
        public PostsController(ApplicationDbContext context, ILogger<PostsController> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        //DELETE:
        [HttpDelete("{slug}")]
        public async Task<IActionResult> DeletePostAsync(string slug)
        {
            try
            {
                var post = GetPostBySlugAsync(slug); // await context.Posts.FindAsync();  //GetPostBySlugAsync(slug);
                if (post == null)
                {
                    return NotFound();
                }
                context.Posts.Remove(post.Result);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
            return NoContent();
        }

        //GET: api/posts
        [HttpGet]
        public ActionResult<PostsRequestData> GetAlll()
        {
            var posts = GetPostsAsync();
               //Will return multiple blog posts, ordered by most recent first.
            var postsRequest = new PostsRequestData(posts.Result.OrderByDescending(post => 
            { if (post.UpdatedAt != null) return post.UpdatedAt;
                else return post.CreatedAt; 
            } ));
            return Ok(postsRequest);
        }

        private async Task<IEnumerable<Post>> GetPostsAsync()
        {
            var posts = await context.Posts.Include(p => p.Tags).ToListAsync();
            foreach(Post post in posts)
            {
                post.TagList = new List<string>();
               foreach(Tag tag in post.Tags)
                {
                    post.TagList.Add(tag.Name);
                }
            }
            return posts;
        }

        //GET: api/posts/:slug
        [HttpGet("{slug}")]
        public ActionResult<PostRequestData> GetPostBySlug(string slug)
        {
            try 
            { 
                var post = GetPostBySlugAsync(slug);
                if (post == null)
                {
                    logger.LogWarning("GET blog post with slug ({slug}) NOT FOUND", slug);
                    return NotFound();
                }
                var postRequestData = new PostRequestData { BlogPost = post.Result };
                return Ok(postRequestData);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }
        private async Task<Post> GetPostBySlugAsync(string slug)
        {
                return await context.Posts.FindAsync(slug);
        }

        [HttpPost]
        public IActionResult PostBlogPost(PostRequestData post)
        {
            if (post == null) //ima li ovo smisla jer nema ?,ne moze biti null ...
            {
                logger.LogWarning("POST blog posts BAD REQUEST");
                return BadRequest();
            }
            //Requirement of fields is obtained by [Requiered] Anotation
            if (post.BlogPost.Title == null || post.BlogPost.Title.Length == 0) //treba li ovo kad radi na nivou anotacije?
            {
                logger.LogWarning("POST blog posts with empty title ({post.title}) BAD REQUEST", post);
                // return StatusCode(500);
                return BadRequest();
            }

            if (context.Posts == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
            try
            {
                if (context.Posts.FirstOrDefault(p => p.Title.Equals(post.BlogPost.Title)) != null) { 
                    logger.LogWarning("POST blog posts with  title ({tagTitle}) BAD REQUEST", post);
                    return BadRequest("The title field is taken.");
                    // return BadRequest("The title field is taken.");  // trebam li reci u poruci da je zauzet title?
                }
                var newPost = CreateNewPostAsync(post.BlogPost).Result;
                return CreatedAtAction(nameof(GetPostBySlug), new { slug = newPost.Slug }, newPost); /// ovo ispod nije okej, jer traze da im vratim ovaj novi, dodani objekat: return Ok(post); return StatusCode(201);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }


        private async Task<Post> CreateNewPostAsync(Post post)
        {
            
            var newSlug = Utilities.Slugify(post.Title);
            if (post.TagList != null)
            {
                SaveTagsOfPost(post);
            }
            var createdAt = Utilities.GetZuluOfNow();
            var newPost = new Post { Slug = newSlug, Title = post.Title, Body = post.Body, 
                Description = post.Description, CreatedAt = createdAt };
            context.Posts.Add(newPost);
            await context.SaveChangesAsync();
            return newPost;
        }

        //TODO
        private void SaveTagsOfPost(Post blogPost)
        {
            //provjeriti da li ima tih tagova u bazi, pa ako nema kreirati nove entry-je u bazi u obje tabele.

            //Ako ima, dodati u samo post-Tag 

        }

        //TODO
        // GET: api/posts/filter?tag=smt
        [HttpGet("Filter")]
        public async Task<ActionResult> Filter(string tag)
        {
            var foundTag = context.Tags.Where(t => t.Name.Equals(tag)).Select(t => new Tag { Id = t.Id, Name = t.Name }).FirstOrDefault(); //tag name je unique
            if(foundTag == null)
            {
                logger.LogWarning("GET blog posts with tag name ({tag}) NOT FOUND", tag);
                return NotFound("Tag with name {tag} does not exists.");
            }
            var postTags = await context.PostTags.Where(pt => pt.TagId == foundTag.Id).
                Select(pt => new PostTag { TagId = pt.TagId, PostSlug = pt.PostSlug }).ToListAsync();

            //...


            return NoContent();
        }

        //TODO
        //PUT:
        
    }
}


