namespace LoanService;

public class RepayLoanRequest
{
    public int id { get; set; }
    public int subscriberId { get; set; }
    public int amount { get; set; }
}