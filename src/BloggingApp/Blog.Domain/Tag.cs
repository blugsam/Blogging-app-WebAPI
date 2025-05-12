namespace Blog.Domain
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Навигационное свойство для связи многие-ко-многим с Постами
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
