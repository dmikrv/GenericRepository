namespace GenericRepository.Core.Common;

public interface IVersioned
{
    public string Etag { get; set; }
}