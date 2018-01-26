using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NT_Common.Extensions;

namespace NT_WeChatUtilities
{
    public class WeChatUtilities
    {
        private readonly WeChatApiUrls _weChatApiUrls;
        private readonly IConfiguration _congiguration;
        private readonly string _url;

        private static HttpClient client = new HttpClient();

        public WeChatUtilities(WeChatApiUrls weChatApiUrls, IConfiguration congiguration)
        {
            _weChatApiUrls = weChatApiUrls;
            _congiguration = congiguration;
            _url = _congiguration["CacheServer:Url"];
        }

        private async Task GetFromWechatServer(string url, Action<JObject> action = null)
        {
            var responseText = string.Empty;
            var response = await client.GetAsync(url);
            responseText = await response.Content.ReadAsStringAsync();
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

        private async Task<string> GetFromCacheServer(string key, Action<JObject> action = null)
        {
            var url = _url + $"/{key}";
            var jsonStr = await client.GetStringAsync(url);
            var jobj = JObject.FromObject(JsonConvert.DeserializeObject(jsonStr));
            var accessToken = string.Empty;
            if (jobj.TryGetValue("ErrorObj", out var errorVal))
            {
                throw new Exception($"请求缓存服务器出错: {errorVal}");
            }
            else
            {
                accessToken = jobj["Value"].Value<string>();
            }
            return accessToken == "null" ? string.Empty : accessToken;
        }

        private Task PostDataToCacheServer(string key, WeChatAccessToken value)
        {
            var cacheObj = new {
                Key = key,
                 Value = JsonConvert.SerializeObject(value),
                 Options = new
                 {
                     AbsoluteExpirationRelativeToNow = value.ExpiresAt.TimeOfDay
                 }
            };
            return client.PostAsJsonAsync(_url, cacheObj);
        }

        public async Task<string> GetAccessToken()
        {
            string key = "WeChatAccessToken";
            var accessTokenResultStr = await this.GetFromCacheServer(key);
            WeChatAccessToken accessTokenResult = null;
            Action<JObject> callback = jObject =>
            {
                var expiresAt = DateTime.Now.AddHours(1.5);
                accessTokenResult = new WeChatAccessToken
                {
                    AccessToken = jObject.GetValue("access_token").Value<string>(),
                    ExpiresIn = jObject.GetValue("expires_in").Value<int>(),
                    ExpiresAt = expiresAt
                };
                this.PostDataToCacheServer(key, accessTokenResult);
            };
            if (!string.IsNullOrEmpty(accessTokenResultStr))
            {
                accessTokenResult = JsonConvert.DeserializeObject<WeChatAccessToken>(accessTokenResultStr);
            }
            else
            {
                await this.GetFromWechatServer(_weChatApiUrls.GetAcessTokenUrl(), callback);
            }
            return accessTokenResult?.AccessToken;
        }

        public async Task GetUserInfo(string openId)
        {
            var token = await this.GetAccessToken();
            await this.GetFromWechatServer(_weChatApiUrls.GetUserBasicInfoUrl(token, openId), jObject => 
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
                        EventKey = root.Element("EventKey").Value,
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
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}