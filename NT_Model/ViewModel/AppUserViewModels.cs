using Microsoft.AspNetCore.Identity;
using NT_Model.Entity;

namespace NT_Model.ViewModel
{
    public class AppUserViewModel
    {
        public string Id { get; set; }
        
        public string NickName { get; set; }

        public int Sex { get; set; }
        
        public string City { get; set; }

        public string Province { get; set; }

        public string Country { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }
        
        public WeChatInfo WeChatInfo { get; set; } 
    }

    public class AppUserSearchViewModel
    {
        public string Id { get; set; }

        public WeChatInfo WeChatInfo { get; set; }
    }
}