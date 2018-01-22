using Microsoft.AspNetCore.Identity;

namespace NT_WebApp.Models
{
    public class AppUser : IdentityUser
    {
        public string NickName { get; set; }

        public int Sex { get; set; }
        
        public string City { get; set; }

        public string Province { get; set; }

        public string Country { get; set; }

        public WeChatInfo WeChatInfo { get; set; } 
    }
}