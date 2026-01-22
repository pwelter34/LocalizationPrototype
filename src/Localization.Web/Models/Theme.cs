using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Localization.Web.Models;

[Table("Theme", Schema = "dbo")]
public class Theme
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("Name")]
    public string Name { get; set; } = default!;

    [Column("Description")]
    public string? Description { get; set; }

    [Column("DomainName")]
    public string DomainName { get; set; } = default!;

    [Column("SiteName")]
    public string? SiteName { get; set; }

    [Column("SiteLogo")]
    public string? SiteLogo { get; set; }

    [Column("SiteStyle")]
    public string? SiteStyle { get; set; }

    [Column("LegalName")]
    public string? LegalName { get; set; }

    [Column("SupportPhoneNumber")]
    public string? SupportPhoneNumber { get; set; }

    [Column("SupportEmail")]
    public string? SupportEmail { get; set; }

    [Column("Created")]
    public DateTimeOffset Created { get; set; }

    [Column("CreatedBy")]
    public string? CreatedBy { get; set; }

    [Column("Updated")]
    public DateTimeOffset Updated { get; set; }

    [Column("UpdatedBy")]
    public string? UpdatedBy { get; set; }
}