using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace NT_Model.Entity
{
    public class WeChatInfo
    {
        [MaxLength(256)]
        public string Id { get; set;}
        
        public AppUser Owner { get; set; }

        public string OwnerId { get; set; }

        public string OpenId { get; set; }

        public int Subscribe { get; set; }

        public string Language { get; set; }
        
        public string HeadImgUrl { get; set; }

        public int SubscribeTime { get; set; }

        public string Unionid { get; set; }

        public string Remark { get; set; }

        public int GroupId { get; set; }

        public string TagIdList { get; set; }
    }
}