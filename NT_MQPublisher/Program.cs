using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NT_CommonConfig.Infrastructure;

namespace NT_MQPublisher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Meesage Publish Server,PID: " +　Process.GetCurrentProcess().Id;
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
            .UseStartup<Startup>()
            .UseUrls("http://localhost:5001")
            .Build();
        }
    }
}
