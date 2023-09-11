using System.ComponentModel.DataAnnotations;

namespace MassTransitPlay.Data.Models;

public class Issue
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? AssigneeId { get; set; }
    public Guid? OriginatorId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsOpen { get; set; }

    public DateTimeOffset Opened { get; set; }

    public ICollection<IssueTask> Tasks { get; set; } = new List<IssueTask>();
}