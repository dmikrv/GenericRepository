using GenericRepository.Core.Contracts;
using GenericRepository.Core.Exceptions.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace GenericRepository.Extensions.ServiceProviders;

public static class ServiceProviderExceptionFactoryExtensions
{
    public static IServiceCollection AddRepositoryExceptionFactory<TImplementation>(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TImplementation : class, IRepositoryExceptionFactory
    {
        services.Add(new ServiceDescriptor(typeof(IRepositoryExceptionFactory), typeof(TImplementation), lifetime));
        return services;
    }

    public static IServiceCollection AddRepositoryExceptionFactory(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        services.Add(new ServiceDescriptor(typeof(IRepositoryExceptionFactory), typeof(RepositoryExceptionFactoryBase), lifetime));
        return services;
    }
}