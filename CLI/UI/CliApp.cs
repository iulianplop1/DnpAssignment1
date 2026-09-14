using Entities;
using RepositoryContracts;

namespace CLI.UI;

public class CliApp
{
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;

    public CliApp(
        IUserRepository userRepository,
        IPostRepository postRepository,
        ICommentRepository commentRepository)
    {
        this.userRepository = userRepository;
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    public async Task StartAsync()
    {
        Console.WriteLine("Welcome to the forum CLI!");

        while (true)
        {
            PrintMenu();
            string choice = Console.ReadLine()?.Trim() ?? string.Empty;

            try
            {
                switch (choice)
                {
                    case "1":
                        await CreateUserAsync();
                        break;
                    case "2":
                        await CreatePostAsync();
                        break;
                    case "3":
                        await AddCommentAsync();
                        break;
                    case "4":
                        await ShowPostOverviewAsync();
                        break;
                    case "5":
                        await ShowPostDetailsAsync();
                        break;
                    case "0":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Please choose a number from 0 to 5.");
                        break;
                }
            }
            catch (InvalidOperationException exception)
            {
                Console.WriteLine($"Error: {exception.Message}");
            }

            Console.WriteLine();
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine("------------------------------");
        Console.WriteLine("1. Create user");
        Console.WriteLine("2. Create post");
        Console.WriteLine("3. Add comment to post");
        Console.WriteLine("4. View post overview");
        Console.WriteLine("5. View specific post");
        Console.WriteLine("0. Exit");
        Console.Write("Choose an option: ");
    }

    private async Task CreateUserAsync()
    {
        Console.WriteLine("\nCREATE USER");
        string userName = ReadRequiredText("User name: ");
        string password = ReadRequiredText("Password: ");

        bool userNameTaken = userRepository.GetMany()
            .Any(user => user.UserName.Equals(
                userName,
                StringComparison.OrdinalIgnoreCase));

        if (userNameTaken)
        {
            Console.WriteLine("A user with that name already exists.");
            return;
        }

        User user = new User
        {
            UserName = userName,
            Password = password
        };

        User createdUser = await userRepository.AddAsync(user);
        Console.WriteLine($"User created successfully with ID {createdUser.Id}.");
    }

    private async Task CreatePostAsync()
    {
        Console.WriteLine("\nCREATE POST");
        int userId = ReadPositiveInt("Author user ID: ");

        User author = await userRepository.GetSingleAsync(userId);
        string title = ReadRequiredText("Title: ");
        string body = ReadRequiredText("Body: ");

        Post post = new Post
        {
            Title = title,
            Body = body,
            UserId = author.Id
        };

        Post createdPost = await postRepository.AddAsync(post);
        Console.WriteLine($"Post created successfully with ID {createdPost.Id}.");
    }

    private async Task AddCommentAsync()
    {
        Console.WriteLine("\nADD COMMENT");
        int userId = ReadPositiveInt("Comment author user ID: ");
        int postId = ReadPositiveInt("Post ID: ");

        User author = await userRepository.GetSingleAsync(userId);
        Post post = await postRepository.GetSingleAsync(postId);
        string body = ReadRequiredText("Comment: ");

        Comment comment = new Comment
        {
            Body = body,
            UserId = author.Id,
            PostId = post.Id
        };

        Comment createdComment = await commentRepository.AddAsync(comment);
        Console.WriteLine($"Comment added successfully with ID {createdComment.Id}.");
    }

    private async Task ShowPostOverviewAsync()
    {
        Console.WriteLine("\nPOST OVERVIEW");

        List<Post> posts = postRepository.GetMany()
            .OrderBy(post => post.Id)
            .ToList();

        if (posts.Count == 0)
        {
            Console.WriteLine("There are no posts.");
            return;
        }

        foreach (Post post in posts)
        {
            User author = await userRepository.GetSingleAsync(post.UserId);
            Console.WriteLine($"[{post.Id}] {post.Title} - by {author.UserName}");
        }
    }

    private async Task ShowPostDetailsAsync()
    {
        Console.WriteLine("\nPOST DETAILS");
        int postId = ReadPositiveInt("Post ID: ");

        Post post = await postRepository.GetSingleAsync(postId);
        User author = await userRepository.GetSingleAsync(post.UserId);

        Console.WriteLine("------------------------------");
        Console.WriteLine($"ID: {post.Id}");
        Console.WriteLine($"Title: {post.Title}");
        Console.WriteLine($"Author: {author.UserName}");
        Console.WriteLine($"Body: {post.Body}");
        Console.WriteLine("Comments:");

        List<Comment> comments = commentRepository.GetMany()
            .Where(comment => comment.PostId == post.Id)
            .OrderBy(comment => comment.Id)
            .ToList();

        if (comments.Count == 0)
        {
            Console.WriteLine("No comments yet.");
            return;
        }

        foreach (Comment comment in comments)
        {
            User commentAuthor = await userRepository.GetSingleAsync(comment.UserId);
            Console.WriteLine(
                $"[{comment.Id}] {commentAuthor.UserName}: {comment.Body}");
        }
    }

    private static string ReadRequiredText(string message)
    {
        while (true)
        {
            Console.Write(message);
            string value = Console.ReadLine()?.Trim() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            Console.WriteLine("The value cannot be empty.");
        }
    }

    private static int ReadPositiveInt(string message)
    {
        while (true)
        {
            Console.Write(message);
            string input = Console.ReadLine()?.Trim() ?? string.Empty;

            if (int.TryParse(input, out int value) && value > 0)
            {
                return value;
            }

            Console.WriteLine("Please enter a positive whole number.");
        }
    }
}
