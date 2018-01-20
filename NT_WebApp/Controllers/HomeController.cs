using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace NT_WebApp.Controllers
{
    public class HomeController : Controller
    {
        public ViewResult Index() => View();

        public ViewResult Discount() => View();

        public ViewResult Foo() => View();

        public ViewResult Goods() => View();

        public ViewResult NT_Demo() => View();

        public ViewResult Order() => View();

        public ViewResult Rec() => View();

        public ViewResult Reserve() => View();

        public ViewResult Test() => View();

    }
}