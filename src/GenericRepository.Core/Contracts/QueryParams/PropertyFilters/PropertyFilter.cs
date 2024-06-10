using GenericRepository.Core.Models.Filters;

namespace GenericRepository.Core.Contracts.QueryParams.PropertyFilters;

public class PropertyFilter<TPrimaryKey> : ISelectionPropertyFilter<TPrimaryKey>, ITextPropertyFilter
{
    public IReadOnlyCollection<TPrimaryKey>? Selection { get; set; }
    public StringPropertyFilter? Text { get; set; }
}