namespace LoanService;

public class ProvideLoanRequest
{
    public int id { get; set; }
    public int subscriberId { get; set; }
    public int Amount { get; set; }
    public string loan_type { get; set; }
}