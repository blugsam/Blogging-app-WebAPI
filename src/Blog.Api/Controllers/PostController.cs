using Blog.Application.Contracts.PostDtos;
using Blog.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers
{
    [ApiController]
    [Route("api/posts")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostsController> _logger;

        public PostsController(IPostService postService, ILogger<PostsController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        // GET: api/posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? tagName = null)
        {
            var posts = await _postService.GetAllPostsAsync(pageNumber, pageSize, tagName);
            return Ok(posts);
        }

        // GET: api/posts/{slug}
        [HttpGet("{slug}")]
        public async Task<ActionResult<PostDto>> GetPost(string slug)
        {
            var post = await _postService.GetPostBySlugAsync(slug);

            if (post == null)
            {
                _logger.LogInformation("Post with slug '{Slug}' not found.", slug);
                return NotFound(new { Message = $"Post with slug '{slug}' not found." });
            }

            return Ok(post);
        }

        // POST: api/posts
        [HttpPost]
        public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostDto createPostDto)
        {
            try
            {
                var createdPost = await _postService.CreatePostAsync(createPostDto);
                _logger.LogInformation("Post with ID {PostId} and Slug '{Slug}' created successfully.", createdPost.Id, createdPost.Slug);
                return CreatedAtAction(nameof(GetPost), new { slug = createdPost.Slug }, createdPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating post with Title '{Title}'. DTO: {@CreatePostDto}", createPostDto.Title, createPostDto);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while creating the post.");
            }
        }

        // PUT: api/posts/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PostDto>> UpdatePost(Guid id, [FromBody] UpdatePostDto updatePostDto)
        {
            try
            {
                var updatedPost = await _postService.UpdatePostAsync(id, updatePostDto);

                if (updatedPost == null)
                {
                    _logger.LogWarning("Post with ID {PostId} not found for update.", id);
                    return NotFound(new { Message = $"Post with ID '{id}' not found for update." });
                }
                _logger.LogInformation("Post with ID {PostId} updated successfully.", id);
                return Ok(updatedPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating post with ID {PostId}. DTO: {@UpdatePostDto}", id, updatePostDto);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while updating the post.");
            }
        }

        // DELETE: api/posts/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            await _postService.DeletePostAsync(id);
            _logger.LogInformation("Post with ID {PostId} deleted successfully.", id);
            return NoContent();
        }

    }
}