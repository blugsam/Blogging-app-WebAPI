using Blog.Application.Contracts.PostDtos;

namespace Blog.Application.Services
{
    public interface IPostService
    {
        Task<PostDto> GetPostBySlugAsync(string slug);
        Task<IEnumerable<PostDto>> GetAllPostsAsync(int pageNumber, int pageSize, string? tagName = null);
        Task<PostDto> CreatePostAsync(CreatePostDto createPostDto);
        Task<PostDto> UpdatePostAsync(Guid postId, UpdatePostDto updatePostDto);
        Task DeletePostAsync(Guid postId);
    }
}
