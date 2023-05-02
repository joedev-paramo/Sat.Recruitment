using NLog;

namespace Sat.Recruitment.Api.Infrastructure
{
    public class LoggerManager : ILoggerManager
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Used for NLog.")]
        private static ILogger logger = LogManager.GetCurrentClassLogger();


        public LoggerManager()
        {
        }

        public void LogDebug(string message)
        {
            logger.Debug(message);
        }

        public void LogError(string message)
        {
            logger.Error(message);
        }

        public void LogInformation(string message)
        {
            logger.Info(message);
        }

        public void LogWarning(string message)
        {
            logger.Warn(message);
        }
    }
}
