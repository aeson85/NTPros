using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NT_WebApp.Controllers
{
    //[Authorize]
    public class AdminController : Controller
    {
        public ViewResult Index() => View();

        public ViewResult Home() => View();

        //public ViewResult Login() => View();

        public ViewResult Member() => View();

        public ViewResult Order() => View();

        public ViewResult Res() => View();
		
		public ViewResult NT_Images() => View();
    }
}