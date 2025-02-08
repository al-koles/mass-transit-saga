using MassTransit;
using TestMassTransit.Contracts;

namespace TestMassTransit.Receiver.Consumers;

public class SampleEventConsumer(
    ILogger<SampleEventConsumer> _logger
) : IConsumer<SampleEvent>
{
    public  Task Consume(ConsumeContext<SampleEvent> context)
    {
        _logger.LogInformation("Received: {Event}", context.Message);

        return Task.CompletedTask;
    }
}
