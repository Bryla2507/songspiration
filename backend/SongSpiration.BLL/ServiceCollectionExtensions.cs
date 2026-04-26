using Microsoft.Extensions.DependencyInjection;
using SongSpiration.BLL.Interfaces;
using SongSpiration.BLL.Services;

namespace SongSpiration.BLL;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers BLL services into the DI container.
    /// </summary>
    public static IServiceCollection AddSongSpirationBll(this IServiceCollection services)
    {
        services.AddScoped<IPinService, PinService>();
        services.AddScoped<IUserService, UserService>();

        // Add other services as needed
        // services.AddScoped<ICollectionService, CollectionService>();
        // services.AddScoped<ILikeService, LikeService>();
        // services.AddScoped<IFilterService, FilterService>();

        return services;
    }
}