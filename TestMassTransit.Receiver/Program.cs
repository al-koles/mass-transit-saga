using MassTransit;
using TestMassTransit.Receiver.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(bus =>
{
    bus.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(true));

    bus.AddConsumer<SampleEventConsumer>();

    bus.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("test",
    (ILogger<Program> logger) =>
    {
        logger.LogInformation("Test endpoint executed");

        return Results.Ok("Hello World");
    });

// app.MapDefaultEndpoints();

app.Run();
