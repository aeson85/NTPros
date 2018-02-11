using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using NT_WebApp.Infrastructure;
using NT_WeChatUtilities;

namespace NT_WebApp.Controllers
{
    public class ThirdAuthController : Controller
    {
        private readonly Func<string, MessageDelivery> _deliverServiceAccessor;

        public ThirdAuthController(Func<string, MessageDelivery> deliverServiceAccessor) => _deliverServiceAccessor = deliverServiceAccessor;

        public void WeChatCallback(string signature, string timestamp, string nonce, string echostr)
        {
            var responseText = echostr;
            var contentType = "text/plain";
            if (this.Request.Method.Equals("post", StringComparison.OrdinalIgnoreCase))
            {
                var deliverHandler = _deliverServiceAccessor("wechat");
                var weChatMsg = deliverHandler.GetMessage<WeChatMessage>(this.Request.Body);
                deliverHandler.Publish(weChatMsg);
                responseText = $"<xml><ToUserName><![CDATA[{weChatMsg.FromUserName}]]></ToUserName><FromUserName><![CDATA[{weChatMsg.ToUserName}]]></FromUserName><CreateTime>{weChatMsg.CreateTime}</CreateTime> <MsgType><![CDATA[text]]></MsgType><Content><![CDATA[你好,欢迎来到老B之家！！！]]></Content> </xml>";
                contentType = "application/xml";
            }
            this.Response.ContentType = contentType;
            this.Response.WriteAsync(responseText);
        }

        [HttpGet]
        public string GetWeChatAccessToken()
        {
            return string.Empty;
            //var token = await _weChatUtilities.GetAccessToken();
            //return token;
        }
    }
}