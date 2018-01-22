using System;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using NT_WebApp.Infrastructure.WeChat;

namespace NT_WebApp.Controllers
{
    public class ThirdAuthController : Controller
    {
        private readonly IDistributedCache _distributedCache;
        private readonly WeChatUtilities _weChatUtilities;

        public ThirdAuthController(IDistributedCache distributedCache, WeChatUtilities weChatUtilities)
        {
            _distributedCache = distributedCache;
            _weChatUtilities = weChatUtilities;
        }

        //[Produces("text/xml")]
        public IActionResult WechatCallback(string signature, string timestamp, string nonce, string echostr)
        {
            var responseText = echostr;
            if (this.Request.Method.Equals("post", StringComparison.OrdinalIgnoreCase))
            {
                var xDoc = XDocument.Load(this.Request.Body);
                var root = xDoc.Element("xml");
                var toUserName = root.Element("ToUserName")?.Value;
                var fromUserName = root.Element("FromUserName")?.Value;
                var createTime = root.Element("CreateTime")?.Value;
                var msgType = root.Element("MsgType")?.Value;
                var content = root.Element("Content")?.Value;
                var msgId = root.Element("MsgId")?.Value;
                var responseTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                responseText = $"<xml><ToUserName><![CDATA[{fromUserName}]]></ToUserName><FromUserName>< ![CDATA[{toUserName}]]></FromUserName> <CreateTime>{responseTime.ToString()}</CreateTime> <MsgType><![CDATA[text]]></MsgType><Content>< ![CDATA[老B,敢不敢来喝两杯!!]]></Content></xml>";
                return new ContentResult {
                    Content = responseText,
                    ContentType = "application/xml",
                    StatusCode = 200
                };
                // return new WeChatMsgResponse
                // {
                //     ToUserName = fromUserName,
                //     FromUserName = toUserName,
                //     CreateTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
                //     MsgType = "text",
                //     Content = "老B,敢不敢来喝两杯!!"
                // };
                //return responseText;
            }
            return Content(responseText, "application/plain");
        }

        [HttpGet]
        public async Task<string> GetWeChatAccessToken()
        {
            var token = await _weChatUtilities.GetAccessToken();
            return token;
        }
    }
}