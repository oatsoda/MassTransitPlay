using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MassTransitPlay.Stats.Domain.Models;

[Index(nameof(Type), IsUnique = true)]
public class IssueTotals
{
    public const string OVERALL_TYPE = "Overall";

    [Key]
    public Guid Id { get; set; }

    public string Type { get; set; } = OVERALL_TYPE;

    public int TotalIssues { get; set; }
}