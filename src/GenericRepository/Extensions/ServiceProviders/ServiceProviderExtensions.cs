using GenericRepository.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GenericRepository.Extensions.ServiceProviders;

public static class ServiceProviderExtensions
{
    public static IServiceCollection AddGenericRepositoryCommonDependencies(this IServiceCollection services)
    {
        return services.AddTransient<RepositoryCommonDependencies>();
    }

    public static IServiceCollection AddServicesFromAssemblyContaining<TMarkerInterface, TWhereInterfaces, TWhereImplementations>(
        this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        // Get the assembly where the interfaces are defined
        var interfaceAssembly = typeof(TWhereInterfaces).Assembly;

        // Get the assembly where the interfaces are defined
        var implementationAssembly = typeof(TWhereImplementations).Assembly;

        // Find all interfaces that extend IEntityRepository
        var entityInterfaces = interfaceAssembly
            .GetTypes()
            .Where(type => type.IsInterface && typeof(TMarkerInterface).IsAssignableFrom(type));

        // Iterate through each interface and find its corresponding implementation
        foreach (var entityInterface in entityInterfaces)
        {
            var implementationType = implementationAssembly
                .GetTypes()
                .FirstOrDefault(type =>
                    type.IsClass &&
                    !type.IsAbstract &&
                    entityInterface.IsAssignableFrom(type) &&
                    type.GetInterfaces().Contains(entityInterface));

            // Register the interface and implementation with the specified lifetime
            if (implementationType is not null)
                services.TryAdd(new ServiceDescriptor(entityInterface, implementationType, lifetime));
            else
                throw new NullReferenceException($"Not found repository implementation for {entityInterface.Name}");
        }

        return services;
    }
}