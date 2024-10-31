using MassTransit;
using Order.Outbox.Table.Publisher.Service;
using Order.Outbox.Table.Publisher.Service.Jobs;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddQuartz(configurator =>
{
    var orderOutboxPublishJobJobKey = new JobKey("OrderOutboxPublishJob");

    configurator.AddJob<OrderOutboxPublishJob>(jobConfigurator =>
    {
        jobConfigurator.WithIdentity(orderOutboxPublishJobJobKey);
    });

    TriggerKey orderOutboxPublishJobTriggerKey = new TriggerKey("OrderOutboxPublishTrigger");

    configurator.AddTrigger(triggerConfigurator =>
    {
        triggerConfigurator
            .ForJob(orderOutboxPublishJobJobKey)
            .WithIdentity(orderOutboxPublishJobTriggerKey)
            .StartAt(DateTime.UtcNow)
            .WithSimpleSchedule(x => x.WithIntervalInSeconds(5).RepeatForever());
    });
});

builder.Services.AddMassTransit(configure =>
{
    configure.UsingRabbitMq((context, factoryConfigurator) =>
    {
        factoryConfigurator.Host(builder.Configuration["RabbitMQ"]);
    });
});

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

var host = builder.Build();
host.Run();