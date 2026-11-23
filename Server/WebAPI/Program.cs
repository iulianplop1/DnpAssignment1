using EfcRepositories;
using RepositoryContracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<ForumContext>();
builder.Services.AddScoped<IUserRepository, UserEfcRepository>();
builder.Services.AddScoped<IPostRepository, PostEfcRepository>();
builder.Services.AddScoped<ICommentRepository, CommentEfcRepository>();

var app = builder.Build();

app.MapControllers();
app.Run();
