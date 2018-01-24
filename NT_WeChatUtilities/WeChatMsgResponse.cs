using System.Runtime.Serialization;

namespace NT_WeChatUtilities
{
    [DataContract(Name="xml", Namespace="")]
    public class WeChatMsgResponse
    {
        [DataMember]
        public string ToUserName { get; set; }

        [DataMember]
        public string FromUserName { get; set; }

        [DataMember]
        public long CreateTime { get; set; }

        [DataMember]
        public string MsgType { get; set; }

        [DataMember]
        public string Content { get; set; }
    }
}