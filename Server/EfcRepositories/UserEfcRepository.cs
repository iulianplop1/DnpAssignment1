using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace EfcRepositories;

public class UserEfcRepository : IUserRepository
{
    private readonly ForumContext context;

    public UserEfcRepository(ForumContext context)
    {
        this.context = context;
    }

    public async Task<User> AddAsync(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        if (!await context.Users.AnyAsync(item => item.Id == user.Id))
            throw new InvalidOperationException($"User with ID '{user.Id}' not found");

        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        User user = await GetSingleAsync(id);
        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }

    public async Task<User> GetSingleAsync(int id)
    {
        User? user = await context.Users.SingleOrDefaultAsync(item => item.Id == id);
        return user ?? throw new InvalidOperationException($"User with ID '{id}' not found");
    }

    public IQueryable<User> GetMany()
    {
        return context.Users.AsQueryable();
    }
}
