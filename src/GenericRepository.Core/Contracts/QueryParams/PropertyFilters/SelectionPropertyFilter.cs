namespace GenericRepository.Core.Contracts.QueryParams.PropertyFilters;

public class SelectionPropertyFilter<TPrimaryKey> : ISelectionPropertyFilter<TPrimaryKey>
{
    public IReadOnlyCollection<TPrimaryKey>? Selection { get; set; }
}