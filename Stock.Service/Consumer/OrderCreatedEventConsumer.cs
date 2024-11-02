using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using Stock.Service.Models.Contexts;

namespace Stock.Service.Consumer;

public class OrderCreatedEventConsumer(StockDbContext _stockDbContext) : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var result = await _stockDbContext.OrderInboxes.AnyAsync(
            c => c.IdempotentToken == context.Message.IdempotentToken);

        if (!result)
        {
            await _stockDbContext.OrderInboxes.AddAsync(new()
            {
                Processed = false,
                Payload = JsonSerializer.Serialize(context.Message),
                IdempotentToken = context.Message.IdempotentToken
            });

            await _stockDbContext.SaveChangesAsync();
        }

        var inboxList = await _stockDbContext
            .OrderInboxes
            .Where(c => c.Processed == false)
            .ToListAsync();

        foreach (var inbox in inboxList)
        {
            var orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(inbox.Payload);

            await Console.Out.WriteLineAsync(
                $"{orderCreatedEvent.OrderId} order id değerine karşılık olan siparişin stok işlemleri başarıyla tamamlandı.");

            inbox.Processed = true;

            await _stockDbContext.SaveChangesAsync();
        }
    }
}