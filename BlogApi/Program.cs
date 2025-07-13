using Microsoft.EntityFrameworkCore;
using BlogApi.Data;
using BlogApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add database
builder.Services.AddDbContext<BlogContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Listen on ports from configuration or default to 5077/5078
    var httpPort = builder.Configuration.GetValue<int?>("ASPNETCORE_HTTP_PORT") ?? 5077;
    var httpsPort = builder.Configuration.GetValue<int?>("ASPNETCORE_HTTPS_PORT") ?? 5078;
    
    serverOptions.ListenAnyIP(httpPort);
    serverOptions.ListenAnyIP(httpsPort);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
    context.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.MapGet("/", () => "Blog API");

app.MapGet("/api/blogs", async (BlogContext db) =>
{
    return await db.Blogs.ToListAsync();
});

app.MapGet("/api/blogs/{id}", async (int id, BlogContext db) =>
{
    var blog = await db.Blogs.FindAsync(id);
    return blog is not null ? Results.Ok(blog) : Results.NotFound();
});

app.MapPost("/api/blogs", async (Blog blog, BlogContext db) =>
{
    if (string.IsNullOrWhiteSpace(blog.Title) || string.IsNullOrWhiteSpace(blog.Content))
    {
        return Results.BadRequest("Title and Content are required");
    }

    blog.CreatedAt = DateTime.UtcNow;
    blog.UpdatedAt = DateTime.UtcNow;
    
    db.Blogs.Add(blog);
    await db.SaveChangesAsync();
    
    return Results.Created($"/api/blogs/{blog.Id}", blog);
});

app.MapPut("/api/blogs/{id}", async (int id, Blog updatedBlog, BlogContext db) =>
{
    var blog = await db.Blogs.FindAsync(id);
    if (blog is null) return Results.NotFound();

    if (string.IsNullOrWhiteSpace(updatedBlog.Title) || string.IsNullOrWhiteSpace(updatedBlog.Content))
    {
        return Results.BadRequest("Title and Content are required");
    }

    blog.Title = updatedBlog.Title;
    blog.Content = updatedBlog.Content;
    blog.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok(blog);
});

app.MapDelete("/api/blogs/{id}", async (int id, BlogContext db) =>
{
    var blog = await db.Blogs.FindAsync(id);
    if (blog is null) return Results.NotFound();

    db.Blogs.Remove(blog);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
