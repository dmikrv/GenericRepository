namespace GenericRepository.Core.Contracts;

/// <summary>
///     A service for retrieving information about user who performs operation.
/// </summary>
public interface ICurrentUserIdProvider
{
    /// <summary>
    ///     Gets a current user.
    /// </summary>
    /// <param name="token">A cancellation token.</param>
    /// <returns>A current user.</returns>
    Task<TUserPrimaryKey> GetCurrentUserIdAsync<TUserPrimaryKey>(CancellationToken token = default);
}