using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GenericRepository.Core.Common;

public interface IVersioned
{
    [JsonIgnore] [Timestamp] public byte[] RowVersion { get; set; }
    
    [NotMapped]
    public string Etag { get; set; }
}