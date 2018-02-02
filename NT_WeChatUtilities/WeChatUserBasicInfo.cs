using System.Collections.Generic;

namespace NT_WeChatUtilities
{
    public class WeChatUserBasicInfo
    {
        public int Subscribe { get; set; }

        public string OpenId { get; set; }

        public string NickName { get; set; }

        public int Sex { get; set; }

        public string Language { get; set; }

        public string City { get; set; }

        public string Province { get; set; }

        public string Country { get; set; }
        
        public string HeadImgUrl { get; set; }

        public int Subscribe_Time { get; set; }

        public string Unionid { get; set; }

        public string Remark { get; set; }

        public int GroupId { get; set; }

        public int[] Tagid_List { get; set; }
    }
}