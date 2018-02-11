using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NT_Model.Account;
using NT_Model.Entity;
using Microsoft.AspNetCore.Http;

namespace NT_Auth.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IClientStore _clientStore;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEventService _events;

        public AccountController(
            IIdentityServerInteractionService interaction,
            IAuthenticationSchemeProvider schemeProvider,
            IClientStore clientStore,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IEventService events)
        {
            _interaction = interaction;
            _schemeProvider = schemeProvider;
            _clientStore = clientStore;
            _userManager = userManager;
            _signInManager = signInManager;
            _events = events;
        }

        public async Task<IActionResult> Login(string returnUrl)
        {
            var loginViewModel = await this.BuildLoginViewModelAsync(returnUrl);
            
            if (loginViewModel.IsExternalLoginOnly)
            {
            }

            return View(loginViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputViewModel model, string button)
        {
            if (ModelState.IsValid)
            {
                await _signInManager.SignOutAsync();
                var user = await _userManager.FindByNameAsync(model.Username);
                var identityResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (identityResult.Succeeded)
                {
                    //await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName));
                    
                    AuthenticationProperties properties = null;
                    if (AccountOptions.AllowRemeberLogin && model.RememberLogin)
                    {
                        properties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberLoginDuration)
                        };
                    }
                    await HttpContext.SignInAsync(user.Id, user.UserName, properties);

                    return Redirect(model.ReturnUrl);
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]LoginInputViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.Username
                };
                var identityResult = await _userManager.CreateAsync(user, model.Password);
                if (identityResult.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return Json(identityResult.Errors);
                }
            }
            return Json(ModelState);
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null)
            {
                return new LoginViewModel
                {
                    EnableLocalLogin = false,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                    ExternalProviders = new ExternalProvider[] { new ExternalProvider { AuthenticationScheme = context.IdP } }
                };
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(p => p.DisplayName != null || 
                       (p.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase))
                )
                .Select(p => new ExternalProvider 
                {
                    DisplayName = p.DisplayName,
                    AuthenticationScheme = p.Name
                });

            var allowLocal = true;
            if (context?.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;
                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(p => client.IdentityProviderRestrictions.Contains(p.AuthenticationScheme));
                    }
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRemeberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers
            };
        }
    }
}