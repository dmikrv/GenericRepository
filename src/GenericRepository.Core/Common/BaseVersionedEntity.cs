using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GenericRepository.Core.Common;

public abstract class BaseVersionedEntity<TPrimaryKey> : BaseEntity<TPrimaryKey>, IVersionedEntity
    where TPrimaryKey : IEquatable<TPrimaryKey>
{
    [JsonIgnore] [Timestamp] public virtual byte[] RowVersion { get; set; } = null!;

    [NotMapped]
    public string Etag
    {
        get => Convert.ToBase64String(RowVersion);
        set
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (value != null) RowVersion = Convert.FromBase64String(value);
        }
    }
}