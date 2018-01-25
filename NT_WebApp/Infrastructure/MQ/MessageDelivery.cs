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

        public void Publish(Stream stream)
        {
            var url = this.GetPublishServerUrl();
            using (var client = new HttpClient())
            {
                var body = this.Process(stream);
                var a = client.PostAsJsonAsync(url, body).Result;
            }
        }

        protected abstract object Process(Stream stream);
    }
}