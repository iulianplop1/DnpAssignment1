using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class PostInMemoryRepository : IPostRepository
{
    private readonly List<Post> posts =
    [
        new Post
        {
            Id = 1,
            Title = "Welcome to the forum",
            Body = "This is the first post in our forum.",
            UserId = 1
        },
        new Post
        {
            Id = 2,
            Title = "Learning C#",
            Body = "What is your favourite part of C#?",
            UserId = 2
        },
        new Post
        {
            Id = 3,
            Title = "Assignment questions",
            Body = "Use this post to discuss Assignment 2.",
            UserId = 3
        }
    ];

    public Task<Post> AddAsync(Post post)
    {
        post.Id = posts.Any() ? posts.Max(p => p.Id) + 1 : 1;
        posts.Add(post);
        return Task.FromResult(post);
    }

    public Task UpdateAsync(Post post)
    {
        Post? existingPost = posts.SingleOrDefault(p => p.Id == post.Id);
        if (existingPost is null)
            throw new InvalidOperationException($"Post with ID '{post.Id}' not found");

        posts.Remove(existingPost);
        posts.Add(post);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Post? post = posts.SingleOrDefault(p => p.Id == id);
        if (post is null)
            throw new InvalidOperationException($"Post with ID '{id}' not found");

        posts.Remove(post);
        return Task.CompletedTask;
    }

    public Task<Post> GetSingleAsync(int id)
    {
        Post? post = posts.SingleOrDefault(p => p.Id == id);
        if (post is null)
            throw new InvalidOperationException($"Post with ID '{id}' not found");

        return Task.FromResult(post);
    }

    public IQueryable<Post> GetMany()
    {
        return posts.AsQueryable();
    }
}
