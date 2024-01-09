using GenericRepository.Contracts;
using GenericRepository.Core.Contracts;
using GenericRepository.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GenericRepository.Extensions.ServiceProviders;

public static class ServiceProviderUnitOfWorkExtensions
{
    public static IServiceCollection AddUnitOfWork<TInterface, TImplementation>(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TInterface : class, IUnitOfWork where TImplementation : class, TInterface
    {
        services.Add(new ServiceDescriptor(typeof(IUnitOfWork), typeof(TImplementation), lifetime));
        services.Add(new ServiceDescriptor(typeof(TInterface), typeof(TImplementation), lifetime));
        return services;
    }

    public static IServiceCollection AddUnitOfWork<TImplementation>(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TImplementation : class, IUnitOfWork
    {
        services.Add(new ServiceDescriptor(typeof(IUnitOfWork), typeof(TImplementation), lifetime));
        return services;
    }

    public static IServiceCollection AddGenericRepositoryUnitOfWork<TDbContext, TUserPrimaryKey>(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext where TUserPrimaryKey : struct
    {
        services.Add(new ServiceDescriptor(typeof(IUnitOfWork),
            typeof(GenericRepositoryUnitOfWork<TDbContext, TUserPrimaryKey>), lifetime));
        return services;
    }

    public static IServiceCollection AddGenericRepositoryUnitOfWorkVal<TDbContext, TUserPrimaryKey>(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext where TUserPrimaryKey : struct
    {
        services.Add(new ServiceDescriptor(typeof(IUnitOfWork),
            typeof(GenericRepositoryUnitOfWorkVal<TDbContext, TUserPrimaryKey>), lifetime));
        return services;
    }

    public static IServiceCollection AddGenericRepositoryUnitOfWorkRef<TDbContext, TUserPrimaryKey>(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext where TUserPrimaryKey : class
    {
        services.Add(new ServiceDescriptor(typeof(IUnitOfWork),
            typeof(GenericRepositoryUnitOfWorkRef<TDbContext, TUserPrimaryKey>), lifetime));
        return services;
    }

    public static IServiceCollection AddEntityAuditService(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.Add(new ServiceDescriptor(typeof(IEntityAuditService), typeof(EntityAuditService), lifetime));
        return services;
    }
}