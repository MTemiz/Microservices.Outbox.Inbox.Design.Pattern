using Quartz;

namespace Order.Outbox.Table.Publisher.Service.Jobs;

public class OrderOutboxPublishJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }
}