using System;
using RabbitMQ.Client;

namespace NT_MQConsumer.Infrastructure
{
    public interface IConsumerHandler
    {
        string RoutingKey { get; }

        string QueueName { get; }

        void Execute(string message);
    }
}