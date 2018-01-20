using Microsoft.AspNetCore.Mvc;

namespace NT_WebApp.Controllers
{
    public class ThridAuthController : Controller
    {
        public string WechatCallback(string signature, string timestamp, string nonce, string echostr)
        {
            return echostr;
        }
    }
}