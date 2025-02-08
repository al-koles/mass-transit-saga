using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// builder.add
builder.AddProject<TestMassTransit_Sender>("sender")
    .WithExternalHttpEndpoints();
builder.AddProject<TestMassTransit_Receiver>("receiver");

builder.Build().Run();
