using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepository;

    public CommentsController(ICommentRepository commentRepository)
    {
        this.commentRepository = commentRepository;
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody] UpdateCommentDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Body))
        {
            return BadRequest("Comment body cannot be empty.");
        }

        try
        {
            Comment comment = await commentRepository.GetSingleAsync(id);
            comment.Body = request.Body;
            await commentRepository.UpdateAsync(comment);
            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetComment([FromRoute] int id)
    {
        try
        {
            Comment comment = await commentRepository.GetSingleAsync(id);
            return Ok(ToDto(comment));
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpGet]
    public ActionResult<List<CommentDto>> GetComments(
        [FromQuery] int? userId = null,
        [FromQuery] int? postId = null)
    {
        if (userId <= 0 || postId <= 0)
        {
            return BadRequest("User ID and post ID must be positive.");
        }

        IQueryable<Comment> comments = commentRepository.GetMany();

        if (userId.HasValue)
        {
            comments = comments.Where(comment => comment.UserId == userId.Value);
        }

        if (postId.HasValue)
        {
            comments = comments.Where(comment => comment.PostId == postId.Value);
        }

        List<CommentDto> dtos = comments.OrderBy(comment => comment.Id)
            .Select(comment => ToDto(comment)).ToList();
        return Ok(dtos);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteComment([FromRoute] int id)
    {
        try
        {
            await commentRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(exception.Message);
        }
    }

    private static CommentDto ToDto(Comment comment)
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
