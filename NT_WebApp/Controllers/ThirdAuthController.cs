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
        private readonly IDistributedCache _distributedCache;
        private readonly Func<string, MessageDelivery> _deliverServiceAccessor;

        public ThirdAuthController(IDistributedCache distributedCache, Func<string, MessageDelivery> deliverServiceAccessor)
        {
            _distributedCache = distributedCache;
            _deliverServiceAccessor = deliverServiceAccessor;
        }

        //[Produces("text/xml")]
        public IActionResult WechatCallback(string signature, string timestamp, string nonce, string echostr)
        {
            var responseText = echostr;
            if (this.Request.Method.Equals("post", StringComparison.OrdinalIgnoreCase))
            {
                _deliverServiceAccessor("wechat").Publish(this.Request.Body);
                //_deliverServiceAccessor("wechat").Deliver();
                //var weChatMsg = _weChatUtilities.Parse(this.Request.Body);
                //_messageDelivery.Deliver(weChatMsg);
                // if (weChatMsg.MsgType == WeChatMsgType.Event)
                // {
                //     await _weChatUtilities.GetUserInfo(weChatMsg.FromUserName);
                // }
            }
            else if (this.Request.Method.Equals("get", StringComparison.OrdinalIgnoreCase))
            {
                return Content(echostr, "application/plain");
            }
            return NotFound();
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