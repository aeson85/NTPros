using Microsoft.Extensions.Configuration;

namespace NT_WeChatUtilities
{
    public class WeChatApiUrls
    {
        private readonly IConfiguration _configuration;

        private readonly string _appId;

        private readonly string _appSecret;

        public IConfiguration Configuration => _configuration;

        public string AppId => _appId;

        public string AppSecret => _appSecret;

        public WeChatApiUrls(IConfiguration configuration)
        {
            _configuration = configuration;
            _appId = _configuration["WeChat:AppID"];
            _appSecret = _configuration["WeChat:AppSecret"];
        }

        public string GetAcessTokenUrl() => $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={_appId}&secret={_appSecret}";

        public string GetUserBasicInfoUrl(string accessToken = "", string openId = "") => $"https://api.weixin.qq.com/cgi-bin/user/info?access_token={accessToken}&openid={openId}&lang=zh_CN";
    }
}