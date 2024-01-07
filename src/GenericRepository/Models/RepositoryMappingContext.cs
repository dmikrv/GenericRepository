namespace GenericRepository.Models;

public class RepositoryMappingContext<TEntity> where TEntity : class
{
    public TEntity Source { get; set; } = null!;

    // You can override in child classes to add additional properties if needed
}