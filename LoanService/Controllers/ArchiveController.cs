using System.Net;
using LoanService.Model;
using LoanService.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoanService;

[Route("api/LoanService/[controller]")]
[ApiController]
public class ArchiveController : Controller
{
    private readonly ILogger<ArchiveController> _logger;

    private readonly LoanDb _loandb;
    private readonly IArchiveService _archiveService;

    public ArchiveController(ILogger<ArchiveController> logger, LoanDb loandb, IArchiveService archiveService)
    {
        _logger = logger;
        _loandb = loandb;
        _archiveService = archiveService;
    }

    [HttpGet("GetAllArchive")]
    // public async Task<ActionResult> GetArchiveAll() //bele de olar 
    public async Task<ActionResult<List<ArchiveModel>>> GetArchiveAll()
    {
        try
        {
            List<ArchiveModel> list = await _archiveService.GetArchiveAll();
            return new OkObjectResult(new ApiResponse<List<ArchiveModel>>((int)HttpStatusCode.OK, "Success", list));
        }
        catch (Exception e)
        {
            return new OkObjectResult(new ApiResponse<object>(500,
                $"Exception thrown via processing request. Method: {nameof(this.GetArchiveAll)}, Error message: {e.Message}"));
        }
    }

    [HttpGet("GetArchive")]
    public async Task<ActionResult<ArchiveModel>> GetArchiveById([FromQuery] int id)
    {
        try
        {
            ArchiveModel model = await _archiveService.GetArchiveById(id);
            return new OkObjectResult(new ApiResponse<ArchiveModel>((int)HttpStatusCode.OK, "Success", model));
        }
        catch (Exception e)
        {
            return new OkObjectResult(new ApiResponse<object>(500,
                $"Exception thrown via processing request. Method: {nameof(this.GetArchiveById)}, Error message: {e.Message}"));
        }
    }
}