using System;

namespace NT_WeChatUtilities
{
    public class WeChatAccessToken
    {
        public string AccessToken { get; set; }

        public int ExpiresIn { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}