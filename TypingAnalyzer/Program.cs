using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TypingAnalyzer.Keyboard;
using TypingAnalyzer.Interfaces;
using TypingAnalyzer.Window;

namespace TypingAnalyzer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            IConfiguration configuration = new ConfigurationBuilder()
                .AddXmlFile("analyzer.config", true, true)
                .Build();

            var services = new ServiceCollection();

            ConfigureServices(services, configuration);

            var provider = services.BuildServiceProvider();
            var analyzer = provider.GetRequiredService<Analyzer>();

            Log.Information($"Application initialized with {services.Count} services. Starting GUI.");
            Application.Run(new HostForm(analyzer));
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                            .WriteTo.File(configuration.GetValue("Logger:LogFile", "analyzer.log"))
                            .WriteTo.Console()
                            .MinimumLevel.Is(Enum.TryParse<LogEventLevel>(configuration.GetValue("Logging:LogLevel", "Information"), out var level) ? level : LogEventLevel.Information)
                            .CreateLogger();

            services.AddLogging(c => c.AddSerilog());
            services.AddSingleton<ILoggerFactory>(s => new SerilogLoggerFactory(Log.Logger));
            services.AddSingleton(configuration);

            services.AddTransient<IGlobalKeyboardHook, GlobalKeyboardHook>();
            services.AddTransient<IActiveWindowHandler, ActiveWindowHandler>();
            services.AddTransient<Analyzer>();
        }
    }
}
