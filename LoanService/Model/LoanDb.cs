using LoanService.Model;
using Microsoft.EntityFrameworkCore;

namespace LoanService;

public class LoanDb : DbContext
{
    public LoanDb()
        : this(new DbContextOptions<LoanDb>())
    {
    }

    public LoanDb(DbContextOptions<LoanDb> options)
        : base(options)
    {
    }


    public virtual DbSet<LoanModel> Loans { get; set; }
    public virtual DbSet<ArchiveModel> Archives { get; set; }
}