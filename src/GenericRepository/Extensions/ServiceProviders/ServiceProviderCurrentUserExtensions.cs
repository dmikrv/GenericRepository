using GenericRepository.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace GenericRepository.Extensions.ServiceProviders;

public static class ServiceProviderCurrentUserExtensions
{
    public static IServiceCollection AddRepositoryCurrentUserProviderService<TImplementation>(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TImplementation : class, ICurrentUserIdProvider
    {
        services.Add(new(typeof(ICurrentUserIdProvider), typeof(TImplementation), lifetime));
        return services;
    }

    public static IServiceCollection AddRepositoryTenantProviderService<TImplementation>(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TImplementation : class, ITenantIdProvider
    {
        services.Add(new(typeof(ITenantIdProvider), typeof(TImplementation), lifetime));
        return services;
    }
}