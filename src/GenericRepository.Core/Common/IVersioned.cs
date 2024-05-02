using System.ComponentModel.DataAnnotations.Schema;

namespace GenericRepository.Core.Common;

public interface IVersioned
{
    [NotMapped]
    public string Etag { get; set; }
}