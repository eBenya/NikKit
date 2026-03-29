using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NikKit.Outbox.Abstraction;
using NikKit.Outbox.Hosting.EFCore.Repository;

namespace NikKit.Outbox.Hosting.EFCore;

public static class OutboxServiceCollectionExtensions
{
    /// <summary>
    /// Event saver injection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="conf"></param>
    /// <typeparam name="TDbContext"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddEventSaver<TDbContext>(this IServiceCollection services, IConfiguration conf)
        where TDbContext : DbContext
    {
        services.Configure<OutboxOptions>(opt => conf.GetSection(OutboxOptions.SectionName).Bind(opt));

        services.AddScoped<IEventSaver, OutboxEventSaver>(sp =>
            new OutboxEventSaver(sp.GetRequiredService<TDbContext>()));

        return services;
    }

    /// <summary>
    /// Custom event saver injection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="conf"></param>
    /// <typeparam name="TEventSaverRepo">Custom saver version</typeparam>
    /// <returns></returns>
    public static IServiceCollection AddCustomEventSaver<TEventSaverRepo>(this IServiceCollection services, IConfiguration conf)
        where TEventSaverRepo : class, IEventSaver
    {
        services.Configure<OutboxOptions>(opt => conf.GetSection(OutboxOptions.SectionName).Bind(opt));
        services.AddScoped<IEventSaver, TEventSaverRepo>();

        return services;
    }

    /// <summary>
    /// OutBox worker registration.
    /// With default repo realization.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <typeparam name="TDbContext"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddOutboxJob<TDbContext>(this IServiceCollection services, IConfiguration configuration)
        where TDbContext : DbContext
    {
        var options = configuration.GetSection(OutboxOptions.SectionName).Get<OutboxOptions>() ?? new OutboxOptions();
        //services.AddOutboxJob<OutboxRepository>(configuration);
        services.AddScoped<IOutboxRepository, OutboxRepository>(sp =>
            new OutboxRepository(sp.GetRequiredService<TDbContext>(), Options.Create(options)));
        return services;
    }
}