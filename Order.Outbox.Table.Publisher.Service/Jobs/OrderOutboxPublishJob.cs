using System.Text.Json;
using MassTransit;
using Order.Outbox.Table.Publisher.Service.Entities;
using Quartz;
using Shared.Events;

namespace Order.Outbox.Table.Publisher.Service.Jobs;

[DisallowConcurrentExecution]
public class OrderOutboxPublishJob(IPublishEndpoint publishEndpoint) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        if (OrderOutboxSingletonDatabase.DataReaderState)
        {
            OrderOutboxSingletonDatabase.SetDataReaderBusy();

            var outboxList =
                (await OrderOutboxSingletonDatabase.QueryAsync<OrderOutbox>(
                    $@"Select * from OrderOutboxes Where ProcessedDate is null Order By OccuredOn Asc")).ToList();

            foreach (var orderOutbox in outboxList)
            {
                if (orderOutbox.Type == nameof(OrderCreatedEvent))
                {
                    OrderCreatedEvent orderCreatedEvent =
                        JsonSerializer.Deserialize<OrderCreatedEvent>(orderOutbox.Payload);

                    if (orderCreatedEvent != null)
                    {
                        await publishEndpoint.Publish(orderCreatedEvent);

                        await OrderOutboxSingletonDatabase.ExecuteAsync(
                            $"Update OrderOutboxes Set ProcessedDate = GetDate() Where Id = '{orderOutbox.Id}'");
                    }
                }
            }

            OrderOutboxSingletonDatabase.SetDataReaderReady();

            await Console.Out.WriteLineAsync($"[{DateTime.Now}] - OrderOutbox published.");
        }
    }
}