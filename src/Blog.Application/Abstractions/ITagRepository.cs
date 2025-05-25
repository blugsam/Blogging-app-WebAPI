using Blog.Domain.Entities;

namespace Blog.Application.Abstractions
{
    public interface ITagRepository
    {
        Task<Tag?> GetByIdAsync(Guid id);
        Task<Tag?> GetByNameAsync(string name);
        Task<IEnumerable<Tag>> GetAllAsync();
        Task AddAsync(Tag tag);
        Task AddRangeAsync(IEnumerable<Tag> tags);
        Task UpdateAsync(Tag tag);
        Task DeleteAsync(Tag tag);
        Task<List<Tag>> GetOrCreateTagsAsync(IEnumerable<string> tagNames);
    }
}