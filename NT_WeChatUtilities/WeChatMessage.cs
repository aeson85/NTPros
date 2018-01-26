using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NT_WeChatUtilities
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WeChatMsgType
    {
        [EnumMember(Value = "none")]
        None,
        
        [EnumMember(Value = "text")]
        Text,

        [EnumMember(Value = "event")]
        Event,
    }
    
    public class WeChatMessage
    {
        public string MsgId { get; set; }

        public string FromUserName { get; set; }

        public string ToUserName { get; set; }

        public long CreateTime { get; set; }

        public string Content { get; set; }

        public WeChatMsgType MsgType { get; set; }

        public WeChatEventInfo EventInfo { get; set; }
    }
}