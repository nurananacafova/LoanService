using System.Net;
using LoanService;
using LoanService.Model;
using Newtonsoft.Json;
using WireMock.Server;

namespace LoanServiceIntegration.Tests.System;

public class IntegrationTests
{
    [Fact]
    public async Task GetAllLoans_ReturnsOk()
    {
        var server = WireMockServer.Start();
        var httpClient = server.CreateClient();
        httpClient.BaseAddress = new Uri($"http://localhost:{5002}");

        var response = await httpClient.GetAsync("/api/LoanService/GetAllLoans");
        var body = await response.Content.ReadAsStringAsync();
        var actualResult = JsonConvert.DeserializeObject<List<LoanModel>>(body);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetLoanById_ReturnsOk()
    {
        var server = WireMockServer.Start();
        var httpClient = server.CreateClient();
        httpClient.BaseAddress = new Uri($"http://localhost:{5002}");
        int loanId = 9045;
        var response = await httpClient.GetAsync($"/api/LoanService/GetLoan?id={loanId}");
        var body = await response.Content.ReadAsStringAsync();
        var actualResult = JsonConvert.DeserializeObject<LoanModel>(body);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(actualResult.amount == 1914);
    }

    [Fact]
    public async Task GetLoanById_LoanDoesNotExist_ReturnsNull()
    {
        var server = WireMockServer.Start();
        var httpClient = server.CreateClient();
        httpClient.BaseAddress = new Uri($"http://localhost:{5002}");
        int loanId = 9045785;
        var response = await httpClient.GetAsync($"/api/LoanService/GetLoan?id={loanId}");
        var body = await response.Content.ReadAsStringAsync();
        var actualResult = JsonConvert.DeserializeObject<LoanModel>(body);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetAmountById_ReturnsOk()
    {
        var server = WireMockServer.Start();
        var httpClient = server.CreateClient();
        httpClient.BaseAddress = new Uri($"http://localhost:{5002}");
        int loanId = 9045;
        var response = await httpClient.GetAsync($"/api/LoanService/GetLoan/amount?id={loanId}");
        var body = await response.Content.ReadAsStringAsync();
        var actualResult = JsonConvert.DeserializeObject<int>(body);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(actualResult == 1914);
    }

    [Fact]
    public async Task GetAmountById_LoanDoesNotExist_ReturnsNull()
    {
        var server = WireMockServer.Start();
        var httpClient = server.CreateClient();
        httpClient.BaseAddress = new Uri($"http://localhost:{5002}");
        int loanId = 9044565;
        var response = await httpClient.GetAsync($"/api/LoanService/GetLoan/amount?id={loanId}");
        var body = await response.Content.ReadAsStringAsync();
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task IsEligible_ReturnsOk()
    {
        var request = new ProvideLoanRequest()
        {
            subscriberId = 6,
            Amount = 23,
        };
        var server = WireMockServer.Start();
        var httpClient = server.CreateClient();
        httpClient.BaseAddress = new Uri($"http://localhost:{5002}");
        var response =
            await httpClient.GetAsync(
                $"/api/LoanService/EligibilityCheck?subscriberId={request.subscriberId}&amount={request.Amount}");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Customer is eligible for take loan.", body);
    }

    [Fact]
    public async Task IsEligible_ReturnsFalse()
    {
        var request = new ProvideLoanRequest()
        {
            subscriberId = 1,
            Amount = 23,
        };
        var server = WireMockServer.Start();
        var httpClient = server.CreateClient();
        httpClient.BaseAddress = new Uri($"http://localhost:{5002}");

        var response =
            await httpClient.GetAsync(
                $"/api/LoanService/EligibilityCheck?subscriberId={request.subscriberId}&amount={request.Amount}");
        var body = await response.Content.ReadAsStringAsync();


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Customer is not eligible for take loan.", body);
    }

    [Fact]
    public async Task ProvideLoan_ReturnsOk()
    {
        var loan = new LoanModel()
        {
            subscriberId = 5,
            amount = 234,
            loan_type = "Simple Loan"
        };

        var server = WireMockServer.Start();
        var httpClient = server.CreateClient();
        httpClient.BaseAddress = new Uri($"http://localhost:{5002}");
        var response =
            await httpClient.PostAsJsonAsync("/api/LoanService/ProvideLoan", loan);

        var body = await response.Content.ReadAsStringAsync();
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ProvideLoan_SubscriberDoesNotExist_ReturnsOk()
    {
        var loan = new LoanModel()
        {
            subscriberId = 0,
            amount = 234,
            loan_type = "Simple Loan"
        };

        var server = WireMockServer.Start();
        var httpClient = server.CreateClient();
        httpClient.BaseAddress = new Uri($"http://localhost:{5002}");
        var response =
            await httpClient.PostAsJsonAsync("/api/LoanService/ProvideLoan", loan);

        var body = await response.Content.ReadAsStringAsync();
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("Subscriber not found", body);
    }

    [Fact]
    public async Task ProvideLoan_SubscriberNotEligible_ReturnsOk()
    {
        var loan = new LoanModel()
        {
            subscriberId = 1,
            amount = 234,
            loan_type = "Simple Loan"
        };

        var server = WireMockServer.Start();
        var httpClient = server.CreateClient();
        httpClient.BaseAddress = new Uri($"http://localhost:{5002}");
        var response =
            await httpClient.PostAsJsonAsync("/api/LoanService/ProvideLoan", loan);

        var body = await response.Content.ReadAsStringAsync();
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("Subscriber not eligible.", body);
    }

    [Fact]
    public async Task RepayLoan_ReturnsOk()
    {
        var request = new RepayLoanRequest()
        {
            id = 9046,
            amount = 43,
        };

        var server = WireMockServer.Start();
        var httpClient = server.CreateClient();
        httpClient.BaseAddress = new Uri($"http://localhost:{5002}");

        var response = await httpClient.PutAsJsonAsync($"/api/LoanService/RepayLoan", request);
        var body = await response.Content.ReadAsStringAsync();
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task RepayLoan_LoanDoesNotExist_ThrowsException()
    {
        var request = new RepayLoanRequest()
        {
            id = 9045646,
            amount = 43,
        };

        var server = WireMockServer.Start();
        var httpClient = server.CreateClient();
        httpClient.BaseAddress = new Uri($"http://localhost:{5002}");

        var response = await httpClient.PutAsJsonAsync($"/api/LoanService/RepayLoan", request);
        var body = await response.Content.ReadAsStringAsync();
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal($"Active loan not found. LoanId: {request.id}", body);
    }

    [Fact]
    public async Task RepayLoan_AmountDoesNotExist_ThrowsException()
    {
        var request = new RepayLoanRequest()
        {
            id = 9046,
            amount = 0,
        };

        var server = WireMockServer.Start();
        var httpClient = server.CreateClient();
        httpClient.BaseAddress = new Uri($"http://localhost:{5002}");

        var response = await httpClient.PutAsJsonAsync($"/api/LoanService/RepayLoan", request);
        var body = await response.Content.ReadAsStringAsync();
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("Amount not found", body);
    }

    [Fact]
    public async Task ArchiveLoan_ReturnsOk()
    {
        var server = WireMockServer.Start();
        var httpClient = server.CreateClient();
        httpClient.BaseAddress = new Uri($"http://localhost:{5002}");
        int loanId = 7028;

        var response = await httpClient.DeleteAsync($"/api/LoanService/ArchiveLoan?id={loanId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ArchiveLoan_LoanDoesNotExist_ThrowsException()
    {
        var server = WireMockServer.Start();
        var httpClient = server.CreateClient();
        httpClient.BaseAddress = new Uri($"http://localhost:{5002}");
        int loanId = 702545;

        var response = await httpClient.DeleteAsync($"/api/LoanService/ArchiveLoan?id={loanId}");
        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("Loan not found", body);
    }
}