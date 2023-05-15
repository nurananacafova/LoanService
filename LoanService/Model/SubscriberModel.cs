using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace LoanService.Model;

public class SubscriberModel
{
    [Key] public int id { get; set; }
    [Required] public string? language { get; set; }
    [Required] public string? email { get; set; }

    [JsonProperty("registration_date")]
    [Required]
    public DateTime registration_date { get; set; }
}