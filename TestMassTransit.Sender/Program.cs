using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(bus =>
{
    bus.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(true));

    bus.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
