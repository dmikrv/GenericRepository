using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GenericRepository.Core.Common.Auditable.Versioned;

public interface IVersionedEntity : IVersioned
{
    [JsonIgnore] [Timestamp] public byte[] RowVersion { get; set; }
}