namespace GenericRepository.Core.Contracts.QueryParams;

public interface IFilterQueryParams<TFilter>
{
    public TFilter? Filters { get; set; }
}