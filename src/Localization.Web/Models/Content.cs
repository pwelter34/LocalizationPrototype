using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Localization.Web.Models;

[Table("Content", Schema = "dbo")]
public class Content
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("CultureCode")]
    public string CultureCode { get; set; } = default!;

    [Column("LocalizeKey")]
    public string LocalizeKey { get; set; } = default!;

    [Column("LocalizeText")]
    public string? LocalizeText { get; set; }

    [Column("Created")]
    public DateTimeOffset Created { get; set; }

    [Column("CreatedBy")]
    public string? CreatedBy { get; set; }

    [Column("Updated")]
    public DateTimeOffset Updated { get; set; }

    [Column("UpdatedBy")]
    public string? UpdatedBy { get; set; }
}

