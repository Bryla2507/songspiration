using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SongSpiration.BLL;
using SongSpiration.BLL.DTOs;
using SongSpiration.DAL;
using SongSpiration.Models;

namespace TestImplementation;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Testing SongSpiration implementation...");

        // Set up dependency injection
        var services = new ServiceCollection();

        // Add DAL with in-memory database for testing
        services.AddSongSpirationDal(options =>
            options.UseInMemoryDatabase("SongSpirationTestDb"));

        // Add BLL services
        services.AddSongSpirationBll();

        var serviceProvider = services.BuildServiceProvider();

        try
        {
            // Test User Service
            Console.WriteLine("\n=== Testing User Service ===");
            var userService = serviceProvider.GetRequiredService<IUserService>();

            // Test registration
            var registerDto = new RegisterUserDto
            {
                Email = "test@example.com",
                DisplayName = "Test User",
                Password = "password123"
            };

            var authResponse = await userService.RegisterAsync(registerDto);
            Console.WriteLine($"Registered user: {authResponse.User.DisplayName} (ID: {authResponse.User.Id})");
            Console.WriteLine($"Access Token: {authResponse.AccessToken}");

            // Test login
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "password123"
            };

            var loginResponse = await userService.LoginAsync(loginDto);
            Console.WriteLine($"Logged in user: {loginResponse.User.DisplayName}");

            // Test get profile
            var profile = await userService.GetProfileAsync(loginResponse.User.Id);
            Console.WriteLine($"Profile: {profile?.DisplayName} - {profile?.Email}");

            // Test Pin Service
            Console.WriteLine("\n=== Testing Pin Service ===");
            var pinService = serviceProvider.GetRequiredService<IPinService>();

            // Test create pin
            var createPinDto = new CreatePinDto
            {
                Title = "My First Pin",
                Description = "This is a test pin",
                Visibility = PinVisibility.Public
            };

            var createdPin = await pinService.CreatePinAsync(loginResponse.User.Id, createPinDto);
            Console.WriteLine($"Created pin: {createdPin.Title} (ID: {createdPin.Id})");

            // Test get pin by ID
            var retrievedPin = await pinService.GetPinByIdAsync(createdPin.Id);
            Console.WriteLine($"Retrieved pin: {retrievedPin?.Title}");

            // Test get all pins
            var allPins = await pinService.GetAllPinsAsync();
            Console.WriteLine($"Total pins: {allPins.Count()}");

            // Test get pins by user ID
            var userPins = await pinService.GetPinsByUserIdAsync(loginResponse.User.Id);
            Console.WriteLine($"User pins: {userPins.Count()}");

            // Test update pin
            var updateDto = new UpdatePinDto
            {
                Title = "Updated Pin Title",
                Description = "Updated description"
            };

            var updatedPin = await pinService.UpdatePinAsync(createdPin.Id, updateDto);
            Console.WriteLine($"Updated pin: {updatedPin.Title}");

            // Test delete pin
            var deleteResult = await pinService.DeletePinAsync(createdPin.Id);
            Console.WriteLine($"Delete successful: {deleteResult}");

            Console.WriteLine("\n=== All tests completed successfully! ===");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}