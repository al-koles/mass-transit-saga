using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var elasticsearch = builder.AddElasticsearch("elasticsearch",
    password: builder.AddParameter("ElasticsearchPassword"))
    .WithDataVolume("elasticsearch_volume");

var rabbitmq = builder
    .AddRabbitMQ("rabbitmq", password: builder.AddParameter("RabbitmqPassword", secret: true))
    .WithManagementPlugin(15673)
    .WithDataVolume("rabbitmq_volume");

builder.AddProject<TestMassTransit_Sender>("sender")
    .WithExternalHttpEndpoints()
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(elasticsearch)
    .WaitFor(elasticsearch);

builder.AddProject<TestMassTransit_Receiver>("receiver")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(elasticsearch)
    .WaitFor(elasticsearch);

builder.Build().Run();
