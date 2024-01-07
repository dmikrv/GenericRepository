namespace GenericRepository.Core.Contracts.Repositories;

/// <summary>
///     An interface for repositories.
/// </summary>
/// <typeparam name="TEntity">The type of entities to work with.</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    public Task<bool> IsAggregateModifiedAsync(TEntity entity, CancellationToken token = default);

    /// <summary>
    ///     Creates an entity <paramref name="entity" /> in the change tracker.
    /// </summary>
    /// <param name="entity">An entity to be created.</param>
    /// <param name="token">A cancellation token.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken token = default);

    /// <summary>
    ///     Updates an entity <paramref name="entity" /> in the change tracker.
    /// </summary>
    /// <param name="entity">An entity to be updated.</param>
    /// <param name="token">A cancellation token.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken token = default);

    /// <summary>
    ///     Deletes an entity <paramref name="entity" /> in the change tracker.
    /// </summary>
    /// <param name="entity">An entity to be deleted.</param>
    /// <param name="token">A cancellation token.</param>
    /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
    Task DeleteAsync(TEntity entity, CancellationToken token = default);
}