using LoanService.Model;
using LoanService.Services;
using Microsoft.AspNetCore.Mvc;


namespace LoanService;

[Route("api/[controller]")]
[ApiController]
public class LoanServiceController : Controller
{
    private readonly ILoanService _loanService;

    public LoanServiceController(ILoanService loanService)
    {
        _loanService = loanService;
    }


    [HttpPost("ProvideLoan")]
    public async Task<IActionResult> ProvideLoan([FromBody] ProvideLoanRequest request)
    {
        try
        {
            long loanId = await _loanService.ProvideLoan(request);
            if (loanId == 0)
            {
                return Ok(null);
            }

            return Ok(loanId);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpGet("EligibilityCheck")]
    public async Task<IActionResult> IsEligible([FromQuery] ProvideLoanRequest request)
    {
        try
        {
            string result = await _loanService.IsEligible(request);
            if (result == null)
            {
                return Ok(null);
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            return new OkObjectResult(new ApiResponse<object>(500,
                $"Exception thrown via processing request. Method: {nameof(this.IsEligible)}, Error message: {e.Message}"));
        }
    }


    [HttpGet("GetAllLoans")]
    public async Task<ActionResult<LoanModel>> GetAll_Loans()
    {
        List<LoanModel> loanModels;
        try
        {
            loanModels = await _loanService.GetAllLoans();

            if (loanModels.Count == 0)
            {
                return Ok(null);
            }

            return Ok(loanModels);
        }
        catch (Exception e)
        {
            return new OkObjectResult(new ApiResponse<object>(500,
                $"Exception thrown via processing request. Method: {nameof(this.GetAll_Loans)}, Error message: {e.Message}"));
        }
    }

    [HttpGet("GetLoan")]
    public async Task<ActionResult<LoanModel>> GetLoanById([FromQuery] int id)
    {
        try
        {
            LoanModel loanModels = await _loanService.GetLoan_ById(id);

            if (loanModels == null)
            {
                return Ok(null);
            }

            return Ok(loanModels);
        }
        catch (Exception e)
        {
            return new OkObjectResult(new ApiResponse<object>(500,
                $"Exception thrown via processing request. Method: {nameof(this.GetLoanById)}, Error message: {e.Message}"));
        }
    }

    [HttpGet("GetLoan/amount")]
    public async Task<ActionResult<int>> GetAmountById([FromQuery] int id)
    {
        try
        {
            int amount = await _loanService.GetAmountById(id);
            if (amount == 0)
            {
                return Ok(null);
            }

            return Ok(amount);
        }
        catch (Exception e)
        {
            return new OkObjectResult(new ApiResponse<object>(500,
                $"Exception thrown via processing request. Method: {nameof(this.GetAmountById)}, Error message: {e.Message}"));
        }
    }

    [HttpPut("RepayLoan")]
    public async Task<IActionResult> UpdateAmount([FromBody] RepayLoanRequest request)
    {
        try
        {
            long
                result = await _loanService.UpdateAmount(request);
            if (result == 0)
            {
                return Ok(null);
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("ArchiveLoan")]
    public async Task<IActionResult> ArchiveLoan([FromQuery] int id)
    {
        try
        {
            if (id == 0)
            {
                return Ok(null);
            }

            await _loanService.ArchiveLoan(id);
            return Ok(Task.CompletedTask);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
}