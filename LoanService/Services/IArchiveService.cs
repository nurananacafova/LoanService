using LoanService.Model;
using Microsoft.AspNetCore.Mvc;

namespace LoanService.Services;

public interface IArchiveService
{
    Task<List<ArchiveModel>> GetArchiveAll();
    Task<ArchiveModel> GetArchiveById([FromQuery] int id);
}