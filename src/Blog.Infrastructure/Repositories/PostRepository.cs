using Blog.Application.Interfaces.Persistence;
using Blog.Domain.Entities;
using Blog.Infrastructure.Data;
using Blog.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Post?> GetByIdAsync(Guid id)
        {
            return await _context.Posts
                                 .IncludeTags()
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Post?> GetBySlugAsync(string slug)
        {
            return await _context.Posts
                                 .Where(p => p.IsPublished)
                                 .IncludeTags()
                                 .FirstOrDefaultAsync(p => p.Slug == slug);
        }

        public async Task<IEnumerable<Post>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Posts
                                 .IncludeTags()
                                 .OrderByDescending(p => p.CreatedAt)
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPublishedPostsAsync(int pageNumber, int pageSize, string? tagName = null)
        {
            var query = _context.Posts
                .Where(p => p.IsPublished)
                .IncludeTags()
                .OrderByDescending(p => p.CreatedAt)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tagName))
            {
                query = query.Where(p => p.PostTags.Any(pt => pt.Tag.Name == tagName));
            }

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetPublishedPostsCountAsync(string? tagName = null)
        {
            var query = _context.Posts
                .Where(p => p.IsPublished)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tagName))
            {
                query = query.Where(p => p.PostTags.Any(pt => pt.Tag.Name == tagName));
            }
            return await query.CountAsync();
        }

        public async Task<Post> AddAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task UpdateAsync(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Post post)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SlugExistsAsync(string slug, Guid? currentPostId = null)
        {
            if (currentPostId.HasValue)
            {
                return await _context.Posts.AnyAsync(p => p.Slug == slug && p.Id != currentPostId.Value);
            }
            return await _context.Posts.AnyAsync(p => p.Slug == slug);
        }
    }
}