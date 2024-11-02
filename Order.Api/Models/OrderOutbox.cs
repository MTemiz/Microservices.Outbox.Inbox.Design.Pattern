using System.ComponentModel.DataAnnotations;

namespace Order.Api.Models;

public class OrderOutbox
{
    [Key]
    public Guid IdempotentToken { get; set; }
    public DateTime OccuredOn { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public string Type { get; set; }
    public string Payload { get; set; }
    
}