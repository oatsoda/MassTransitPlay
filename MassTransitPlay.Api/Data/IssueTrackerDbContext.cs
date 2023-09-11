using Microsoft.EntityFrameworkCore;
using MassTransitPlay.Data.Models;

namespace MassTransitPlay.Data
{
    public class IssueTrackerDbContext : DbContext
    {
        public DbSet<User> User { get; set; } = null!;
        public DbSet<Issue> Posts { get; set; } = null!;

        public IssueTrackerDbContext(DbContextOptions<IssueTrackerDbContext> options): base(options) { }

        // Add EF Migrations with:
        //   dotnet ef migrations add <name>
    }
}
