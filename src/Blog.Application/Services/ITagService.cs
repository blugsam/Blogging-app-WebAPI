using Blog.Application.Contracts.TagDtos;

namespace Blog.Application.Services
{
    public interface ITagService
    {
        Task<IEnumerable<TagDto>> GetAllTagsAsync();
    }
}
