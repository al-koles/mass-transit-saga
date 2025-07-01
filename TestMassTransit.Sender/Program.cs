using MassTransit;
using TestMassTransit.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(bus =>
{
    bus.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(true));

    bus.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddHttpLogging(logging =>
{
    logging.CombineLogs = true;
});

// builder.Services.AddGrpcClient<Greeter.GreeterClient>(o =>
// {
//     o.Address = new Uri(builder.Configuration.GetConnectionString("grpc-service")!);
// });

var app = builder.Build();

app.UseHttpLogging();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");

app.MapGet("sample",
    (IPublishEndpoint publishEndpoint) =>
    {
        publishEndpoint.Publish(new SampleEvent(DateTime.Now));
        
        return Results.Ok("Event published!");
    });

// app.MapGet("say-hello",
//     async (Greeter.GreeterClient greeterClient) =>
//     {
//         var reply = await greeterClient.SayHelloAsync(new HelloRequest { Name = "Sender" });
//
//         return reply;
//     });

app.MapDefaultEndpoints();

app.Run();
