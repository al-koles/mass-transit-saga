using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var elasticsearch = builder.AddElasticsearch("elasticsearch",
        password: builder.AddParameter("ElasticsearchPassword"))
    .WithEnvironment("xpack.security.enabled", "false")
    .WithDataVolume("elasticsearch_volume")
    .WithLifetime(ContainerLifetime.Persistent);

builder.AddContainer("kibana", "docker.elastic.co/kibana/kibana", "8.15.3")
    .WithEnvironment("ELASTICSEARCH_HOSTS", "http://elasticsearch:9200")
    .WithEndpoint(5601, 5601, "http", "kibana")
    .WaitFor(elasticsearch)
    .WithLifetime(ContainerLifetime.Persistent);

var rabbitmq = builder
    .AddRabbitMQ("rabbitmq", password: builder.AddParameter("RabbitmqPassword", secret: true))
    .WithManagementPlugin(15673)
    .WithDataVolume("rabbitmq_volume")
    .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<TestMassTransit_Sender>("sender")
    .WithExternalHttpEndpoints()
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(elasticsearch)
    .WaitFor(elasticsearch);

builder.AddProject<TestMassTransit_Receiver>("receiver")
    .WithExternalHttpEndpoints()
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(elasticsearch)
    .WaitFor(elasticsearch);

builder.Build().Run();
