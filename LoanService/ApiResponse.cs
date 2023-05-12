namespace LoanService;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public string StatusDescription { get; set; }
    public T Response { get; set; }

    public ApiResponse(int statusCode, string statusDescription, T response = default)
    {
        StatusCode = statusCode;
        StatusDescription = statusDescription;
        Response = response;
    }
}