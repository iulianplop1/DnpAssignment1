using ApiContracts;

namespace BlazorApp.HttpServices;

public class HttpUserService : IUserService
{
    private readonly HttpClient client;

    public HttpUserService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<UserDto> AddUserAsync(CreateUserDto request)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync("Users", request);
        return await ApiResponse.ReadAsync<UserDto>(response);
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        using HttpResponseMessage response = await client.GetAsync("Users");
        return await ApiResponse.ReadAsync<List<UserDto>>(response);
    }
}
