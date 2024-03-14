namespace GenericRepository.Core.Contracts.QueryParams;

public interface IDeletedFilterQueryParams
{
    public bool? IsDeleted { get; set; }
}