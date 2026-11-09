using ApiContracts;

namespace BlazorApp.HttpServices;

public class HttpCommentService : ICommentService
{
    private readonly HttpClient client;

    public HttpCommentService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<List<CommentDto>> GetCommentsAsync(int postId)
    {
        using HttpResponseMessage response = await client.GetAsync($"Posts/{postId}/Comments");
        return await ApiResponse.ReadAsync<List<CommentDto>>(response);
    }

    public async Task<CommentDto> AddCommentAsync(int postId, CreateCommentDto request)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync($"Posts/{postId}/Comments", request);
        return await ApiResponse.ReadAsync<CommentDto>(response);
    }
}
