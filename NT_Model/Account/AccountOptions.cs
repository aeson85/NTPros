using System;
using Microsoft.AspNetCore.Server.IISIntegration;

namespace NT_Model.Account
{
    public class AccountOptions
    {
        public static bool AllowLocalLogin = true;

        public static bool AllowRemeberLogin = true;

        public static TimeSpan RememberLoginDuration = TimeSpan.FromDays(30);

        public static readonly string WindowsAuthenticationSchemeName = IISDefaults.AuthenticationScheme;

        public static string InvalidCredentialsErrorMessage = "Invalid username or password";
    }
}