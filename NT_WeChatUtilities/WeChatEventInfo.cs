using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NT_WeChatUtilities
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WeChatEventType
    {
        [EnumMember(Value = "none")]
        None,

        [EnumMember(Value = "subscribe")]
        Subscribe,

        [EnumMember(Value = "unsubscribe")]
        UnSubscribe
    }
    
    public class WeChatEventInfo
    {
        public WeChatEventType EventType { get; set; }

        public string EventKey { get; set; }
    }
}