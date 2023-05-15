using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoanService.Model;
using Microsoft.AspNetCore.Mvc;

namespace LoanService.Services;

public interface ILoanService
{
    Task<long> ProvideLoan([FromQuery] ProvideLoanRequest request);
    Task<String> IsEligible([FromQuery] ProvideLoanRequest request);
    Task<List<LoanModel>> GetAllLoans();
    Task<LoanModel> GetLoan_ById([FromQuery] int id);
    Task<int> GetAmountById([FromQuery] int id);
    Task<long> UpdateAmount([FromQuery] RepayLoanRequest request);
    Task ArchiveLoan([FromQuery] int id);
}