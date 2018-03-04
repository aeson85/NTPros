
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using NT_Model.Entity;
using NT_Model.ViewModel;
using NT_WeChatUtilities;

namespace NT_CommonConfig.Infrastructure
{
    public class WeChatProfile : Profile
    {
        public WeChatProfile(IConfiguration configuration)
        {
            this.CreateMap<WeChatUserBasicInfo, WeChatInfo>().ForMember(p => p.TagIdList, opt => opt.MapFrom(p => p.Tagid_List != null ? string.Join(",", p.Tagid_List) : string.Empty)).ForMember(p => p.SubscribeTime, opt => opt.MapFrom(p => p.Subscribe_Time)).ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null));
            
            this.CreateMap<WeChatUserBasicInfo, AppUserViewModel>().ForMember(p => p.City, opt => opt.MapFrom(p => p.City)).ForMember(p => p.NickName, opt => opt.MapFrom(p => p.NickName)).ForMember(p => p.Province, opt => opt.MapFrom(p => p.Province)).ForMember(p => p.Country, opt => opt.MapFrom(p => p.Country)).ForMember(p => p.Sex, opt => opt.MapFrom(p => p.Sex)).ForMember(p => p.WeChatInfo, opt => opt.MapFrom(p => p)).ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null));
        }
    }
}