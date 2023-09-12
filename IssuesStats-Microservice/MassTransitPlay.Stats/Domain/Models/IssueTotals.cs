using System.ComponentModel.DataAnnotations;

namespace MassTransitPlay.Stats.Domain.Models;

public class IssueTotals
{
    [Key]
    public Guid Id { get; set; } = Guid.Empty;

    public int TotalIssues { get; set; }
}