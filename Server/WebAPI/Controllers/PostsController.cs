using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;
    private readonly ICommentRepository commentRepository;

    public PostsController(
        IPostRepository postRepository,
        IUserRepository userRepository,
        ICommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
        this.commentRepository = commentRepository;
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> AddPost([FromBody] CreatePostDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Body))
        {
            return BadRequest("Title and body cannot be empty.");
        }

        if (!userRepository.GetMany().Any(user => user.Id == request.UserId))
        {
            return BadRequest("The author must be an existing user.");
        }

        Post post = new Post
        {
            Title = request.Title.Trim(),
            Body = request.Body,
            UserId = request.UserId
        };
        Post created = await postRepository.AddAsync(post);
        PostDto dto = ToDto(created);
        return Created($"/Posts/{dto.Id}", dto);
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdatePost([FromRoute] int id, [FromBody] UpdatePostDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Body))
        {
            return BadRequest("Title and body cannot be empty.");
        }

        try
        {
            Post post = await postRepository.GetSingleAsync(id);
            // Update the content while keeping the original author.
            post.Title = request.Title.Trim();
            post.Body = request.Body;
            await postRepository.UpdateAsync(post);
            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetPost([FromRoute] int id)
    {
        try
        {
            Post post = await postRepository.GetSingleAsync(id);
            return Ok(ToDto(post));
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpGet]
    public ActionResult<List<PostDto>> GetPosts(
        [FromQuery] string? titleContains = null,
        [FromQuery] int? userId = null)
    {
        if (userId <= 0)
        {
            return BadRequest("User ID must be positive.");
        }

        IQueryable<Post> posts = postRepository.GetMany();

        if (!string.IsNullOrWhiteSpace(titleContains))
        {
            posts = posts.Where(post => post.Title.Contains(titleContains, StringComparison.OrdinalIgnoreCase));
        }

        if (userId.HasValue)
        {
            posts = posts.Where(post => post.UserId == userId.Value);
        }

        List<PostDto> dtos = posts.OrderBy(post => post.Id)
            .Select(post => ToDto(post)).ToList();
        return Ok(dtos);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePost([FromRoute] int id)
    {
        try
        {
            await postRepository.GetSingleAsync(id);

            if (commentRepository.GetMany().Any(comment => comment.PostId == id))
            {
                return Conflict("Delete this post's comments before deleting the post.");
            }

            await postRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpPost("{postId:int}/Comments")]
    public async Task<ActionResult<CommentDto>> AddComment(
        [FromRoute] int postId,
        [FromBody] CreateCommentDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Body))
        {
            return BadRequest("Comment body cannot be empty.");
        }

        try
        {
            await postRepository.GetSingleAsync(postId);

            if (!userRepository.GetMany().Any(user => user.Id == request.UserId))
            {
                return BadRequest("The author must be an existing user.");
            }

            Comment comment = new Comment
            {
                Body = request.Body,
                UserId = request.UserId,
                PostId = postId
            };
            Comment created = await commentRepository.AddAsync(comment);
            CommentDto dto = ToCommentDto(created);
            return Created($"/Comments/{dto.Id}", dto);
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpGet("{postId:int}/Comments")]
    public async Task<ActionResult<List<CommentDto>>> GetPostComments(
        [FromRoute] int postId,
        [FromQuery] int? userId = null)
    {
        if (userId <= 0)
        {
            return BadRequest("User ID must be positive.");
        }

        try
        {
            await postRepository.GetSingleAsync(postId);
            IQueryable<Comment> comments = commentRepository.GetMany()
                .Where(comment => comment.PostId == postId);

            if (userId.HasValue)
            {
                comments = comments.Where(comment => comment.UserId == userId.Value);
            }

            List<CommentDto> dtos = comments.OrderBy(comment => comment.Id)
                .Select(comment => ToCommentDto(comment)).ToList();
            return Ok(dtos);
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(exception.Message);
        }
    }

    private static PostDto ToDto(Post post)
    {
        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId
        };
    }

    private static CommentDto ToCommentDto(Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Body = comment.Body,
            UserId = comment.UserId,
            PostId = comment.PostId
        };
    }
}
