using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();

builder.AddProject<TestMassTransit_Sender>("sender")
    .WithExternalHttpEndpoints()
    .WithReference(rabbitmq);

builder.AddProject<TestMassTransit_Receiver>("receiver")
    .WithReference(rabbitmq);;

builder.Build().Run();
