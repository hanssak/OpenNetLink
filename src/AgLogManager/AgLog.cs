using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;

namespace AgLogManager
{
    public static class AgLog
    {
        public static LoggingLevelSwitch LogLevelSwitch {get; set; } = new LoggingLevelSwitch();

        public static Serilog.ILogger Here(
            this Serilog.ILogger logger,
            [CallerMemberName] string memberName = "",   
            [CallerFilePath] string filePath = "",    
            [CallerLineNumber] int lineNumber = 0)
        {
            return logger.ForContext("MemberName", "in method " + memberName)
                .ForContext("FilePath", "at " + filePath)
                .ForContext("FileName", Path.GetFileNameWithoutExtension(filePath))
                .ForContext("LineNumber", ":" + lineNumber);
        }
    }

    public class OperationIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {     
            if (logEvent.Properties.TryGetValue("RequestId", out var requestId))
                logEvent.AddPropertyIfAbsent(new LogEventProperty("operationId", requestId));
        }
    }
}