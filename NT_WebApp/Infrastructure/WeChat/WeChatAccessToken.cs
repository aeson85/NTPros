using System;

namespace NT_WebApp.Infrastructure.WeChat
{
    public class WeChatAccessToken
    {
        public string AccessToken { get; set; }

        public int ExpiresIn { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}