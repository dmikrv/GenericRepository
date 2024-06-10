using System.ComponentModel.DataAnnotations.Schema;

namespace GenericRepository.Core.Common.Auditable.Versioned;

public interface IVersioned
{
    [NotMapped] public string Etag { get; set; }
}