using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConsoleApp1;

public class RunnerV1 : IRunner
{
    private readonly ILogger<RunnerV1> _logger;
    private readonly string _ip;
    private readonly int _port;

    public RunnerV1(IConfiguration configuration, ILogger<RunnerV1> logger)
    {
        _ip = Debugger.IsAttached ? "127.0.0.1" : configuration.GetSection("IP").Value;
        _port = Debugger.IsAttached ? 25 : int.Parse(configuration.GetSection("Port").Value);
        _logger = logger;
    }

    public void DoAction()
    {
        Parallel.ForEach(Enumerable.Range(0, 2000), (i) =>
        {
            var targetFrameworkAttribute = Assembly.GetExecutingAssembly()
                .GetCustomAttributes<TargetFrameworkAttribute>()
                .SingleOrDefault();

           // var evtListener = new MyEventListener();

            try
            {
                using var client = new SmtpClient(_ip, _port);
                client.Credentials = new NetworkCredential("  ", "fakeSmtpPassword");
                client.Send(
                    "communication@outlook.com",
                    "helloworld@outlook.com",
                    $"{client.GetHashCode()} -> test now:{DateTime.Now} with {targetFrameworkAttribute?.FrameworkName}",
                    $"<html xmlns='http://www.w3.org/1999/xhtml' >{DateTime.Now}</html>");
                Console.WriteLine($"{DateTime.Now}: send email from {client.Host}:{client.Port}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
            finally
            {
                //evtListener.Dispose();
            }
        });
    }
}