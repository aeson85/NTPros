using System.Threading.Tasks;
using IdentityServer4.Services;
using NT_Model.Account;
using Microsoft.AspNetCore.Mvc;

namespace NT_Auth.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;

        public HomeController(IIdentityServerInteractionService interaction)
        {
            _interaction = interaction;
        }

    
        public async Task<IActionResult> Error(string errorId)
        {
            var errorViewModel = new ErrorViewModel();
            errorViewModel.Error = await _interaction.GetErrorContextAsync(errorId);
            return View("Error", errorViewModel);
        }
    }
}