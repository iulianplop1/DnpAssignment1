# DNP Assignment 4

This part adds a controller-based REST API to the existing forum application.

## Run the API

From the solution folder:

```powershell
dotnet run --project Server/WebAPI/WebAPI.csproj
```

The API listens at `http://localhost:5080`. In Rider, you can also choose the
WebAPI project's `http` run profile. Use `Server/WebAPI/WebAPI.http` to send
requests. Create a user first, then a post, then a comment. Update the ID
variables in the HTTP file with the IDs returned by those requests.

## Projects

- `Shared/ApiContracts`: simple request and response DTO classes.
- `Server/WebAPI`: controllers and repository registration in `Program.cs`.
- `Server/FileRepositories`: the existing JSON file persistence.
- `Server/RepositoryContracts` and `Server/Entities`: the existing interfaces and entities.

The controllers receive repository interfaces through their constructors.
They use `async`/`await`, object initializers, LINQ filtering and ordinary
`try`/`catch`, as shown in class. User responses contain ID and user name only.

## Routes

| Method | Route | Purpose |
|---|---|---|
| POST | `/Users` | Create a user |
| PUT | `/Users/{id}` | Update user name and password |
| GET | `/Users/{id}` | Get one user |
| GET | `/Users?userNameContains=ali` | List/filter users |
| DELETE | `/Users/{id}` | Delete a user |
| POST | `/Posts` | Create a post |
| PATCH | `/Posts/{id}` | Update title and body, keeping the author |
| GET | `/Posts/{id}` | Get one post |
| GET | `/Posts?titleContains=cats&userId=1` | List/filter posts |
| DELETE | `/Posts/{id}` | Delete a post |
| POST | `/Posts/{postId}/Comments` | Create a comment on a post |
| GET | `/Posts/{postId}/Comments?userId=1` | Get that post's comments |
| PATCH | `/Comments/{id}` | Update a comment's body |
| GET | `/Comments/{id}` | Get one comment |
| GET | `/Comments?userId=1&postId=1` | List/filter comments |
| DELETE | `/Comments/{id}` | Delete a comment |

Query parameters are optional and can be combined. Updates require all the
fields listed in their request DTO. The post and comment update DTOs exclude
author IDs and post relationships, so those relationships stay the same.

Successful creates return `201 Created` and a `Location` header. Reads return
`200 OK`, and updates/deletes return `204 No Content`. Invalid input returns
`400 Bad Request`, missing resources return `404 Not Found`, and duplicate
user names or deleting a resource that still has dependents return `409 Conflict`.
Delete comments before their post, and a user's posts/comments before the user.

The repositories save `users.json`, `posts.json` and `comments.json` in the
running process's working directory. These local data files are ignored by Git.

## Course references

- [Assignment 4 instructions](https://github.com/MichaelViuff/DNP/blob/main/Assignment/README.md#part-4---creating-a-rest-web-api)
- `dnp_webapi_02_rest_controllers.pdf`, pages 13–35: DTOs, controllers,
  dependency injection, HTTP verbs, query parameters and nested comment routes.
