using Prometheus;

namespace LoanService;

public class LoanMetrics
{
    public static readonly Counter InsertCounter = Metrics
        .CreateCounter("insert_loan", "Post method's counter");

    public static readonly Counter GetCounts = Metrics
        .CreateCounter("get_loan", "Get method's counter");

    public static readonly Counter RequestCountByMethod = Metrics
        .CreateCounter("requests_total", "Number of requests received, by HTTP method.",
            labelNames: new[] { "method" });

    public static readonly Counter GetByIdCount = Metrics
        .CreateCounter("get_by_id_loan", "GetById method's counter");

    public static readonly Counter GetAmountCount = Metrics
        .CreateCounter("get_amount_loan", "Get method's counter for get amount");

    public static readonly Counter UpdateCounter = Metrics
        .CreateCounter("update_loan", "Put method's counter");

    public static readonly Counter DeleteCounter = Metrics
        .CreateCounter("delete_loan", "Delete method's counter");
}