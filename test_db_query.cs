using System;
using Microsoft.EntityFrameworkCore;
using SongSpiration.Models.Entities;

class Program
{
    static void Main()
    {
        var optionsBuilder = new DbContextOptionsBuilder<SongSpirationDbContext>();
        optionsBuilder.UseSqlite("Data Source=SongSpiration.db");

        using var context = new SongSpirationDbContext(optionsBuilder.Options);

        var pins = context.Pins.ToList();

        Console.WriteLine("Pins in database:");
        foreach (var pin in pins)
        {
            Console.WriteLine($"ID: {pin.Id}");
            Console.WriteLine($"Title: {pin.Title}");
            Console.WriteLine($"Filename: {pin.Filename}");
            Console.WriteLine($"FilePath: {pin.FilePath}");
            Console.WriteLine("---");
        }
    }
}