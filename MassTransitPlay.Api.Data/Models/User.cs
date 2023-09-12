using System.ComponentModel.DataAnnotations;

namespace MassTransitPlay.Api.Domain.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
}