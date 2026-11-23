using Microsoft.EntityFrameworkCore;
using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository userRepository;

    public AuthController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginRequest request)
    {
        string userName = request.UserName.Trim();
        List<User> users = await userRepository.GetMany().ToListAsync();
        User? user = users.SingleOrDefault(
            user => user.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));

        if (user is null || user.Password != request.Password)
        {
            return Unauthorized("Invalid user name or password.");
        }

        return Ok(new UserDto { Id = user.Id, UserName = user.UserName });
    }
}
