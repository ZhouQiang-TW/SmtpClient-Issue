using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConsoleApp1;

public class RunnerV2 : IRunner
{
    private readonly ConcurrentQueue<SmtpClient> _smtpClients;
    private readonly ILogger<RunnerV2> _logger;
    private readonly string _ip;
    private readonly int _port;

    public RunnerV2(IConfiguration configuration, ILogger<RunnerV2> logger)
    {
        _ip = Debugger.IsAttached ? "127.0.0.1" : configuration.GetSection("IP").Value;
        _port = Debugger.IsAttached ? 25 : int.Parse(configuration.GetSection("Port").Value);
        _logger = logger;
        _smtpClients = new ConcurrentQueue<SmtpClient>(Enumerable.Range(0, 2).Select(x => new SmtpClient(_ip, _port)
        {
            
        }));
    }

    public void DoAction()
    {
        var targetFrameworkAttribute = Assembly.GetExecutingAssembly()
            .GetCustomAttributes<TargetFrameworkAttribute>()
            .SingleOrDefault();

        Parallel.ForEach(Enumerable.Range(0, 2000), (i) =>
        {
            try
            {
                Console.WriteLine(i);
                if (!_smtpClients.TryDequeue(out var client))
                    return;

                client.Credentials = new NetworkCredential("  ", "fakeSmtpPassword");
                client.Send(
                    "communication@outlook.com",
                    "helloworld@outlook.com",
                    $"{i}:{client.GetHashCode()} -> test now:{DateTime.Now} with {targetFrameworkAttribute?.FrameworkName}",
                    $"<html xmlns='http://www.w3.org/1999/xhtml' >{DateTime.Now}</html>");
                Console.WriteLine($"{DateTime.Now}: send email from {client.Host}:{client.Port}");
                _smtpClients.Enqueue(client);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        });
    }
}