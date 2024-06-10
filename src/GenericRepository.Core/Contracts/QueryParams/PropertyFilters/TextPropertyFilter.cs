using GenericRepository.Core.Models.Filters;

namespace GenericRepository.Core.Contracts.QueryParams.PropertyFilters;

public class TextPropertyFilter : ITextPropertyFilter
{
    public StringPropertyFilter? Text { get; set; }
}