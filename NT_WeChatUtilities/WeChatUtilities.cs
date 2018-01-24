using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NT_Common.Extensions;

namespace NT_WeChatUtilities
{
    public class WeChatUtilities
    {
        private readonly IDistributedCache _distributedCache;
        private readonly WeChatApiUrls _weChatApiUrls;
        
        private async void Get(string url, Action<JObject> action = null)
        {
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
                    action?.Invoke(jObject);
                }
            }
            else
            {
                throw new Exception("微信服务器返回空字符串");
            }
        }

        public WeChatUtilities(IDistributedCache distributedCache, WeChatApiUrls weChatApiUrls)
        {
            _distributedCache = distributedCache;
            _weChatApiUrls = weChatApiUrls;
        }

        public async Task<string> GetAccessToken()
        {
            string key = "WeChatAccessToken";
            var accessTokenResultStr = await _distributedCache.GetStringAsync(key);
            WeChatAccessToken accessTokenResult = null;
            Action<JObject> callback = async jObject =>
            {
                var expiresAt = DateTime.Now.AddHours(1.5);
                accessTokenResult = new WeChatAccessToken
                {
                    AccessToken = jObject.GetValue("access_token").Value<string>(),
                    ExpiresIn = jObject.GetValue("expires_in").Value<int>(),
                    ExpiresAt = expiresAt
                };
                await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(accessTokenResult), new DistributedCacheEntryOptions {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1.5)
                });
            };
            if (!string.IsNullOrEmpty(accessTokenResultStr))
            {
                accessTokenResult = JsonConvert.DeserializeObject<WeChatAccessToken>(accessTokenResultStr);
            }
            else
            {
                this.Get(_weChatApiUrls.GetAcessTokenUrl(), callback);
            }
            return accessTokenResult?.AccessToken;
        }

        public async Task GetUserInfo(string openId)
        {
            var token = await this.GetAccessToken();
            this.Get(_weChatApiUrls.GetUserBasicInfoUrl(token, openId), jObject => 
            {

            });
        }

        public WeChatMessage Parse(Stream requestStream)
        {
            try
            {
                var xDoc = XDocument.Load(requestStream);
                var root = xDoc.Element("xml");
                var toUserName = root.Element("ToUserName").Value;
                var fromUserName = root.Element("FromUserName").Value;
                var createTime = long.Parse(root.Element("CreateTime").Value);
                var msgType = root.Element("MsgType").Value.ToEnum<WeChatMsgType>();
                var content = root.Element("Content")?.Value;
                var msgId = root.Element("MsgId")?.Value;
                WeChatEventInfo eventInfo = null;
                if (msgType == WeChatMsgType.Event)
                {
                    eventInfo = new WeChatEventInfo
                    {
                        EventKey = root.Element("").Value,
                        EventType = root.Element("Event").Value.ToEnum<WeChatEventType>()
                    };
                }
                return new WeChatMessage 
                {
                    MsgId = msgId,
                    ToUserName = toUserName,
                    FromUserName = fromUserName,
                    CreateTime = createTime,
                    MsgType = msgType,
                    Content = content,
                    EventInfo = eventInfo
                };
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}