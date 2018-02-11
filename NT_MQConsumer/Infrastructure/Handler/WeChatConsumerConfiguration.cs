using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NT_CommonConfig.Infrastructure;
using NT_WeChatUtilities;
using RabbitMQ.Client;
using NT_Common.Extensions;
using AutoMapper;
using NT_Model.ViewModel;

namespace NT_MQConsumer.Infrastructure.Handler
{
    public class WeChatConsumerHandler : IConsumerHandler
    {
        private readonly IConfiguration _configuration;
        private readonly WeChatUtilities _weChatUtilities;
        private readonly MQPublishServerUrls _mqPublishServerUrls;
        private readonly IMapper _mapper;

        public string RoutingKey => "wechat";

        public string QueueName => _configuration["RabbitMQ:Wechat:QueueName"];
        
        public WeChatConsumerHandler(IConfiguration configuration, WeChatUtilities weChatUtilities, MQPublishServerUrls mqPublishServerUrls,IMapper mapper)
        {
            _configuration = configuration;
            _weChatUtilities = weChatUtilities;
            _mqPublishServerUrls = mqPublishServerUrls;
            _mapper = mapper;
        }

        public async void Execute(string message)
        {
            var weChatMsg = JsonConvert.DeserializeObject<WeChatMessage>(message);
            if (weChatMsg.MsgType == WeChatMsgType.Event)
            {
                switch (weChatMsg.EventInfo.EventType)
                {
                    case WeChatEventType.Subscribe:
                    case WeChatEventType.UnSubscribe:
                    {
                        var basicUserInfo = await _weChatUtilities.GetUserInfo(weChatMsg.FromUserName);
                        this.SaveToDb(basicUserInfo);
                    }
                    break;
                }
            }
        }

        private WeChatUserBasicInfo GetTestWeChatUserBasicInfo()
        {
            var basicUserInfo = new WeChatUserBasicInfo
            {
                Subscribe = 0,
                OpenId = "orW9-jh1BQUU40t5yqnPH7SDHhzs",
                NickName = "大mm",
                Sex = 1,
                Language = "zh-cn",
                City = "成都",
                Province = "四川",
                Country = "中国",
                HeadImgUrl = "http://wx.qlogo.cn/mmopen/g3MonUZtNHkdmzicIlibx6iaFqAc56vxLSUfpb6n5WKSYVY0ChQKkiaJSgQ1dZuTOgvLLrhJbERQQ4eMsv84eavHiaiceqxibJxCfHe/0",
                Subscribe_Time = 1382694957,
                Unionid = "o6_bmasdasdsad6_2sgVt7hMZOPfL",
                Remark = "Remark",
                GroupId = 0,
                Tagid_List = new int[] { 128, 2 }
            };
            return basicUserInfo;
        }
        private async void SaveToDb(WeChatUserBasicInfo weChatUserBasicInfo)
        {
            var url = _mqPublishServerUrls.GetWechatUserInfoSaveUrl();
            using (var client = new HttpClient())
            {
                var appUserViewModel = _mapper.Map<AppUserViewModel>(weChatUserBasicInfo);
                var response = await client.PostAsJsonAsync(url, appUserViewModel);
            }
        }
    }
}