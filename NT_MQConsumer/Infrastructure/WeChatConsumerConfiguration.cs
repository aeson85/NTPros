using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace NT_MQConsumer.Infrastructure
{
    public class WeChatConsumerHandler : IConsumerHandler
    {
        private readonly IConfiguration _configuration;

        public string RoutingKey => "wechat";

        public string QueueName => _configuration["RabbitMQ:Wechat:QueueName"];

        public WeChatConsumerHandler(IConfiguration configuration) => _configuration = configuration;

        public void Execute(string message)
        {
            System.Console.WriteLine("开始任务");
            Thread.Sleep(5000);
            System.Console.WriteLine("结束任务");
        }
    }
}