using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NT_WeChatUtilities;
using RabbitMQ.Client;

namespace NT_MQConsumer.Infrastructure
{
    public class WeChatConsumerHandler : IConsumerHandler
    {
        private readonly IConfiguration _configuration;

        private readonly WeChatUtilities _weChatUtilities;

        public string RoutingKey => "wechat";

        public string QueueName => _configuration["RabbitMQ:Wechat:QueueName"];

        public WeChatConsumerHandler(IConfiguration configuration, WeChatUtilities weChatUtilities)
        {
            _configuration = configuration;
            _weChatUtilities = weChatUtilities;
        }

        public async void Execute(string message)
        {
            var weChatMsg = JsonConvert.DeserializeObject<WeChatMessage>(message);
            if (weChatMsg.MsgType == WeChatMsgType.Event)
            {
                switch (weChatMsg.EventInfo.EventType)
                {
                    case WeChatEventType.Subscribe:
                    {
                        Console.WriteLine("新用户关注消息");
                        await _weChatUtilities.GetUserInfo(weChatMsg.FromUserName);
                    }
                    break;
                    case WeChatEventType.UnSubscribe:
                    {
                        Console.WriteLine("用户取消关注消息");
                    }
                    break;
                }
            }
            Console.WriteLine("结束任务");
        }
    }
}