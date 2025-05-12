using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Persistence
{
    public interface IPostRepository
    {
        Task<Post?> GetByIdAsync(Guid id);
        Task<Post?> GetBySlugAsync(string slug);
        Task<IEnumerable<Post>> GetAllAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Post>> GetPublishedPostsAsync(int pageNumber, int pageSize, string? tagName = null);
        Task<int> GetPublishedPostsCountAsync(string? tagName = null);
        Task<Post> AddAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(Post post);
        Task<bool> SlugExistsAsync(string slug, Guid? currentPostId = null);
    }
}