using SongSpiration.BLL;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;
using SongSpiration.DAL;
using SongSpiration.Models.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DAL with SQL Server (or in-memory for development/testing)
builder.Services.AddSongSpirationDal(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.UseInMemoryDatabase("SongSpirationDb");
    }
    else
    {
        // Configure for SQL Server in production
        // var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        // options.UseSqlServer(connectionString);
    }
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

app.UseCors(); // Use the CORS policy

app.MapControllers(); // Map controllers

app.Run();
