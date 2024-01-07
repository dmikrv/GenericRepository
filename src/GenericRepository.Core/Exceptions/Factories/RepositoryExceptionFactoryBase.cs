using System.Diagnostics.Contracts;
using GenericRepository.Core.Contracts;

namespace GenericRepository.Core.Exceptions.Factories;

/// <summary>
///     Used to produce exceptions.
/// </summary>
public class RepositoryExceptionFactoryBase : IRepositoryExceptionFactory
{
    /// <inheritdoc />
    [Pure]
    public NotFoundDomainException EntityNotFound<T>()
    {
        var message = $"{typeof(T).Name} was not found.";
        return new NotFoundDomainException(message, "Not Found");
    }

    /// <inheritdoc />
    [Pure]
    public NotFoundDomainException EntityNotFound<T>(object id)
    {
        var message = $"{typeof(T).Name} with id {id} was not found.";
        return new NotFoundDomainException(message, "Not Found");
    }

    /// <inheritdoc />
    [Pure]
    public NotImplementedDomainException NotImplemented(string? localizedMessage = null, string? localizedTitle = null)
    {
        localizedTitle ??= "Not implemented";
        return new NotImplementedDomainException(localizedMessage, localizedTitle);
    }
}