using System.Diagnostics.Tracing;

namespace ConsoleApp1;

public class MyEventListener : EventListener
{
    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        if (eventSource.Name == "System.Net.Sockets")
        {
            EnableEvents(eventSource, EventLevel.LogAlways);
            Console.WriteLine("enabled for sockets event source");
        }

        if (eventSource.Name == "Private.InternalDiagnostics.System.Net.Mail")
        {
            EnableEvents(eventSource, EventLevel.LogAlways);
            Console.WriteLine("enabled for mail net event source");
        }

        if (eventSource.Name == "Private.InternalDiagnostics.System.Net.Sockets")
        {
            EnableEvents(eventSource, EventLevel.LogAlways);
            Console.WriteLine("enabled for sockets net event source");
        }
    }

    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        if (eventData.Payload != null)
            Console.WriteLine(
                $"{eventData.EventId}, {DateTime.Now}, {eventData.EventName}, {eventData.Message}, {string.Join(", ", eventData.Payload)}");
    }
}