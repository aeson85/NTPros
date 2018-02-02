using System;
using RabbitMQ.Client;

namespace NT_MQConsumer.Infrastructure.Handler
{
    public interface IConsumerHandler
    {
        string RoutingKey { get; }

        string QueueName { get; }

        void Execute(string message);
    }
}