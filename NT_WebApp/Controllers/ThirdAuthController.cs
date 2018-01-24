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
        public async Task<IActionResult> WechatCallback(string signature, string timestamp, string nonce, string echostr)
        {
            var responseText = echostr;
            if (this.Request.Method.Equals("post", StringComparison.OrdinalIgnoreCase))
            {
                var weChatMsg = _weChatUtilities.Parse(this.Request.Body);
                if (weChatMsg.MsgType == WeChatMsgType.Event)
                {
                    await _weChatUtilities.GetUserInfo(weChatMsg.FromUserName);
                }
            }
            else if (this.Request.Method.Equals("get", StringComparison.OrdinalIgnoreCase))
            {
                return Content(echostr, "application/plain");
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<string> GetWeChatAccessToken()
        {
            var token = await _weChatUtilities.GetAccessToken();
            return token;
        }
    }
}