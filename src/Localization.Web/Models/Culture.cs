using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Localization.Web.Models;

[Table("Culture", Schema = "dbo")]
public class Culture
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("CultureCode")]
    public string CultureCode { get; set; } = default!;

    [Column("DisplayName")]
    public string DisplayName { get; set; } = default!;

    [Column("NativeName")]
    public string NativeName { get; set; } = default!;

    [Column("FlagClass")]
    public string? FlagClass { get; set; }

    [Column("IsActive")]
    public bool IsActive { get; set; }

    [Column("IsDefault")]
    public bool IsDefault { get; set; }

    [Column("DisplayOrder")]
    public int DisplayOrder { get; set; }

    [Column("Created")]
    public DateTimeOffset Created { get; set; }

    [Column("CreatedBy")]
    public string? CreatedBy { get; set; }

    [Column("Updated")]
    public DateTimeOffset Updated { get; set; }

    [Column("UpdatedBy")]
    public string? UpdatedBy { get; set; }
}
