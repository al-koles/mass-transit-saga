using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder
    .AddRabbitMQ("rabbitmq", password: builder.AddParameter("RabbitmqPassword", secret: true))
    .WithManagementPlugin(15673);

builder.AddProject<TestMassTransit_Sender>("sender")
    .WithExternalHttpEndpoints()
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.AddProject<TestMassTransit_Receiver>("receiver")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.Build().Run();
