using System.ComponentModel.DataAnnotations;

namespace Blog.Application.Contracts.PostDtos
{
    public class CreatePostDto
    {
        [Required]
        [StringLength(200)]
        public string? Title { get; set; }

        [Required]
        public string? Content { get; set; }

        [StringLength(500)]
        public string? Excerpt { get; set; }

        public required List<string> TagNames { get; set; }
    }
}
