using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConsoleApp1;

public class Runner
{
    private readonly ILogger<Runner> _logger;
    private readonly SmtpClient _client;

    public Runner(IConfigurationRoot configuration, ILogger<Runner> logger)
    {
        var ip = Debugger.IsAttached ? "127.0.0.1" : configuration.GetSection("IP").Value;
        var port = Debugger.IsAttached ? 25 : int.Parse(configuration.GetSection("Port").Value);
        _client = new SmtpClient(ip, port);
        _client.Credentials = new NetworkCredential("  ", "fakeSmtpPassword");
        _logger = logger;
    }

    public void DoAction()
    {
        var targetFrameworkAttribute = Assembly.GetExecutingAssembly()
            .GetCustomAttributes<TargetFrameworkAttribute>()
            .SingleOrDefault();
        
        try
        {
            _client.Send(
                "communication@gms.vialto.com",
                "helloworld@tw.com",
                $"test now:{DateTime.Now} with {targetFrameworkAttribute?.FrameworkName}",
                $"<html xmlns='http://www.w3.org/1999/xhtml' >{DateTime.Now}</html>");
            Console.WriteLine($"{DateTime.Now}: send email from {_client.Host}:{_client.Port}");
        }
        catch (Exception e)
        {
            var messages = new[]
            {
                "No connection could be made because the target machine actively refused it. [::ffff:127.0.0.1]:25", // stop
                "Unable to write data to the transport connection: An existing connection was forcibly closed by the remote host.." // restart
            };
            if (e is SmtpException exception && messages.Contains(exception.InnerException?.Message))
            {
                Console.WriteLine(e.Message);
                return;
            }

            throw;
        }
    }
}