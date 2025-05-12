using System.ComponentModel.DataAnnotations;

namespace Blog.Application.Contracts.TagDtos
{
    public class TagDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }
    }
}
