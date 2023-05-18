using LoanService;
using LoanService.Model;

namespace LoanServiceUnit.Tests.Data;

public class LoanMockData
{
    public static List<LoanModel> AllLoans()
    {
        return new List<LoanModel>
        {
            new LoanModel()
            {
                id = 1,
                subscriberId = 5,
                amount = 234,
                loan_type = "Simple Loan"
            },
            new LoanModel()
            {
                id = 2,
                subscriberId = 6,
                amount = 1111,
                loan_type = "Simple Loan"
            }
        };
    }

    public static LoanModel LoanById()
    {
        return new LoanModel()
        {
            id = 1,
            subscriberId = 5,
            amount = 234,
            loan_type = "Simple Loan"
        };
    }

    public static ProvideLoanRequest IsEligible()
    {
        return new ProvideLoanRequest()
        {
            subscriberId = 1,
            Amount = 23,
            loan_type = "Simple Loan"
        };
    }

    public static List<LoanModel> GetEmptyLoan()
    {
        return new List<LoanModel>();
    }

    public static ProvideLoanRequest NewLoan()
    {
        return new ProvideLoanRequest()
        {
            // id = 1,
            subscriberId = 1,
            Amount = 234,
            loan_type = "Simple Loan"
        };
    }

    public static SubscriberModel subscriberModel()
    {
        return new SubscriberModel()
        {
            id = 1,
            language = "az",
            email = "newmailfortests1@gmail.com",
            registration_date = Convert.ToDateTime("2022-04-04")
        };
    }
}