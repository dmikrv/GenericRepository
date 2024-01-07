using System.Diagnostics.Contracts;
using GenericRepository.Core.Exceptions;

namespace GenericRepository.Core.Contracts;

/// <summary>
///     Used to produce exceptions.
/// </summary>
public interface IRepositoryExceptionFactory
{
    /// <summary>
    ///     Creates an exception from localized error message.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>Created exception.</returns>
    [Pure]
    NotFoundDomainException EntityNotFound<T>();

    /// <summary>
    ///     Creates an exception from localized error message.
    /// </summary>
    /// <param name="id">The entity id.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>Created exception.</returns>
    [Pure]
    NotFoundDomainException EntityNotFound<T>(object id);

    /// <summary>
    ///     Creates an exception from localized error message.
    /// </summary>
    /// <param name="localizedMessage">A localized exception message string.</param>
    /// <param name="localizedTitle">The localized exception title.</param>
    /// <returns>Created exception.</returns>
    [Pure]
    NotImplementedDomainException NotImplemented(string? localizedMessage = null, string? localizedTitle = null);
}