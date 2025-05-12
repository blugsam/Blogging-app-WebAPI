using Blog.Application.Contracts.TagDtos;

namespace Blog.Application.Contracts.PostDtos
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Slug { get; set; }
        public string? Content { get; set; }
        public string? Excerpt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsPublished { get; set; }
        public required List<TagDto> Tags { get; set; }
    }
}
