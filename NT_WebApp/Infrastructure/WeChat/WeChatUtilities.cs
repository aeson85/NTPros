using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NT_WebApp.Infrastructure.WeChat
{
    public class WeChatUtilities
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConfiguration _configuration;

        public WeChatUtilities(IDistributedCache distributedCache, IConfiguration configuration)
        {
            _distributedCache = distributedCache;
            _configuration = configuration;
        }

        public async Task<string> GetAccessToken()
        {
            string key = "WeChatAccessToken";
            var accessTokenResultStr = await _distributedCache.GetStringAsync(key);
            WeChatAccessToken accessTokenResult = null;
            if (!string.IsNullOrEmpty(accessTokenResultStr))
            {
                accessTokenResult = JsonConvert.DeserializeObject<WeChatAccessToken>(accessTokenResultStr);
            }
            else
            {
                var appId = _configuration["WeChat:AppID"];
                var appSecret = _configuration["WeChat:AppSecret"];
                var url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appId}&secret={appSecret}";
                var responseText = string.Empty;
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    responseText = await response.Content.ReadAsStringAsync();
                }
                if (!string.IsNullOrEmpty(responseText))
                {
                    var jObject = JObject.Parse(responseText);
                    if (jObject.TryGetValue("errcode", out var jErrorCode))
                    {
                        var errorCode = jErrorCode.Value<int>();
                        var errorMsg = jObject.GetValue("errmsg").Value<string>();
                        throw new Exception($"微信请求发生错误！错误代码：{errorCode.ToString()}，说明：{errorMsg}");
                    }
                    else
                    {
                        var expiresAt = DateTime.Now.AddHours(1.5);
                        accessTokenResult = new WeChatAccessToken
                        {
                            AccessToken = jObject.GetValue("access_token").Value<string>(),
                            ExpiresIn = jObject.GetValue("expires_in").Value<int>(),
                            ExpiresAt = expiresAt
                        };
                        await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(accessTokenResult), new DistributedCacheEntryOptions {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromTicks(expiresAt.Ticks)
                        });
                    }
                }
                else
                {
                    throw new Exception("获取Access_Token微信服务器返回空字符串");
                }
            }
            return accessTokenResult?.AccessToken;
        }
    }
}