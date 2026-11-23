namespace Entities;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<Post> Posts { get; set; } = [];
    public List<Comment> Comments { get; set; } = [];
}