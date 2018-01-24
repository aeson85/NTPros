using System.ComponentModel;

namespace NT_WeChatUtilities
{
    public enum WeChatMsgType
    {
        [Description("text")]
        Text,

        [Description("event")]
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