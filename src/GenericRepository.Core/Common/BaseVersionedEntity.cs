using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GenericRepository.Core.Common;

public abstract class BaseVersionedEntity<TPrimaryKey> : BaseEntity<TPrimaryKey>, IVersioned
    where TPrimaryKey : IEquatable<TPrimaryKey>
{
    [JsonIgnore] [Timestamp] protected virtual byte[] RowVersion { get; set; } = null!;

    public string Etag
    {
        get => Convert.ToBase64String(RowVersion);
        set => RowVersion = Convert.FromBase64String(value);
    }
}