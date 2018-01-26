using System.IO;
using Microsoft.Extensions.Configuration;
using NT_WeChatUtilities;

namespace NT_WebApp.Infrastructure
{
    public class WeChatMessageDelivery : MessageDelivery
    {
        private readonly WeChatUtilities _weChatUtilities;

        protected override string RoutingKey => "wechat";
        protected override string QueueName => "wechat_queue";

        public WeChatMessageDelivery(IConfiguration configuration, WeChatUtilities weChatUtilities) : base(configuration) => _weChatUtilities = weChatUtilities;

        public override T GetMessage<T>(Stream stream)
        {
            var wechatMsg = _weChatUtilities.Parse(stream);
            return wechatMsg as T;
        }
    }
}