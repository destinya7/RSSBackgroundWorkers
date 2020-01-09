using System.Diagnostics;

namespace RSSFetcherService.Services
{
    public class WindowsEventLoggerService : ILoggerService
    {
        public WindowsEventLoggerService()
        {
            if (!EventLog.SourceExists(SourceName))
            {
                EventLog.CreateEventSource(SourceName, "RSSFetcherServiceLog");
            }
        }

        public void Debug(string message)
        {
            
        }

        public void Error(string message)
        {
            using (var log = new EventLog())
            {
                log.Source = SourceName;

                log.WriteEntry(message, EventLogEntryType.Error);
            }
        }

        public void Fatal(string message)
        {
            
        }

        public void Info(string message)
        {
            using (var log = new EventLog())
            {
                log.Source = SourceName;
                log.WriteEntry(message, EventLogEntryType.Information);
            }
        }

        public void Warn(string message)
        {
            using (var log = new EventLog())
            {
                log.Source = SourceName;
                log.WriteEntry(message, EventLogEntryType.Warning);
            }
        }

        public string SourceName => "RSSFetcherService";
    }
}
