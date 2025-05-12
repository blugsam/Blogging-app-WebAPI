using Blog.Application.Interfaces.Persistence;
using Blog.Domain.Entities;
using Blog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _context;

        public TagRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Tag?> GetByIdAsync(Guid id)
        {
            return await _context.Tags.FindAsync(id);
        }

        public async Task<Tag?> GetByNameAsync(string name)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await _context.Tags.OrderBy(t => t.Name).ToListAsync();
        }

        public async Task AddAsync(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Tag> tags)
        {
            await _context.Tags.AddRangeAsync(tags);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tag tag)
        {
            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Tag tag)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Tag>> GetOrCreateTagsAsync(IEnumerable<string> tagNames)
        {
            var distinctTagNames = tagNames
                .Select(tn => tn.Trim().ToLower())
                .Where(tn => !string.IsNullOrWhiteSpace(tn))
                .Distinct()
                .ToList();

            var existingTags = await _context.Tags
                .Where(t => distinctTagNames.Contains(t.Name.ToLower()))
                .ToListAsync();

            var newTagNames = distinctTagNames.Except(existingTags.Select(et => et.Name.ToLower())).ToList();

            var newTags = new List<Tag>();
            if (newTagNames.Any())
            {
                foreach (var newTagName in newTagNames)
                {
                    var originalCaseTagName = tagNames.First(tn => tn.Trim().Equals(newTagName, StringComparison.OrdinalIgnoreCase));
                    var tag = new Tag { Name = originalCaseTagName, Id = Guid.NewGuid() };
                    newTags.Add(tag);
                }
                await _context.Tags.AddRangeAsync(newTags);
                await _context.SaveChangesAsync();
            }

            return existingTags.Concat(newTags).ToList();
        }
    }
}