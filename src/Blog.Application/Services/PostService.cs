using AutoMapper;
using Blog.Application.Abstractions;
using Blog.Application.Contracts.PostDtos;
using Blog.Application.Exceptions;
using Blog.Domain.Entities;
using System.Text.RegularExpressions;

namespace Blog.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        public PostService(IPostRepository postRepository, ITagRepository tagRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        public async Task<PostDto> CreatePostAsync(CreatePostDto createPostDto)
        {
            var post = _mapper.Map<Post>(createPostDto);

            // Генерация уникального слага (передаем null, т.к. это новый пост)
            post.Slug = await GenerateUniqueSlugAsync(createPostDto.Title, null);
            post.CreatedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;
            post.IsPublished = false;

            await HandlePostTagsAsync(post, createPostDto.TagNames);

            var createdPost = await _postRepository.AddAsync(post);
            return _mapper.Map<PostDto>(createdPost);
        }

        public async Task<PostDto> UpdatePostAsync(Guid postId, UpdatePostDto updatePostDto)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                throw new NotFoundException(nameof(Post), postId);
            }


            var originalTitle = post.Title;
            var titleChanged = !string.IsNullOrEmpty(updatePostDto.Title) && originalTitle != updatePostDto.Title;

            _mapper.Map(updatePostDto, post);

            if (titleChanged)
            {
                post.Slug = await GenerateUniqueSlugAsync(post.Title, postId);
            }

            post.UpdatedAt = DateTime.UtcNow;

            await HandlePostTagsAsync(post, updatePostDto.TagNames);

            await _postRepository.UpdateAsync(post);
            return _mapper.Map<PostDto>(post);
        }


        public async Task<PostDto> GetPostBySlugAsync(string slug)
        {
            var post = await _postRepository.GetBySlugAsync(slug);
            if (post == null)
            {
                throw new NotFoundException($"Post with slug '{slug}' was not found.");
            }
            return _mapper.Map<PostDto>(post);
        }

        public async Task<IEnumerable<PostDto>> GetAllPostsAsync(int pageNumber, int pageSize, string? tagName = null)
        {
            var posts = await _postRepository.GetPublishedPostsAsync(pageNumber, pageSize, tagName);
            return _mapper.Map<IEnumerable<PostDto>>(posts);
        }

        public async Task DeletePostAsync(Guid postId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                throw new NotFoundException(nameof(Post), postId);
            }
            await _postRepository.DeleteAsync(post);
        }

        private async Task<string> GenerateUniqueSlugAsync(string? title, Guid? currentPostId)
        {
            if (string.IsNullOrWhiteSpace(title))
                return $"post-{Guid.NewGuid().ToString().Substring(0, 8)}";

            var baseSlug = Regex.Replace(title.ToLowerInvariant().Trim(), @"\s+", "-");
            baseSlug = Regex.Replace(baseSlug, @"[^a-z0-9\-]", "");
            baseSlug = baseSlug.Length > 100 ? baseSlug.Substring(0, 100) : baseSlug;
            baseSlug = baseSlug.Trim('-');
            if (string.IsNullOrWhiteSpace(baseSlug))
                baseSlug = $"post-{DateTime.UtcNow:yyyyMMddHHmmss}";

            var slug = baseSlug;
            var count = 1;
            while (await _postRepository.SlugExistsAsync(slug, currentPostId))
            {
                slug = $"{baseSlug}-{count++}";
                if (count > 100)
                {
                    throw new SlugGenerationException($"Could not generate a unique slug for title '{title}' after {count - 1} attempts.");
                }
            }
            return slug;
        }

        private async Task HandlePostTagsAsync(Post post, List<string> tagNames)
        {
            if (post.PostTags == null) post.PostTags = new List<PostTag>();
            var currentTags = post.PostTags.Select(pt => pt.Tag.Name).ToList();
            var newTagNames = tagNames?.Select(tn => tn.Trim().ToLowerInvariant()).Where(tn => !string.IsNullOrWhiteSpace(tn)).Distinct().ToList() ?? new List<string>();

            var tagsToRemove = post.PostTags
                .Where(pt => !newTagNames.Contains(pt.Tag.Name, StringComparer.OrdinalIgnoreCase))
                .ToList();
            foreach (var tagToRemove in tagsToRemove)
            {
                post.PostTags.Remove(tagToRemove);
            }

            var tagNamesToAdd = newTagNames
                .Where(newName => !currentTags.Contains(newName, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (tagNamesToAdd.Any())
            {
                foreach (var tagName in tagNamesToAdd)
                {
                    var tag = await _tagRepository.GetByNameAsync(tagName);
                    if (tag == null)
                    {
                        tag = new Tag { Name = tagName };
                    }
                    post.PostTags.Add(new PostTag { Post = post, Tag = tag });
                }
            }
        }
    }
}