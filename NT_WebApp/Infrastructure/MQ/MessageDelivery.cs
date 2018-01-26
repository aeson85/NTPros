using Microsoft.Extensions.Configuration;
using System;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using NT_Common.Extensions;

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

        public void Publish(object message)
        {
            var url = this.GetPublishServerUrl();
            //var client = new HttpClient();
            client.PostAsJsonAsync(url, message);
        }

        public abstract T GetMessage<T>(Stream stream) where T : class;
    }
}