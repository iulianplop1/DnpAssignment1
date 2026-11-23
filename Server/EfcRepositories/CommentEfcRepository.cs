using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace EfcRepositories;

public class CommentEfcRepository : ICommentRepository
{
    private readonly ForumContext context;

    public CommentEfcRepository(ForumContext context)
    {
        this.context = context;
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        await context.Comments.AddAsync(comment);
        await context.SaveChangesAsync();
        return comment;
    }

    public async Task UpdateAsync(Comment comment)
    {
        if (!await context.Comments.AnyAsync(item => item.Id == comment.Id))
            throw new InvalidOperationException($"Comment with ID '{comment.Id}' not found");

        context.Comments.Update(comment);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        Comment comment = await GetSingleAsync(id);
        context.Comments.Remove(comment);
        await context.SaveChangesAsync();
    }

    public async Task<Comment> GetSingleAsync(int id)
    {
        Comment? comment = await context.Comments.SingleOrDefaultAsync(item => item.Id == id);
        return comment ?? throw new InvalidOperationException($"Comment with ID '{id}' not found");
    }

    public IQueryable<Comment> GetMany()
    {
        return context.Comments.AsQueryable();
    }
}
