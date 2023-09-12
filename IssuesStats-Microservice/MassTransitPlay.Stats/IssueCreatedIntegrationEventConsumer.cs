using MassTransit;
using MassTransitPlay.Stats.Domain.Models;
using MassTransitPlay.Stats.Domain.Persistence;
using MassTransitPlay.SharedContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MassTransitPlay.Stats
{
    public class IssueCreatedIntegrationEventConsumer : IConsumer<IssueCreatedIntegrationEvent>
    {
        private readonly IssueStatsDbContext m_DbContext;
        private readonly ILogger<IssueCreatedIntegrationEventConsumer> m_Logger;

        public IssueCreatedIntegrationEventConsumer(IssueStatsDbContext dbContext, ILogger<IssueCreatedIntegrationEventConsumer> logger)
        {
            m_DbContext = dbContext;
            m_Logger = logger;
        }

        public async Task Consume(ConsumeContext<IssueCreatedIntegrationEvent> context)
        {
            while (true) // TODO: Implement max retries
            {                
                var totals = await m_DbContext.Totals.FindAsync(Guid.Empty); // OOOOPS, The DB auto overrides the ID, so need to fix this.

                if (totals == null)
                {
                    m_Logger.LogInformation("Creating initial Totals");
                    totals = new IssueTotals() 
                    {
                        TotalIssues = 1
                    };
                    m_DbContext.Totals.Add(totals);
                }
                else
                {
                    totals.TotalIssues++;
                }


                try
                {
                    await m_DbContext.SaveChangesAsync();
                    return;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // TODO: Rather than re-execute the Find, the exception has the latest versio on ex.Entries
                    m_Logger.LogWarning(ex, "Concurrency failure; Retrying...");
                }
            }
        }
    }
}
