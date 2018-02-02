using System;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NT_Common.Extensions
{
    public class JsonContent : StringContent
    {
        public JsonContent(object obj) : base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
        { }
    }

    public static class HttpClientExtesion
    {
        public static Task<HttpResponseMessage> PostAsJsonAsync(this HttpClient client, string url, object obj)
        {
            return client.PostAsync(url, new JsonContent(obj));
        } 

        public static Task<HttpResponseMessage> PutAsJsonAsync(this HttpClient client, string url, object obj)
        {
            return client.PutAsync(url, new JsonContent(obj));
        } 
    }
}