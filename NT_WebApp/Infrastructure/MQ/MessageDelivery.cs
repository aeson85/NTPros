using Microsoft.Extensions.Configuration;
using System;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using NT_Common.Extensions;
using System.Net;

namespace NT_WebApp.Infrastructure
{
    public abstract class MessageDelivery
    {
        private static HttpClient client = new HttpClient();
        private readonly IConfiguration _configuration;

        protected abstract string QueueName { get; }
        protected abstract string RoutingKey { get; }

        public MessageDelivery(IConfiguration configuration) => _configuration = configuration;

        protected string GetPublishServerUrl()
        {
            var url = _configuration["MQPublishServer:Url"];
            var exchangeName = _configuration["RabbitMQ:ExchangeName"];
            url = string.Format(url, this.RoutingKey, this.QueueName, exchangeName);
            return url;
        }

        public async void Publish(object message)
        {
            var url = this.GetPublishServerUrl();
            try
            {
                var response = await client.PostAsJsonAsync(url, message);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"消息发布服务器处理异常,地址:{url}");
                }
            }
            catch (HttpRequestException)
            {
                throw new Exception($"消息发布服务器无法连接,地址:{url}");
            }
        }

        public abstract T GetMessage<T>(Stream stream) where T : class;
    }
}