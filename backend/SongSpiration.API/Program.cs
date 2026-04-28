using SongSpiration.BLL;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;
using SongSpiration.DAL;
using SongSpiration.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "SongSpiration API", Version = "v1" });
});

// Add DAL with SQLite database
builder.Services.AddSongSpirationDal(options =>
{
    // Use SQLite database for both development and production
    var databasePath = Path.Combine(Directory.GetCurrentDirectory(), "SongSpiration.db");
    options.UseSqlite($"Data Source={databasePath}");
});

// Add BLL services
builder.Services.AddSongSpirationBll();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Allow frontend origin
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles(); // Enable static file serving

// Serve frontend files from the frontend/dist directory
var frontendPath = Path.Combine(Directory.GetCurrentDirectory(), "../../frontend/dist");
if (Directory.Exists(frontendPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(frontendPath),
        RequestPath = ""
    });

    // Serve .gp5 files specifically
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(frontendPath),
        RequestPath = "",
        ContentTypeProvider = new FileExtensionContentTypeProvider
        {
            Mappings = { [".gp5"] = "application/octet-stream" }
        }
    });
}

app.UseCors(); // Use the CORS policy

app.MapControllers(); // Map controllers

// Add fallback to serve index.html for client-side routing
app.MapFallbackToFile("index.html", new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(frontendPath),
    RequestPath = ""
});

app.Run();
