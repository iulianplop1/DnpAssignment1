using Microsoft.EntityFrameworkCore;
using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;

    public UsersController(
        IUserRepository userRepository,
        IPostRepository postRepository,
        ICommentRepository commentRepository)
    {
        this.userRepository = userRepository;
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser([FromBody] CreateUserDto request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("User name and password cannot be empty.");
        }

        string userName = request.UserName.Trim();
        List<User> users = await userRepository.GetMany().ToListAsync();
        bool nameTaken = users.Any(user => user.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));

        if (nameTaken)
        {
            return Conflict("A user with that name already exists.");
        }

        User user = new User { UserName = userName, Password = request.Password };
        User created = await userRepository.AddAsync(user);
        UserDto dto = ToDto(created);
        return Created($"/Users/{dto.Id}", dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserDto request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("User name and password cannot be empty.");
        }

        try
        {
            User user = await userRepository.GetSingleAsync(id);
            string userName = request.UserName.Trim();
            List<User> users = await userRepository.GetMany().ToListAsync();
            bool nameTaken = users.Any(other => other.Id != id && other.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));

            if (nameTaken)
            {
                return Conflict("A user with that name already exists.");
            }

            user.UserName = userName;
            user.Password = request.Password;
            await userRepository.UpdateAsync(user);
            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUser([FromRoute] int id)
    {
        try
        {
            User user = await userRepository.GetSingleAsync(id);
            return Ok(ToDto(user));
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetUsers([FromQuery] string? userNameContains = null)
    {
        List<User> users = await userRepository.GetMany().OrderBy(user => user.Id).ToListAsync();

        if (!string.IsNullOrWhiteSpace(userNameContains))
        {
            users = users.Where(user => user.UserName.Contains(userNameContains, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        List<UserDto> dtos = users.Select(user => ToDto(user)).ToList();
        return Ok(dtos);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        try
        {
            await userRepository.GetSingleAsync(id);

            // Keep posts and comments from referencing a user who no longer exists.
            if (await postRepository.GetMany().AnyAsync(post => post.UserId == id)
                || await commentRepository.GetMany().AnyAsync(comment => comment.UserId == id))
            {
                return Conflict("Delete this user's posts and comments before deleting the user.");
            }

            await userRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(exception.Message);
        }
    }

    private static UserDto ToDto(User user)
    {
        // Passwords are stored in the entity, but never returned to the client.
        return new UserDto { Id = user.Id, UserName = user.UserName };
    }
}
