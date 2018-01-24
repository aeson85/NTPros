using System.ComponentModel;

namespace NT_WebApp.Infrastructure.WeChat
{
    public enum WeChatEventType
    {
        [Description("subscribe")]
        Subscribe,

        [Description("unsubscribe")]
        UnSubscribe
    }
    
    public class WeChatEventInfo
    {
        public WeChatEventType EventType { get; set; }

        public string EventKey { get; set; }
    }
}