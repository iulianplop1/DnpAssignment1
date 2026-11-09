using System.Security.Claims;
using ApiContracts;
using BlazorApp.HttpServices;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorApp.Auth;

public class SimpleAuthProvider : AuthenticationStateProvider
{
    private readonly HttpClient client;
    private ClaimsPrincipal currentUser = new ClaimsPrincipal(new ClaimsIdentity());

    public SimpleAuthProvider(HttpClient client)
    {
        this.client = client;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(currentUser));
    }

    public async Task LoginAsync(string userName, string password)
    {
        LoginRequest request = new LoginRequest { UserName = userName, Password = password };
        using HttpResponseMessage response = await client.PostAsJsonAsync("auth/login", request);
        UserDto user = await ApiResponse.ReadAsync<UserDto>(response);

        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        currentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "apiauth"));
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void Logout()
    {
        currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
