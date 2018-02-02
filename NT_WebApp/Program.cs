using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal;
using NT_CommonConfig.Infrastructure;
using System.Diagnostics;

namespace NT_WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "WebApp,PID: " +　Process.GetCurrentProcess().Id;
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return new WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;
                ConfigurationSettings.Initial(hostingContext, config, args, env.ContentRootPath);
                /*
                var env = hostingContext.HostingEnvironment;
                var settingPath = Path.GetFullPath(Path.Combine(@"../NT_Common/globalSettings.json"));

                config.AddJsonFile(path: settingPath, optional: true, reloadOnChange: true);
                config.SetBasePath(env.ContentRootPath).AddJsonFile(path:"appsettings.json", optional: true, reloadOnChange: true).AddJsonFile(path: $"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                if (env.IsDevelopment()) 
                {
                    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                    if (appAssembly != null) 
                    {
                        config.AddUserSecrets(appAssembly, optional: true);
                    }
                }
                config.AddEnvironmentVariables();

                if (args != null) 
                {
                    config.AddCommandLine(args);
                }
                */
            })
            .ConfigureLogging((hostingContext, logging) => 
            {
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                //logging.AddConsole();
                //logging.AddDebug();
            })
            .UseIISIntegration()
            .UseDefaultServiceProvider((context, options) => 
            {
                options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
            })
            .ConfigureServices(services =>
            {
                services.AddTransient<IConfigureOptions<KestrelServerOptions>, KestrelServerOptionsSetup>();
            })
            .UseStartup<Startup>()
            .UseUrls("http://localhost:5000")
            .Build();
        }
    }
}
