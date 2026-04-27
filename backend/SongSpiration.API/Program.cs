using SongSpiration.BLL;
using SongSpiration.DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSongSpirationBll();
builder.Services.AddSongSpirationDal(options => options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SongSpiration;Trusted_Connection=True;"));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
