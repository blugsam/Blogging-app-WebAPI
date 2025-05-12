using System.ComponentModel.DataAnnotations;

namespace Blog.Application.Contracts.PostDtos
{
    public class UpdatePostDto
    {
        [Required]
        [StringLength(200)]
        public string? Title { get; set; }

        [Required]
        public string? Content { get; set; }

        [StringLength(500)]
        public string? Excerpt { get; set; }

        public bool IsPublished { get; set; }

        public List<string> TagNames { get; set; } = new();
    }
}
