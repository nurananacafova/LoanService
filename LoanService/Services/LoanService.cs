using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using LoanService.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace LoanService.Services;

public class LoanService : ILoanService
{
    private readonly LoanDb _loanDb;
    private IHttpClientHelper _helper;

    private ILoanServiceHelper _loanServiceHelper;

    private readonly ILogger<LoanService> _logger;

    public LoanService(ILogger<LoanService> logger, LoanDb loanDb, IHttpClientHelper helper,
        ILoanServiceHelper loanServiceHelper)
    {
        _logger = logger;
        _loanDb = loanDb;
        _loanServiceHelper = loanServiceHelper;
        _helper = helper;
    }

    public async Task<long> ProvideLoan(ProvideLoanRequest request)
    {
        int loanId = 0;
        try
        {
            SubscriberModel subscriberModel;
            string toMail;

            subscriberModel = await _helper.GetSubscriber(request.subscriberId);
            var scoringApiResponse = await _helper.GetScoringResponse(request);
            Boolean isSuitable = JsonConvert.DeserializeObject<Boolean>(scoringApiResponse);
            if (subscriberModel != null)
            {
                toMail = subscriberModel.email;
                if (isSuitable == true)
                {
                    loanId = _loanServiceHelper.CreateLoan(subscriberModel, request.Amount, request.loan_type);
                }
                else
                {
                    _logger.LogInformation("Subscriber is not eligible for loan.");
                    throw new InvalidOperationException("Subscriber not eligible.");
                }

                await _loanServiceHelper.SendResponse(toMail, $"New Loan Created! \n" +
                                                              $"ID: {loanId};\n" +
                                                              $"Subscriber ID: {request.subscriberId};\n" +
                                                              $"Amount: {request.Amount};");
                LoanMetrics.InsertCounter.Inc();
                _logger.LogInformation($"Response successfully send to {toMail}!");
            }
            else
            {
                _logger.LogInformation("Subscriber not found");

                throw new InvalidConstraintException("Subscriber not found");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }

        return loanId;
    }

    public async Task<string> IsEligible(ProvideLoanRequest request)
    {
        Boolean isEligible;
        string result;
        try
        {
            string scoringApiResponse = await _helper.GetScoringResponse(request);
            _logger.LogInformation($"Scoring api response: {scoringApiResponse}");
            isEligible = JsonConvert.DeserializeObject<Boolean>(scoringApiResponse);
            if (isEligible == true)
            {
                result = "Customer is eligible for take loan.";
            }
            else
            {
                result = "Customer is not eligible for take loan.";
            }

            _logger.LogInformation($"Customer is eligible for loan: {isEligible}");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }

        return result;
    }

    public async Task<List<LoanModel>> GetAllLoans()
    {
        List<LoanModel> loanModels = new List<LoanModel>();

        _logger.LogInformation("request received. {methodName}", "Get All Loans");
        try
        {
            if (_loanDb.Loans == null)
            {
                _logger.LogInformation("response replied: {message}", "Data not found");
                return null;
            }

            loanModels = await _loanDb.Loans.ToListAsync();
            LoanMetrics.GetCounts.Inc();
            LoanMetrics.RequestCountByMethod.WithLabels("GET").Inc();
            foreach (var l in loanModels)
            {
                _logger.LogInformation("response replied: {message} {id} {subscriberid} {amount} {loanType}",
                    "Loans successfully get:",
                    $"loan id:{l.id}", $"subscriber id:{l.subscriberId}", $"amount:{l.amount}",
                    $"loan type:{l.loan_type}");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }

        return loanModels;
    }

    public async Task<LoanModel> GetLoan_ById(int id)
    {
        LoanModel loan;
        try
        {
            _logger.LogInformation("request received: {methodName} {id}", "Get Loan By Id:",
                $"loan id:{id}");
            if (_loanDb.Loans == null)
            {
                _logger.LogInformation("response replied: {message}", "Data not found");
                return null;
            }

            loan = await _loanDb.Loans.FirstOrDefaultAsync(x => x.id == id);
            if (loan == null)
            {
                _logger.LogInformation("response replied: {message}", "Loan not found");
                return null;
            }

            LoanMetrics.GetByIdCount.Inc();
            _logger.LogInformation("response replied: {message} {id} {subscriberId} {amount} {loanType}",
                "Loan successfully get:",
                $"loan id:{loan.id}", $"subscriber id:{loan.subscriberId}", $"amount:{loan.amount}",
                $"loan type:{loan.loan_type}");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }


        return loan;
    }

    public async Task<int> GetAmountById(int id)
    {
        int amount;
        try
        {
            _logger.LogInformation("request received: {methodName} {id}", "Get Amount By Id:",
                $"loan id:{id}");
            if (_loanDb.Loans == null)
            {
                _logger.LogInformation("response replied: {message}", "Data not found");
                return 0;
            }

            var loan = await _loanDb.Loans.FirstOrDefaultAsync(x => x.id == id);

            if (loan == null)
            {
                _logger.LogInformation("response replied: {message}", "Amount not found");
                return 0;
            }

            amount = loan.amount;


            LoanMetrics.GetAmountCount.Inc();
            _logger.LogInformation("response replied: {message} {amount}", "Amount successfully get:",
                $"amount:{amount}");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }

        return amount;
    }

    public async Task<long> UpdateAmount(RepayLoanRequest request)
    {
        long loanId = 0;
        var loan = await _loanDb.Loans.FirstOrDefaultAsync(x => x.id == request.id);

        if (loan == null) throw new InvalidOperationException($"Active loan not found. LoanId: {request.id}");

        int amount = loan.amount;
        if (request.amount == 0)
        {
            throw new InvalidConstraintException("Amount not found");
        }

        if (amount != 0)
        {
            loanId = await _loanServiceHelper.UpdateAmountById(loan, request.amount);
            LoanMetrics.UpdateCounter.Inc();
            return loanId;
        }

        return loanId;
    }

    private bool LoanAvailable(int id)
    {
        return (_loanDb.Loans?.Any(x => x.id == id)).GetValueOrDefault();
    }

    public async Task ArchiveLoan(int id)
    {
        try
        {
            _logger.LogInformation("request received: {methodName} {id}", "Delete Loan:",
                $"loan id:{id}");

            var loan = await _loanDb.Loans.FirstOrDefaultAsync(x => x.id == id);
            if (loan == null)
            {
                _logger.LogInformation("response replied: {message}", "Data not found");
                throw new InvalidOperationException("Loan not found");
            }

            await _loanServiceHelper.AddArchive(loan);
            _loanDb.Loans.Remove(loan);
            await _loanDb.SaveChangesAsync();
            LoanMetrics.DeleteCounter.Inc();
            _logger.LogInformation("response replied: {message}", $"Loan {id} deleted");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }
}