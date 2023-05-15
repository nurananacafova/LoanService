using System;
using System.Net.Http;
using System.Threading.Tasks;
using LoanService.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LoanService;

public class HttpClientHelper : IHttpClientHelper
{
    private HttpClient _client = new HttpClient();
    private readonly Uri _subscriberServiceUrl = new Uri("http://subscriber:5000/api/Subscribers/GetSubscriber/");
    private readonly Uri _scoringServiceUrl = new Uri("http://scoringservice:5004/api/ScoringService/ScoreSubscriber/");
    private readonly Uri _responseServiceUrl = new Uri("http://responseservice:5001/api/Mail/SendMail");
    private readonly ILogger<HttpClientHelper> _logger;

    public HttpClientHelper(ILogger<HttpClientHelper> logger)
    {
        _client.BaseAddress = _subscriberServiceUrl;
        _client.BaseAddress = _scoringServiceUrl;
        _client.BaseAddress = _responseServiceUrl;
        _logger = logger;
    }

    public virtual async Task<SubscriberModel> GetSubscriber(int id)
    {
        SubscriberModel subscriberModel;
        using (var httpClient = new HttpClient())
        {
            using (var subscriberResponse = await httpClient.GetAsync(_subscriberServiceUrl + $"?id={id}"))
            {
                _logger.LogInformation($"Subscriber Service HTTP Client status code: {subscriberResponse.StatusCode}");
                try
                {
                    string subscriberApiResponse = await subscriberResponse.Content.ReadAsStringAsync();
                    subscriberModel = JsonConvert.DeserializeObject<SubscriberModel>(subscriberApiResponse);
                    if (subscriberModel == null)
                    {
                        return null;
                    }

                    return subscriberModel;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    throw;
                }
            }
        }
    }


    public virtual async Task<string> GetScoringResponse(ProvideLoanRequest request)
    {
        string scoringApiResponse;
        using (var httpClient = new HttpClient())
        {
            using (var scoringResponse =
                   await httpClient.GetAsync(_scoringServiceUrl +
                                             $"?id={request.subscriberId}&amount={request.Amount}"))
            {
                _logger.LogInformation($"Scoring Service HTTP Client status code: {scoringResponse.StatusCode}");
                try
                {
                    scoringApiResponse = await scoringResponse.Content.ReadAsStringAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    throw;
                }

                _logger.LogInformation($"Scoring api response: {scoringApiResponse}");
            }
        }

        return scoringApiResponse;
    }


    public virtual Task SendResponse(MailModel mailModel)
    {
        using (var client = new HttpClient())
        {
            var postTask = client.PostAsJsonAsync<MailModel>(_responseServiceUrl, mailModel);
            postTask.Wait();
        }

        return Task.CompletedTask;
    }
}