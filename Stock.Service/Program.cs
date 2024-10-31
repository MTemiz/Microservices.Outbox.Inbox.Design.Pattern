using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Events;
using Stock.Service;
using Stock.Service.Consumer;
using Stock.Service.Models.Contexts;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<StockDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<OrderCreatedEventConsumer>();

    configure.UsingRabbitMq((context, rabbitConfigurator) =>
    {
        rabbitConfigurator.Host(builder.Configuration["RabbitMQ"]);

        rabbitConfigurator.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedQueue,
            rabbitConfigurator => rabbitConfigurator.ConfigureConsumer<OrderCreatedEventConsumer>(context));
    });
});

var host = builder.Build();
host.Run();