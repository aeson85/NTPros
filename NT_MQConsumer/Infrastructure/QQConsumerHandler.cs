using System;
using RabbitMQ.Client;

namespace NT_MQConsumer.Infrastructure
{
    public class QQConsumerHandler : IConsumerHandler
    {
        public string RoutingKey => "qq.#";

        public string QueueName => throw new NotImplementedException();

        public void Execute(string message)
        {
            throw new NotImplementedException();
        }
    }
}