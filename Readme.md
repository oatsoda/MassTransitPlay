### A playground project for MassTransit

Including:
- MT Transactional Outbox with EF Core.
- Rabbit MQ.
- Async Integration Event processing.
- Minimal APIs

### Questions

- Struggling to understand how, with MassTransit, I can have more fine-grained control over the message Publishing. For example:
  -	If I have turned on the Outbox then all Published messages go via SQL Outbox before being dispatched - but what if I want to dispatch an in-memory, in-process Domain Event to execute BEFORE the Save (as part of the Save Transaction)
	- Does this mean "true" Domain Events to update the Domain Model need to be done some other way?
  - If I have turned on Rabbit MQ, then all Published messages will be dispatched via RabbitMQ - but what if I want to dispatch an in-memory, in-process Domain Event?
	- Do I need to use MultiBus and configure a local one? https://masstransit.io/documentation/configuration/multibus
	- But ideally Publisher shouldn't need to know this...

### Future?

- Domain Events vs Integration - updating Domain before SaveAsync
- Add another Microservice
- Clarity over async processing within same Bounded Context vs separate Bounded Context / Microservice.
