using Microsoft.EntityFrameworkCore;
using Blog.Domain.Entities;

namespace Blog.Infrastructure.Extensions
{
    public static class PostQueryExtensions
    {
        public static IQueryable<Post> IncludeTags(this IQueryable<Post> query)
        {
            return query
                   .Include(p => p.PostTags)
                   .ThenInclude(pt => pt.Tag);
        }
    }
}