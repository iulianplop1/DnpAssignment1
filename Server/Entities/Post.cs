namespace Entities;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public List<Comment> Comments { get; set; } = [];
}