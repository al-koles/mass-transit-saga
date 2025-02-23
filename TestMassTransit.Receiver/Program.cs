using MassTransit;
using MassTransit.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TestMassTransit.Receiver.Consumers;

var builder = WebApplication.CreateBuilder(args);

// builder.AddServiceDefaults();

const string serviceName = "receiver";
builder.Logging.ClearProviders();
// builder.Logging.AddOpenTelemetry(options =>
// {
//     options
//         .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
//         .AddConsoleExporter();
// });
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName))
    .WithTracing(tracing => tracing
        .AddSource(DiagnosticHeaders.DefaultListenerName)
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter())
    // .WithMetrics(metrics => metrics
    //     .AddAspNetCoreInstrumentation()
    //     .AddConsoleExporter())
    ;

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
