using Blog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Здесь можно дополнительно сконфигурировать модели, если нужно
            // Например, уникальные индексы, ограничения длины строк и т.д.

            // Пример: Делаем Slug уникальным
            modelBuilder.Entity<Post>()
                .HasIndex(p => p.Slug)
                .IsUnique();

            // Пример: Делаем Name у тега уникальным
            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Name)
                .IsUnique();

            // Конфигурация связи многие-ко-многим (EF Core 5+ может делать это по соглашению,
            // но явное определение может быть полезно для понимания или кастомизации)
            modelBuilder.Entity<Post>()
                .HasMany(p => p.Tags)
                .WithMany(t => t.Posts)
                .UsingEntity(j => j.ToTable("PostTags")); // Явно указываем имя промежуточной таблицы (опционально)
        }
    }
}