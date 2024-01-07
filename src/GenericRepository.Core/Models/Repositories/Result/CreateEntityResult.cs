using GenericRepository.Core.Common;
using GenericRepository.Core.Contracts.Repositories;

namespace GenericRepository.Core.Models.Repositories.Result;

/// <summary>
///     Represents a result of create operation of <see cref="IEntityRepository{TEntity,TPrimaryKey}" />.
/// </summary>
/// <param name="Entity">The created entity.</param>
/// w
/// <param name="ValueAccessor">The accessor for created entity primary key.</param>
/// <typeparam name="TEntity">Type of the entity.</typeparam>
/// <typeparam name="TPrimaryKey">A type of entity's primary key.</typeparam>
public record CreateEntityResult<TEntity, TPrimaryKey>(TEntity Entity, ValueAccessor<TPrimaryKey> ValueAccessor);