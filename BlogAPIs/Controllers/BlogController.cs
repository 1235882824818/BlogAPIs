using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BlogAPIs.Entities;
using BlogAPIs.VM;
using Microsoft.AspNetCore.Authorization;
using BlogAPIs.Data;

namespace BlogAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BlogController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BlogController> _logger;

        public BlogController(ApplicationDbContext context, ILogger<BlogController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("GetAllPosts")]
        public async Task<ActionResult<IEnumerable<Blog>>> GetAllPosts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User is not authenticated.");
                return Unauthorized("User is not authenticated.");
            }

            return await _context.Blogs.ToListAsync();
        }

        [HttpGet("GetPostByID")]
        public async Task<ActionResult<Blog>> GetPostByID(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User is not authenticated.");
                return Unauthorized("User is not authenticated.");
            }

            var post = await _context.Blogs.FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        [HttpGet("GetTopXPosts")]
        public async Task<ActionResult<Blog>> GetTopXPosts(int X)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User is not authenticated.");
                return Unauthorized("User is not authenticated.");
            }

            if (X > _context.Blogs.Count())
            {
                return BadRequest($"The Posts Are Less Than {X} Posts ");
            }
            else
            {
                var topxposts = _context.Blogs.Take(X).ToList();
                return Ok(topxposts);
            }
        }

        [HttpGet("GetPostsByIdRange")]
        public async Task<ActionResult<IEnumerable<Blog>>> GetPostsByIdRange(Guid startId, Guid endId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User is not authenticated.");
                    return Unauthorized("User is not authenticated.");
                }

                if (startId.CompareTo(endId) > 0)
                {
                    return BadRequest("Invalid range: startId should be less than or equal to endId.");
                }

                var postsByRange = await _context.Blogs
                    .Where(p => p.Id.CompareTo(startId) >= 0 && p.Id.CompareTo(endId) <= 0)
                    .ToListAsync();

                if (!postsByRange.Any())
                {
                    return NotFound();
                }

                return Ok(postsByRange);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching posts by ID range.");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("GetPostByKeywords")]
        public async Task<ActionResult<IEnumerable<Blog>>> GetPostByKeywords(string keywords)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User is not authenticated.");
                return Unauthorized("User is not authenticated.");
            }

            if (string.IsNullOrEmpty(keywords))
            {
                return BadRequest("Keywords cannot be null or empty.");
            }

            var posts = await _context.Blogs
                .Where(p => p.Title.Contains(keywords) || p.Content.Contains(keywords))
                .ToListAsync();

            if (posts == null || posts.Count == 0)
            {
                return NotFound();
            }

            return Ok(posts);
        }

        [HttpPost("CreatePost")]
        public async Task<ActionResult<Blog>> CreatePost([FromBody] PostVM post)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User is not authenticated.");
                return Unauthorized("User is not authenticated.");
            }

            try
            {
                var newPost = new Blog
                {
                    Content = post.Content,
                    Title = post.Title,
                    AuthorId = Guid.Parse(userId)
                };

                _context.Blogs.Add(newPost);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetPostByID), new { id = newPost.Id }, newPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new post.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating a new post.");
            }
        }

        [HttpPut("EditPost")]
        public async Task<IActionResult> EditPost(Guid id, [FromBody] PostVM post)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User is not authenticated.");
                return Unauthorized("User is not authenticated.");
            }

            if (PostExists(id))
            {
                var existingPost = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
                if (existingPost != null)
                {
                    existingPost.Title = post.Title;
                    existingPost.Content = post.Content;
                    await _context.SaveChangesAsync();
                    return Ok("Edited");
                }
                else
                {
                    return NotFound("Post not found");
                }
            }
            else
            {
                return BadRequest("Post not found");
            }
        }

        [HttpDelete("DeletePost")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User is not authenticated.");
                return Unauthorized("User is not authenticated.");
            }

            if (!PostExists(id))
            {
                return NotFound("Post not found");
            }

            var post = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (post == null)
            {
                return NotFound("Post not found");
            }

            _context.Blogs.Remove(post);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }

        private bool PostExists(Guid id) => _context.Blogs.Any(e => e.Id == id);
    }
}
