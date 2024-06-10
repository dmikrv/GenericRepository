using GenericRepository.Core.Models.Filters;

namespace GenericRepository.Core.Contracts.QueryParams.PropertyFilters;

public interface ITextPropertyFilter
{
    public StringPropertyFilter? Text { get; set; }
}