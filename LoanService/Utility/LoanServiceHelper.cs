using System;
using System.Linq;
using System.Threading.Tasks;
using LoanService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LoanService;

public class LoanServiceHelper : ILoanServiceHelper
{
    private readonly LoanDb _loanDb;
    private IHttpClientHelper _helper;

    private readonly ILogger<LoanServiceHelper> _logger;

    public LoanServiceHelper(LoanDb loanDb, IHttpClientHelper helper, ILogger<LoanServiceHelper> logger)
    {
        _loanDb = loanDb;
        _helper = helper;
        _logger = logger;
    }


    public virtual async Task AddArchive(LoanModel loan)
    {
        string toMail = null;
        try
        {
            if (loan != null)
            {
                _loanDb.Archives.Add(new ArchiveModel()
                {
                    loanId = loan.id,
                    subscriberId = loan.subscriberId,
                    amount = loan.amount,
                    loan_type = loan.loan_type,
                    create_date = DateTime.Now
                });
                _loanDb.SaveChanges();
            }

            SubscriberModel subscriberModel = await _helper.GetSubscriber(loan.subscriberId);
            if (subscriberModel != null)
            {
                toMail = subscriberModel.email;
            }

            await SendResponse(toMail, $"Loan Repaid!: \n" +
                                       $"Loan ID: {loan.id};\n" +
                                       $"Subscriber ID: {loan.subscriberId};\n" +
                                       $"Amount: {loan.amount};\n" +
                                       $"Loan Type: {loan.loan_type};\n" +
                                       $"Date: {DateTime.Now};");
            _logger.LogInformation($"Response successfully send to {toMail}!");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    public virtual async Task<int> UpdateAmountById(LoanModel loanModel, int requestAmount)
    {
        int id = 0;
        try
        {
            _logger.LogInformation("request received: {methodName} {id} {amount}", "Update Amount:",
                $"loan id:{loanModel.id}", $"repay loan:{requestAmount}");
            int subscriberAmount = loanModel.amount;
            int amount;
            LoanModel loan = await (from l in _loanDb.Loans where l.id == loanModel.id select l).FirstOrDefaultAsync();
            if (loan != null)
            {
                amount = subscriberAmount - requestAmount;

                loan.amount = amount;

                _loanDb.SaveChanges();
                // loanModel = _loanDb.Loans.OrderBy(x => x.id).LastOrDefault();
                id = loanModel.id;
                _logger.LogInformation("response replied: {message} {loanId} {amount}",
                    "Amount updated:", $"loan id:{id}", $"amount:{loan.amount}");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }

        return id;
    }

    public virtual int CreateLoan(SubscriberModel subscriberModel, int requestAmount, string loanType)
    {
        Int32 id = 0;
        try
        {
            _logger.LogInformation("request received: {methodName} {id} {amount} {loanType}", "Create Loan:",
                $"subscriber id:{subscriberModel.id}", $"amount:{requestAmount}", $"loan type:{loanType}");
            LoanModel loanModel = new LoanModel();

            if (subscriberModel != null)
            {
                _loanDb.Loans.Add(new LoanModel()
                {
                    subscriberId = subscriberModel.id,
                    amount = requestAmount,
                    loan_type = loanType,
                });
                _loanDb.SaveChanges();

                loanModel = _loanDb.Loans.OrderBy(x => x.id).LastOrDefault();
                id = loanModel.id;
                _logger.LogInformation("response replied: {message} {loanId} {subscriberId} {amount} {loanType}",
                    "Loan created:", $"loan id:{id}", $"subscriber id:{subscriberModel.id}", $"amount:{requestAmount}",
                    $"loan type:{loanType}");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }

        return id;
    }

    public virtual async Task<ActionResult> SendResponse(string subscriberMail, string content)
    {
        try
        {
            var mailModel = new MailModel()
            {
                To = subscriberMail, From = "newmailfortests1@gmail.com", Password = "ngrtheilnrcfzroz",
                Content = content
            };

            await _helper.SendResponse(mailModel);

            // var result = postTask.Result;
            // if (result.IsSuccessStatusCode)
            // {
            //     var readTask = result.Content.ReadAsAsync<MailModel>();
            //     readTask.Wait();
            //
            //     var insertedStudent = readTask.Result;
            //
            //     Console.WriteLine("Mail SEND! to:{mail} content:{message}", insertedStudent.To,
            //         insertedStudent.Content);
            // }
            // else
            // {
            //     Console.WriteLine(result.StatusCode);
            // }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }

        return new StatusCodeResult(200);
    }
}