using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using LoanService;
using LoanService.Model;
using LoanServiceUnit.Tests.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;

namespace LoanServiceUnit.Tests.System;

public class TestLoanService
{
    public readonly DbContextOptions<LoanDb> dbContextOptions;

    public TestLoanService()
    {
        dbContextOptions = new DbContextOptionsBuilder<LoanDb>()
            .UseInMemoryDatabase(databaseName: "test")
            .Options;
    }

    [Fact]
    public async Task GetAllLoans_LoansExist_ReturnsOk()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockHttpClientHelper = fixture.Create<Mock<HttpClientHelper>>();
        var loggerMock = new Mock<ILogger<LoanService.Services.LoanService>>();
        var mockLoanServiceHelper = fixture.Create<Mock<LoanServiceHelper>>();
        var dbMock = new Mock<LoanDb>();
        dbMock.Setup<DbSet<LoanModel>>(x => x.Loans).ReturnsDbSet(LoanMockData.AllLoans());
        var sut = new LoanService.Services.LoanService(loggerMock.Object, dbMock.Object, mockHttpClientHelper.Object,
            mockLoanServiceHelper.Object);
        var result = await sut.GetAllLoans();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(234, result.ElementAt(0).amount);
    }

    [Fact]
    public async Task GetAllLoans_LoansDoNotExistReturnsNull()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockHttpClientHelper = fixture.Create<Mock<HttpClientHelper>>();
        var loggerMock = new Mock<ILogger<LoanService.Services.LoanService>>();
        var mockLoanServiceHelper = fixture.Create<Mock<LoanServiceHelper>>();
        var dbMock = new Mock<LoanDb>();
        dbMock.Setup<DbSet<LoanModel>>(x => x.Loans).ReturnsDbSet(LoanMockData.GetEmptyLoan());
        //new List<LoanModel>() { new LoanModel()}
        var sut = new LoanService.Services.LoanService(loggerMock.Object, dbMock.Object, mockHttpClientHelper.Object,
            mockLoanServiceHelper.Object);
        var result = await sut.GetAllLoans();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLoanById_LoanExists_ReturnsOk()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockHttpClientHelper = fixture.Create<Mock<HttpClientHelper>>();
        var loggerMock = new Mock<ILogger<LoanService.Services.LoanService>>();
        var mockLoanServiceHelper = fixture.Create<Mock<LoanServiceHelper>>();
        var dbMock = new Mock<LoanDb>();
        int id = 1;

        dbMock.Setup<DbSet<LoanModel>>(x => x.Loans).ReturnsDbSet(new[] { LoanMockData.LoanById() });
        var sut = new LoanService.Services.LoanService(loggerMock.Object, dbMock.Object, mockHttpClientHelper.Object,
            mockLoanServiceHelper.Object);
        var result = await sut.GetLoan_ById(id);
        Assert.NotEmpty(new[] { result });
        Assert.Equal(1, result.id);
        Assert.Equal(5, result.subscriberId);
        Assert.Equal(234, result.amount);
    }


    [Fact]
    public async Task GetLoanById_LoanDoesNotExist_ReturnsNull()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockHttpClientHelper = fixture.Create<Mock<HttpClientHelper>>();
        var loggerMock = new Mock<ILogger<LoanService.Services.LoanService>>();
        var mockLoanServiceHelper = fixture.Create<Mock<LoanServiceHelper>>();
        var dbMock = new Mock<LoanDb>();
        int id = 2;

        dbMock.Setup<DbSet<LoanModel>>(x => x.Loans).ReturnsDbSet(new[] { new LoanModel() { } });
        var sut = new LoanService.Services.LoanService(loggerMock.Object, dbMock.Object, mockHttpClientHelper.Object,
            mockLoanServiceHelper.Object);
        var result = await sut.GetLoan_ById(id);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAmountById_LoanExists_ReturnsOk()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockHttpClientHelper = fixture.Create<Mock<HttpClientHelper>>();
        var loggerMock = new Mock<ILogger<LoanService.Services.LoanService>>();
        var mockLoanServiceHelper = fixture.Create<Mock<LoanServiceHelper>>();
        var dbMock = new Mock<LoanDb>();
        int id = 1;

        dbMock.Setup(x => x.Loans).ReturnsDbSet(LoanMockData.AllLoans());

        var sut = new LoanService.Services.LoanService(loggerMock.Object, dbMock.Object, mockHttpClientHelper.Object,
            mockLoanServiceHelper.Object);

        var result = await sut.GetAmountById(id);
        Assert.Equal(234, result);
    }

    [Fact]
    public async Task GetAmountById_LoanDoesNotExist_ReturnsNull()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockHttpClientHelper = fixture.Create<Mock<HttpClientHelper>>();
        var loggerMock = new Mock<ILogger<LoanService.Services.LoanService>>();
        var mockLoanServiceHelper = fixture.Create<Mock<LoanServiceHelper>>();
        var dbMock = new Mock<LoanDb>();
        int id = 1;

        dbMock.Setup(x => x.Loans).ReturnsDbSet(new[] { new LoanModel() { } });

        var sut = new LoanService.Services.LoanService(loggerMock.Object, dbMock.Object, mockHttpClientHelper.Object,
            mockLoanServiceHelper.Object);

        var result = await sut.GetAmountById(id);
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task ProvideLoan_ReturnsOK()
    {
        var subscriber = new SubscriberModel()
        {
            id = 1,
            language = "az",
            email = "newmailfortests1@gmail.com",
            registration_date = Convert.ToDateTime("2022-04-04")
        };
        var newLoan = new ProvideLoanRequest()
        {
            id = 1,
            subscriberId = 1,
            Amount = 234,
            loan_type = "Simple Loan"
        };
        string mail = "newmailfortests1@gmail.com";
        string content = "Loan created";

        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockHttpClientHelper = fixture.Create<Mock<HttpClientHelper>>();
        var loggerMock = new Mock<ILogger<LoanService.Services.LoanService>>();
        var mockLoanServiceHelper = fixture.Create<Mock<LoanServiceHelper>>();
        var dbMock = fixture.Create<Mock<LoanDb>>();


        mockLoanServiceHelper
            .Setup(x => x.CreateLoan(subscriber, newLoan.Amount, newLoan.loan_type))
            .Returns(newLoan.id);
        mockHttpClientHelper.Setup(x => x.GetScoringResponse(newLoan)).ReturnsAsync("true");
        mockHttpClientHelper.Setup(x => x.GetSubscriber(newLoan.subscriberId)).ReturnsAsync(subscriber);
        mockLoanServiceHelper.Setup(x => x.SendResponse(mail, content)).ReturnsAsync(new StatusCodeResult(200));

        var sut = new LoanService.Services.LoanService(loggerMock.Object, dbMock.Object, mockHttpClientHelper.Object,
            mockLoanServiceHelper.Object);

        var result = await sut.ProvideLoan(newLoan);

        Assert.Equal(1, result);
    }


    [Fact]
    public async Task ProvideLoan_SubscriberDoesNotExist_ThrowsException()
    {
        string mail = "newmailfortests1@gmail.com";
        string content = "Loan created";

        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockHttpClientHelper = fixture.Create<Mock<HttpClientHelper>>();
        var loggerMock = new Mock<ILogger<LoanService.Services.LoanService>>();
        var mockLoanServiceHelper = fixture.Create<Mock<LoanServiceHelper>>();
        var dbMock = fixture.Create<Mock<LoanDb>>();

        var request = LoanMockData.NewLoan();
        mockLoanServiceHelper
            .Setup(x => x.CreateLoan(LoanMockData.subscriberModel(), request.Amount, request.loan_type))
            .Returns(request.id);
        mockHttpClientHelper.Setup(x => x.GetScoringResponse(request)).ReturnsAsync("true");
        mockHttpClientHelper.Setup(x => x.GetSubscriber(request.subscriberId)).ReturnsAsync((SubscriberModel)null);
        mockLoanServiceHelper.Setup(x => x.SendResponse(mail, content)).ReturnsAsync(new StatusCodeResult(200));
        var sut = new LoanService.Services.LoanService(loggerMock.Object, dbMock.Object, mockHttpClientHelper.Object,
            mockLoanServiceHelper.Object);
        var aa = await Assert.ThrowsAsync<InvalidConstraintException>(() => sut.ProvideLoan(request));
        Assert.Equal("Subscriber not found", aa.Message);
    }

    [Fact]
    public async Task ProvideLoan_SuscriberNotEligible_ThrowsException()
    {
        string mail = "newmailfortests1@gmail.com";
        string content = "Loan created";
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockHttpClientHelper = fixture.Create<Mock<HttpClientHelper>>();
        var loggerMock = new Mock<ILogger<LoanService.Services.LoanService>>();
        var mockLoanServiceHelper = fixture.Create<Mock<LoanServiceHelper>>();
        var dbMock = fixture.Create<Mock<LoanDb>>();

        var request = LoanMockData.NewLoan();

        mockLoanServiceHelper
            .Setup(x => x.CreateLoan(LoanMockData.subscriberModel(), request.Amount, request.loan_type))
            .Returns(request.id);
        mockHttpClientHelper.Setup(x => x.GetScoringResponse(request)).ReturnsAsync("false");
        mockHttpClientHelper.Setup(x => x.GetSubscriber(request.subscriberId))
            .ReturnsAsync(LoanMockData.subscriberModel);
        mockLoanServiceHelper.Setup(x => x.SendResponse(mail, content)).ReturnsAsync(new StatusCodeResult(200));
        var sut = new LoanService.Services.LoanService(loggerMock.Object, dbMock.Object, mockHttpClientHelper.Object,
            mockLoanServiceHelper.Object);
        var aa = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.ProvideLoan(request));
        Assert.Equal("Subscriber not eligible.", aa.Message);
    }


    [Fact]
    public async Task IsEligible_ReturnsOK()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockHttpClientHelper = fixture.Create<Mock<HttpClientHelper>>();
        var loggerMock = new Mock<ILogger<LoanService.Services.LoanService>>();
        var mockLoanServiceHelper = fixture.Create<Mock<LoanServiceHelper>>();
        var dbMock = fixture.Create<Mock<LoanDb>>();

        var request = LoanMockData.IsEligible();

        mockHttpClientHelper.Setup(x => x.GetScoringResponse(request)).ReturnsAsync("true");

        var sut = new LoanService.Services.LoanService(loggerMock.Object, dbMock.Object, mockHttpClientHelper.Object,
            mockLoanServiceHelper.Object);

        var result = await sut.IsEligible(request);
        Assert.NotNull(result);
        Assert.Equal("Customer is eligible for take loan.", result);
    }

    [Fact]
    public async Task IsEligible_ReturnsFalse()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockHttpClientHelper = fixture.Create<Mock<HttpClientHelper>>();
        var loggerMock = new Mock<ILogger<LoanService.Services.LoanService>>();
        var mockLoanServiceHelper = fixture.Create<Mock<LoanServiceHelper>>();
        var dbMock = fixture.Create<Mock<LoanDb>>();

        var request = LoanMockData.IsEligible();

        mockHttpClientHelper.Setup(x => x.GetScoringResponse(request)).ReturnsAsync("false");

        var sut = new LoanService.Services.LoanService(loggerMock.Object, dbMock.Object, mockHttpClientHelper.Object,
            mockLoanServiceHelper.Object);
        var result = await sut.IsEligible(request);

        Assert.NotNull(result);
        Assert.Equal("Customer is not eligible for take loan.", result);
    }

    [Fact]
    public async Task UpdateAmount_ReturnsOK()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockHttpClientHelper = fixture.Create<Mock<HttpClientHelper>>();
        var loggerMock = new Mock<ILogger<LoanService.Services.LoanService>>();
        var mockLoanServiceHelper = fixture.Create<Mock<LoanServiceHelper>>();
        var dbMock = new Mock<LoanDb>();

        var loanModel = LoanMockData.LoanById();
        var request = new RepayLoanRequest() { id = 1, subscriberId = 1, amount = 2 };

        dbMock.Setup(x => x.Loans).ReturnsDbSet(new[] { loanModel });
        mockLoanServiceHelper.Setup(x => x.UpdateAmountById(loanModel, request.amount)).ReturnsAsync(loanModel.id);

        var sut = new LoanService.Services.LoanService(loggerMock.Object, dbMock.Object, mockHttpClientHelper.Object,
            mockLoanServiceHelper.Object);
        var result = await sut.UpdateAmount(request);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task UpdateAmount_AmountDoesNotExist_ThrowsException()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockHttpClientHelper = fixture.Create<Mock<HttpClientHelper>>();
        var loggerMock = new Mock<ILogger<LoanService.Services.LoanService>>();
        var mockLoanServiceHelper = fixture.Create<Mock<LoanServiceHelper>>();
        var dbMock = new Mock<LoanDb>();

        var loanModel = LoanMockData.LoanById();
        var request = new RepayLoanRequest() { id = 1, subscriberId = 1, amount = 0 };

        dbMock.Setup(x => x.Loans).ReturnsDbSet(new[] { loanModel });
        mockLoanServiceHelper.Setup(x => x.UpdateAmountById(loanModel, request.amount)).ReturnsAsync(loanModel.id);

        var sut = new LoanService.Services.LoanService(loggerMock.Object, dbMock.Object, mockHttpClientHelper.Object,
            mockLoanServiceHelper.Object);
        var result = await Assert.ThrowsAsync<InvalidConstraintException>(() => sut.UpdateAmount(request));
        Assert.Equal("Amount not found", result.Message);
    }

    [Fact]
    public async Task UpdateAmount_LoanDoesNotExist_ThrowsException()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockHttpClientHelper = fixture.Create<Mock<HttpClientHelper>>();
        var loggerMock = new Mock<ILogger<LoanService.Services.LoanService>>();
        var mockLoanServiceHelper = fixture.Create<Mock<LoanServiceHelper>>();
        var dbMock = new Mock<LoanDb>();

        var loanModel = LoanMockData.LoanById();
        var request = new RepayLoanRequest() { id = 11, subscriberId = 1, amount = 0 };

        dbMock.Setup(x => x.Loans).ReturnsDbSet(new[] { new LoanModel() });
        mockLoanServiceHelper.Setup(x => x.UpdateAmountById(loanModel, request.amount)).ReturnsAsync(loanModel.id);

        var sut = new LoanService.Services.LoanService(loggerMock.Object, dbMock.Object, mockHttpClientHelper.Object,
            mockLoanServiceHelper.Object);
        var result = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.UpdateAmount(request));
        Assert.Equal($"Active loan not found. LoanId: {request.id}", result.Message);
    }

    [Fact]
    public async Task ArchiveLoan_LoanExists_ReturnsOk()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockHttpClientHelper = fixture.Create<Mock<HttpClientHelper>>();
        var loggerMock = new Mock<ILogger<LoanService.Services.LoanService>>();
        var mockLoanServiceHelper = fixture.Create<Mock<LoanServiceHelper>>();
        var dbMock = new Mock<LoanDb>();

        string mail = "newmailfortests1@gmail.com";
        string content = "Loan repaid";
        var loan = LoanMockData.LoanById();
        int id = 1;
        dbMock.Setup(x => x.Loans).ReturnsDbSet(new[] { loan });
        dbMock.Setup<DbSet<ArchiveModel>>(x => x.Archives).ReturnsDbSet(new List<ArchiveModel>());

        mockLoanServiceHelper.Setup(x => x.AddArchive(loan)).Returns(Task.CompletedTask);
        mockLoanServiceHelper.Setup(x => x.SendResponse(mail, content)).ReturnsAsync(new StatusCodeResult(200));

        var sut = new LoanService.Services.LoanService(loggerMock.Object, dbMock.Object, mockHttpClientHelper.Object,
            mockLoanServiceHelper.Object);
        await sut.ArchiveLoan(id);
    }

    [Fact]
    public async Task ArchiveLoan_LoanDoesNotExist_ThrowsException()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockHttpClientHelper = fixture.Create<Mock<HttpClientHelper>>();
        var loggerMock = new Mock<ILogger<LoanService.Services.LoanService>>();
        var mockLoanServiceHelper = fixture.Create<Mock<LoanServiceHelper>>();
        var dbMock = new Mock<LoanDb>();

        string mail = "newmailfortests1@gmail.com";
        string content = "Loan repaid";
        var loan = LoanMockData.LoanById();
        int id = 1;
        dbMock.Setup(x => x.Loans).ReturnsDbSet(new List<LoanModel>());
        dbMock.Setup<DbSet<ArchiveModel>>(x => x.Archives).ReturnsDbSet(new List<ArchiveModel>());

        mockLoanServiceHelper.Setup(x => x.AddArchive(loan)).Returns(Task.CompletedTask);
        mockLoanServiceHelper.Setup(x => x.SendResponse(mail, content)).ReturnsAsync(new StatusCodeResult(200));

        var sut = new LoanService.Services.LoanService(loggerMock.Object, dbMock.Object, mockHttpClientHelper.Object,
            mockLoanServiceHelper.Object);
        var result = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.ArchiveLoan(id));
        Assert.Equal("Loan not found", result.Message);
    }
}