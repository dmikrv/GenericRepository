﻿namespace GenericRepository.Core.Contracts.QueryParams;

public interface IQueryParams : IPagedQueryParams, ISortedQueryParams, ISearchFilterQueryParams, ITypeaheadFilterQueryParams,
    IDeletedFilterQueryParams;

public interface IQueryParams<TPrimaryKey> : IQueryParams, IIdsFilterQueryParams<TPrimaryKey>
    where TPrimaryKey : IEquatable<TPrimaryKey>;

public interface IQueryParams<TPrimaryKey, TFilter> : IQueryParams<TPrimaryKey>, IFilterQueryParams<TFilter>
    where TPrimaryKey : IEquatable<TPrimaryKey>;