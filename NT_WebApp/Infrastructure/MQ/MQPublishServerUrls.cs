using Microsoft.Extensions.Configuration;

namespace NT_WebApp.Infrastructure.MQ
{
    public class MQPublishServerUrls
    {
        private readonly IConfiguration _configuration;
        private readonly string _publisServerHost;

        public MQPublishServerUrls(IConfiguration configuration)
        {
            _configuration = configuration;
            _publisServerHost = _configuration["MQPublishServer:Host"];
        }

        public string GetWeChatUrl() => $"{_publisServerHost}/api/wechat/publish?routingkey={0}&queuename={1}&exchangeName={2}";

        public string GetProductCreateUrl() => $"{_publisServerHost}/api/product/create";
    }
}