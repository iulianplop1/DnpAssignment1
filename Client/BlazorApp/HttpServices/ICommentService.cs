using ApiContracts;

namespace BlazorApp.HttpServices;

public interface ICommentService
{
    Task<List<CommentDto>> GetCommentsAsync(int postId);
    Task<CommentDto> AddCommentAsync(int postId, CreateCommentDto request);
}
