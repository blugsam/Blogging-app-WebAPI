using BloggingApp.Blog.Domain;

namespace Blog.Domain
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty; // Инициализируем строки, чтобы избежать null
        public string Slug { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty; // Краткое описание
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsPublished { get; set; }

        // Навигационное свойство для связи многие-ко-многим с Тегами
        public ICollection<Tag> Tags { get; set; } = new List<Tag>(); // Инициализируем коллекцию
    }
}