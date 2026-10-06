namespace ApiContracts;

public class CreateCommentDto
{
    public required string Body { get; set; }
    public int UserId { get; set; }
    // The post ID comes from the route: /Posts/{postId}/Comments.
}
