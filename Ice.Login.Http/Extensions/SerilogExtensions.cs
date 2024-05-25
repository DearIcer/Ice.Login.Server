using Serilog;
using ILogger = Serilog.ILogger;

namespace Ice.Login.Http.Extensions;

public static class SerilogExtensions
{
    public static ILogger SetSerilog(ConfigurationManager configurationManager)
    {
        return new LoggerConfiguration()
            .ReadFrom.Configuration(configurationManager)
            .CreateLogger();
    }
}