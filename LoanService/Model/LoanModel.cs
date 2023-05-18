using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanService.Model;

[Table("loanTable", Schema = "dbo")]
public class LoanModel
{
    
     public int id { get; set; }
    [Required] public int subscriberId { get; set; }

    [Required] public int amount { get; set; }
    [Required] public string loan_type { get; set; }
}