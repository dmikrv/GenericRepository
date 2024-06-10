namespace GenericRepository.Core.Contracts.QueryParams.PropertyFilters;

public interface ISelectionPropertyFilter<TPrimaryKey>
{
    public IReadOnlyCollection<TPrimaryKey>? Selection { get; set; }
}