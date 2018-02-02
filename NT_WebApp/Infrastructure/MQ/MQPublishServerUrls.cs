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

        public string GetWeChatUrl() => $"{_publisServerHost}/api/wechat/publish?routingkey={{0}}&queuename={{1}}&exchangeName={{2}}";

        public string GetProductCreateUrl() => $"{_publisServerHost}/api/products/create";

        public string GetProductUpdateUrl() => $"{_publisServerHost}/api/products/update";

        public string GetProductSelectUrl() => $"{_publisServerHost}/api/products?name={{0}}&id={{1}}";

        public string GetProductDeleteUrl() => $"{_publisServerHost}/api/products/{{0}}";
    }
}