using GenericRepository.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace GenericRepository.Extensions.ServiceProviders;

public static class ServiceProviderCurrentUserExtensions
{
    public static IServiceCollection AddCurrentUserService<TInterface, TImplementation>(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TInterface : class, ICurrentUserIdProvider
        where TImplementation : class, TInterface
    {
        services.Add(new ServiceDescriptor(typeof(ICurrentUserIdProvider), typeof(TImplementation), lifetime));
        services.Add(new ServiceDescriptor(typeof(TInterface), typeof(TImplementation), lifetime));
        return services;
    }

    public static IServiceCollection AddRepositoryCurrentUserService<TImplementation>(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TImplementation : class, ICurrentUserIdProvider
    {
        services.Add(new ServiceDescriptor(typeof(ICurrentUserIdProvider), typeof(TImplementation), lifetime));
        return services;
    }
}