using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace LoanService.Model;

[Table("loanArchive", Schema = "dbo")]
public class ArchiveModel
{
    [Key] public int id { get; set; }
    [Required] public int loanId { get; set; }
    [Required] public int subscriberId { get; set; }
    [Required] public int amount { get; set; }
    [Required] public string loan_type { get; set; }

    [Required] public DateTime create_date { get; set; }
}