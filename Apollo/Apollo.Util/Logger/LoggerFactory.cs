using System;
using System.IO;
using Serilog;
using Serilog.Events;

namespace Apollo.Util.Logger
{
    public static class LoggerFactory
    {
        private const string LogFilePath = @"\Logs\apollo.log";

        private const string OutputTemplate =
            "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}[{Message}]{NewLine} in method {MemberName} at {FilePath}:{LineNumber}{NewLine}[{Exception}]{NewLine}";
        public static IApolloLogger<T> CreateLogger<T>()
        {
            var projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.FullName ?? string.Empty;

            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: OutputTemplate)
                .WriteTo.File($"{projectDirectory}{LogFilePath}", rollingInterval: RollingInterval.Day, outputTemplate: OutputTemplate)
                .MinimumLevel.Is(LogEventLevel.Debug)
                .MinimumLevel.Debug()
                .CreateLogger()
                .ForContext<T>();
            return new ApolloLogger<T>(logger);
        }

    }
}
