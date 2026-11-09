using ApiContracts;

namespace BlazorApp.HttpServices;

public interface IPostService
{
    Task<List<PostDto>> GetPostsAsync();
    Task<PostDto> GetPostAsync(int id);
    Task<PostDto> AddPostAsync(CreatePostDto request);
    Task UpdatePostAsync(int id, UpdatePostDto request);
}
