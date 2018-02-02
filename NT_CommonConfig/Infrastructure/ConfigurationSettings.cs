using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace NT_CommonConfig.Infrastructure
{
    public class ConfigurationSettings
    {
        public static IConfigurationBuilder Initial(WebHostBuilderContext hostingContext, IConfigurationBuilder config, string[] args, string appJsonPath)
        {
            config = config ?? new ConfigurationBuilder();
            var env = hostingContext?.HostingEnvironment;
            var settingPath = Path.GetFullPath(Path.Combine(@"../NT_CommonConfig/globalSettings.json"));

            config.AddJsonFile(path:settingPath, optional: true, reloadOnChange: true);
            config.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            if (env != null)
            {
                config.SetBasePath(appJsonPath).AddJsonFile(path: $"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
            }
            config.AddUserSecrets(Assembly.Load(new AssemblyName("nt_common")), optional: true);
            config.AddEnvironmentVariables();
            
            return config;
        }

        public static IConfigurationBuilder Initial(string appJsonPath)
        {
            return ConfigurationSettings.Initial(null, null, null, appJsonPath);
        }
    }
}