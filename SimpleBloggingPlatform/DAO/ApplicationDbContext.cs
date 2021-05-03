using Microsoft.EntityFrameworkCore;
using SimpleBloggingPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBloggingPlatform.DAO
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base(options)
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PostTag>().HasKey(postTag => new { postTag.PostSlug, postTag.TagId }); //many to many

            modelBuilder.Entity<Post>().HasMany(x => x.Tags).WithMany(x => x.Posts)
                .UsingEntity<PostTag>(
                        x => x.HasOne(x => x.Tag)
                        .WithMany().HasForeignKey(x => x.TagId),
                        x => x.HasOne(x => x.Post)
                       .WithMany().HasForeignKey(x => x.PostSlug));
        }
    }
}