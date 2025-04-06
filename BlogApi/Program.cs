var builder = WebApplication.CreateBuilder(args);

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.MapGet("/", () => "Hello World!");

app.MapGet("api/blogs", () => new []
{
    new { Id = 1, Title = "First Blog" , Content = "This is the first blog" },
    new { Id = 2, Title = "Second Blog" , Content = "This is the second blog" }
});

app.Run();
