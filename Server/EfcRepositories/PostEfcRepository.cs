using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace EfcRepositories;

public class PostEfcRepository : IPostRepository
{
    private readonly ForumContext context;

    public PostEfcRepository(ForumContext context)
    {
        this.context = context;
    }

    public async Task<Post> AddAsync(Post post)
    {
        await context.Posts.AddAsync(post);
        await context.SaveChangesAsync();
        return post;
    }

    public async Task UpdateAsync(Post post)
    {
        if (!await context.Posts.AnyAsync(item => item.Id == post.Id))
            throw new InvalidOperationException($"Post with ID '{post.Id}' not found");

        context.Posts.Update(post);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        Post post = await GetSingleAsync(id);
        context.Posts.Remove(post);
        await context.SaveChangesAsync();
    }

    public async Task<Post> GetSingleAsync(int id)
    {
        Post? post = await context.Posts.SingleOrDefaultAsync(item => item.Id == id);
        return post ?? throw new InvalidOperationException($"Post with ID '{id}' not found");
    }

    public IQueryable<Post> GetMany()
    {
        return context.Posts.AsQueryable();
    }
}
