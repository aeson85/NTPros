using System.Collections.Generic;
using System.Linq;

namespace NT_Model.Account
{
    public class LoginViewModel : LoginInputViewModel
    {
        public bool AllowRememberLogin { get; set; }

        public bool EnableLocalLogin { get; set; }

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; }

        public IEnumerable<ExternalProvider> VisibleExternalProviders => this.ExternalProviders.Where(p => !string.IsNullOrWhiteSpace(p.DisplayName));

        public bool IsExternalLoginOnly => !this.EnableLocalLogin && this.VisibleExternalProviders.Count() == 1;

        public string ExternalLoginScheme => this.IsExternalLoginOnly ? this.ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;
    }
}