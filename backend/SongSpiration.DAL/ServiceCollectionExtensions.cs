using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SongSpiration.DAL.Interfaces;
using SongSpiration.DAL.Repositories;

namespace SongSpiration.DAL;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers DbContext and DAL repositories into the DI container.
    /// Pass a configuration action for DbContextOptionsBuilder when calling from startup.
    /// Example:
    /// services.AddSongSpirationDal(options => options.UseSqlServer(connectionString));
    /// </summary>
    public static IServiceCollection AddSongSpirationDal(this IServiceCollection services, Action<DbContextOptionsBuilder>? configureDb = null)
    {
        if (configureDb is null)
        {
            // Register DbContext with default options (caller should override or replace in real app)
            services.AddDbContext<SongSpirationDbContext>();
        }
        else
        {
            services.AddDbContext<SongSpirationDbContext>(configureDb);
        }

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IPinRepository, PinRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}