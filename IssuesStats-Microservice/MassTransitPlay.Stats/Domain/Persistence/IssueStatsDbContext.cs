using MassTransit;
using MassTransitPlay.Stats.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MassTransitPlay.Stats.Domain.Persistence
{
    public class IssueStatsDbContext : DbContext
    {
        public DbSet<IssueTotals> Totals { get; set; } = null!;
        
        public IssueStatsDbContext(DbContextOptions<IssueStatsDbContext> options): base(options) { }

        // Add EF Migrations with:
        //   dotnet ef migrations add <name> -o Domain/Persistence/Migrations


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
                        
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
