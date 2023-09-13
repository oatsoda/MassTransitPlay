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
            var totals = await m_DbContext.Totals.SingleOrDefaultAsync(t => t.Type == IssueTotals.OVERALL_TYPE);

            if (totals == null)
            {
                m_Logger.LogInformation("Creating initial Totals");
                totals = new IssueTotals() 
                {
                    Type = IssueTotals.OVERALL_TYPE,
                    TotalIssues = 1
                };
                m_DbContext.Totals.Add(totals);
            }
            else
            {
                totals.TotalIssues++;
                m_Logger.LogInformation("Increasing total issues to {num}", totals.TotalIssues);
            }

            await m_DbContext.SaveChangesAsync();     
        }
    }
}
