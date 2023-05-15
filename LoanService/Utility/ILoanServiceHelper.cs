using System.Threading.Tasks;
using LoanService.Model;
using Microsoft.AspNetCore.Mvc;

namespace LoanService;

public interface ILoanServiceHelper
{
    Task AddArchive(LoanModel loan);
    Task<int> UpdateAmountById(LoanModel loanModel, int requestAmount);
    int CreateLoan(SubscriberModel subscriberModel, int requestAmount, string loanType);
    Task<ActionResult> SendResponse(string subscriberMail, string content);
}