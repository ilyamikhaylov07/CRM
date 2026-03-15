using System.ComponentModel.DataAnnotations.Schema;

namespace Crm.Domain.Common;

public abstract class BaseEntity
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("created_at_utc")]
    public DateTime CreatedAtUtc { get; set; }

    [Column("updated_at_utc")]
    public DateTime? UpdatedAtUtc { get; set; }
}
