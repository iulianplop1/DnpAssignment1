using ApiContracts;

namespace BlazorApp.HttpServices;

public class HttpPostService : IPostService
{
    private readonly HttpClient client;

    public HttpPostService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<List<PostDto>> GetPostsAsync()
    {
        using HttpResponseMessage response = await client.GetAsync("Posts");
        return await ApiResponse.ReadAsync<List<PostDto>>(response);
    }

    public async Task<PostDto> GetPostAsync(int id)
    {
        using HttpResponseMessage response = await client.GetAsync($"Posts/{id}");
        return await ApiResponse.ReadAsync<PostDto>(response);
    }

    public async Task<PostDto> AddPostAsync(CreatePostDto request)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync("Posts", request);
        return await ApiResponse.ReadAsync<PostDto>(response);
    }

    public async Task UpdatePostAsync(int id, UpdatePostDto request)
    {
        using HttpResponseMessage response = await client.PatchAsJsonAsync($"Posts/{id}", request);
        await ApiResponse.CheckAsync(response);
    }
}
