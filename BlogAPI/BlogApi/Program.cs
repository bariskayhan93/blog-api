var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

builder.WebHost.UseUrls("http://localhost:5077", "https://localhost:5078");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else{
    app.UseHttpsRedirection();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");

app.MapGet("api/blogs", () => new []
{
    new { Id = 1, Title = "First Blog" , Content = "This is the first blog" },
    new { Id = 2, Title = "Second Blog" , Content = "This is the second blog" }
});

app.Run();