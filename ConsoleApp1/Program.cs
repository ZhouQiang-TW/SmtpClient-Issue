using System.Diagnostics;
using System.Runtime.InteropServices;
using ConsoleApp1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var logger = LogManager.GetCurrentClassLogger();

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var servicesProvider = new ServiceCollection()
    .AddSingleton(config)
    .AddSingleton<Runner>()
    .AddLogging(loggingBuilder =>
    {
        loggingBuilder.ClearProviders();
        loggingBuilder.SetMinimumLevel(LogLevel.Trace);
        loggingBuilder.AddNLog(config);
    });

try
{
    var runner = servicesProvider.BuildServiceProvider().GetRequiredService<Runner>();

    var interval = Debugger.IsAttached ? 2 : int.Parse(config.GetSection("Interval").Value);
    var timer = new System.Timers.Timer();
    timer.Enabled = true;
    timer.Interval = TimeSpan.FromSeconds(interval).TotalMilliseconds;
    timer.Elapsed += (sender, e) => runner.DoAction();
    Console.WriteLine("Press ANY key to exit");
    Console.ReadKey();
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}