using System.Threading.Tasks;
using LoanService.Model;

namespace LoanService;

public interface IHttpClientHelper
{
    Task<SubscriberModel> GetSubscriber(int id);
    Task<string> GetScoringResponse(ProvideLoanRequest request);

    Task SendResponse(MailModel mailModel);
}