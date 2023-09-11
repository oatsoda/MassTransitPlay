using Microsoft.EntityFrameworkCore;
using MassTransitPlay.Data.Models;
using MassTransit;

namespace MassTransitPlay.Data
{
    public class IssueTrackerDbContext : DbContext
    {
        public DbSet<User> User { get; set; } = null!;
        public DbSet<Issue> Posts { get; set; } = null!;

        public IssueTrackerDbContext(DbContextOptions<IssueTrackerDbContext> options): base(options) { }

        // Add EF Migrations with:
        //   dotnet ef migrations add <name> --project MassTransitPlay.Api.Data --startup-project MassTransitPlay.Api


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
                        
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
