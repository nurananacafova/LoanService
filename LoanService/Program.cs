using LoanService;
using LoanService.Services;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<LoanDb>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddTransient<ILoanService, LoanService.Services.LoanService>();
    builder.Services.AddTransient<IArchiveService, ArchiveService>();

    builder.Services.AddScoped<IHttpClientHelper, HttpClientHelper>();
    builder.Services.AddScoped<ILoanServiceHelper, LoanServiceHelper>();


    var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
    NLog.GlobalDiagnosticsContext.Set("LogDirectory", logPath);
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();
    app.UseHttpsRedirection();

    builder.WebHost.UseUrls("http://*:5002");
    if (app.Environment.IsDevelopment()||app.Environment.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }


    app.UseAuthorization();
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex);
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}

public partial class Program
{
}