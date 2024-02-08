using GenericRepository.Core.Contracts.QueryParams;
using GenericRepository.Core.Enums;

namespace GenericRepository.Core.Models;

public class QueryParams<TPrimaryKey> : IQueryParams<TPrimaryKey>
    where TPrimaryKey : IEquatable<TPrimaryKey>
{
    private int _pageNumber = IPagedQueryParams.DefaultPageNumber;
    private int _pageSize = IPagedQueryParams.DefaultPageSize;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1
            ? IPagedQueryParams.DefaultPageNumber
            : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1
            ? IPagedQueryParams.DefaultPageSize
            : value;
    }

    public string? SortBy { get; set; }

    public SortingDirection? SortDirection { get; set; }

    public string? Search { get; set; }

    public string? Typeahead { get; set; }

    public TPrimaryKey[]? Ids { get; set; }

    public bool? InvertIds { get; set; }
}

public class QueryParams<TPrimaryKey, TFilter> : QueryParams<TPrimaryKey>, IQueryParams<TPrimaryKey, TFilter>
    where TPrimaryKey : IEquatable<TPrimaryKey>
{
    public TFilter? Filters { get; set; }
}