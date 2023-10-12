using PA.QuoteSubmission.SharedKernel.DomainObjects;
using PA.QuoteSubmission.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;

namespace PA.QuoteSubmission.SharedKernel.Implementation
{
    public abstract class RestoreCurrentState<T> : IRestoreCurrentState<T> where T : AggregateRoot
    {
        private readonly IRepository<T>? eventStore;
        private readonly ICacheService<T>? cacheService;
        private readonly DomainEventDispatcher eventDispatcher;

        public RestoreCurrentState(DomainEventDispatcher eventDispatcher, IRepository<T>? eventStore)
        {
            this.eventDispatcher = eventDispatcher;
            this.eventStore = eventStore;
        }
        public RestoreCurrentState(DomainEventDispatcher eventDispatcher, ICacheService<T>? cacheService)
        {
            this.eventDispatcher = eventDispatcher;
            this.cacheService = cacheService;
        }
        public RestoreCurrentState(DomainEventDispatcher eventDispatcher, ICacheService<T>? cacheService, IRepository<T>? eventStore)
        {
            this.eventDispatcher = eventDispatcher;
            this.cacheService = cacheService;
            this.eventStore = eventStore;
        }
        public async Task<T> GetCurrentStateFromCache(ICacheService<T> cacheClient, string cacheKey)
        {
            return await cacheClient.GetValue(cacheKey);
        }

        //T - event message data format, replace IRepository with event repository
        //one partition per aggregate root
        //option to identify if event is replaying or live
        //--------------------------------------
        //cannot have generic implementation which can be reused.
        //event structure - eventid, aggrgate id, event type, affected entity, event body (invokes dependent event handlers)
        // create new aggregate based on first event in the partition
        // add the properties/entities to the aggregate from the subsequent events
        // No Notification handlers will be invoked to supress any external service invocation
        //challenges:
        // if all events to build an aggregate are not available because of ttl of the events in eventstore, it will result in incomplete aggregate reconstruction
        // solution:
        // if all the necessary events are available in eventstore, we will cancel request?
        //base event - id, date & event type & abstract process event
        //process event - will update the aggregate (no external invocations)
        //external event invocations, we will be mediatr framework

        public async Task<T> GetCurrentStateByReplay<Tevent>(IEventRepository<Tevent> eventStore, string partitionKey, T aggregateRoot)
            where Tevent : DomainEvent
        {
            List<Tevent> eventList = await eventStore.GetAllEvents(partitionKey);
            foreach (var e in eventList)
            {
                e.ProcessEvent(aggregateRoot);
            }
            return aggregateRoot;
        }

    }
}
