using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoanService.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LoanService.Services;

public class ArchiveService : IArchiveService
{
    private readonly ILogger<ArchiveService> _logger;

    private readonly LoanDb _loandb;

    public ArchiveService(ILogger<ArchiveService> logger, LoanDb loandb)
    {
        _logger = logger;
        _loandb = loandb;
    }

    public async Task<List<ArchiveModel>> GetArchiveAll()
    {
        try
        {
            _logger.LogInformation("request received: {methodName}", "Get All Archive");

            List<ArchiveModel> archive = await _loandb.Archives.ToListAsync();
            foreach (var s in archive)
            {
                _logger.LogInformation(
                    "response replied: {message} {id} {loanId} {subscriberId} {amount} {loanType} {create_date} ",
                    "All archive successfully get:",
                    $"id:{s.id}", $"loan id:{s.loanId}", $"subscriber id:{s.subscriberId}", $"{s.amount}",
                    $"{s.loan_type}", $"{s.create_date}");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }

        return await _loandb.Archives.ToListAsync();
    }

    public async Task<ArchiveModel> GetArchiveById(int id)
    {
        ArchiveModel archiveModel;
        try
        {
            _logger.LogInformation("request received: {methodName} {id}", "Get Archive Row By Id:",
                $"subscriber id:{id}");

            archiveModel = await _loandb.Archives.FindAsync(id);

            _logger.LogInformation(
                "response replied: {message} {id} {loanId} {subscriberId} {amount} {loanType} {create_date}",
                "Archive row successfully get:",
                $"id:{archiveModel.id}", $"loan id:{archiveModel.loanId}",
                $"subscriber id:{archiveModel.subscriberId}", $"{archiveModel.amount}", $"{archiveModel.loan_type}",
                $"{archiveModel.create_date}");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }

        return (archiveModel);
    }
}