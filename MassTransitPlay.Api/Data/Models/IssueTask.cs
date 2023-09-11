using System.ComponentModel.DataAnnotations;

namespace MassTransitPlay.Data.Models;

public class IssueTask
{
    [Key]
    public Guid Id { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? Started { get; set; }
    public DateTimeOffset Finished { get; set; }
}