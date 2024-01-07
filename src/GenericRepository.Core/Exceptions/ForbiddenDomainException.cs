using System.Net;
using GenericRepository.Core.Exceptions.Base;

namespace GenericRepository.Core.Exceptions;

public class ForbiddenDomainException : DomainException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ForbiddenDomainException" /> class.
    /// </summary>
    /// <param name="localizedMessage">The error message.</param>
    /// <param name="localizedTitle">The error title.</param>
    /// <param name="isUserFriendly">Indicating whether the data of object with this interface should be shown to end user.</param>
    public ForbiddenDomainException(string? localizedMessage, string localizedTitle, bool isUserFriendly = true)
        : base(localizedMessage, localizedTitle, HttpStatusCode.Forbidden, isUserFriendly)
    {
    }
}