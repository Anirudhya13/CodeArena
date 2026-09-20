using CodeArena.Infrastructure.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

var app = builder.Build();

// Ensure the data directory exists and initialize the database safely.
// We wrap this in a try-catch because during 'dotnet build', the OpenAPI generator runs this file 
// without the proper runtime environment, which causes SQLite to throw an error.
try 
{
    var dataDir = Path.Combine(Directory.GetCurrentDirectory(), "data");
    if (!Directory.Exists(dataDir)) 
    {
        Directory.CreateDirectory(dataDir);
    }
    await app.InitialiseDatabaseAsync();
} 
catch (Exception ex)
{
    Console.WriteLine($"Database initialization skipped during build/tooling: {ex.Message}");
}

// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors(static builder => 
    builder.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin());

app.UseFileServer();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseExceptionHandler(options => { });

app.MapDefaultEndpoints();
app.MapEndpoints(typeof(Program).Assembly);
app.MapHub<CodeArena.Web.Hubs.SubmissionHub>("/hubs/submission");

app.MapFallbackToFile("index.html");

app.Run();
