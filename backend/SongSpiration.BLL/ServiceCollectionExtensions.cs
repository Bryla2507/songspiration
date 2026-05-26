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
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IRankingService, RankingService>();

        // Add other services as needed
        // services.AddScoped<ICollectionService, CollectionService>();
        // services.AddScoped<ILikeService, LikeService>();
        // services.AddScoped<IFilterService, FilterService>();

        return services;
    }
}