namespace MassTransitPlay.SharedContracts; // MassTransit requires the namespace to be the same on both Producer and Consumer. Rather than share an Assembly, just fix the Namespace.

public record IssueCreatedIntegrationEvents(Guid Id, string Title);
