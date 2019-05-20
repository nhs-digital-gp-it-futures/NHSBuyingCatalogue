using NLog;

namespace NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint.App
{
  public sealed class LoggerManager<T> : ILoggerManager<T> where T : class
  {
    private readonly ILogger _logger = LogManager.GetLogger(typeof(T).ToString());

    public LoggerManager()
    {
    }

    public void LogDebug(string message)
    {
      _logger.Debug(message);
    }

    public void LogError(string message)
    {
      _logger.Error(message);
    }

    public void LogInfo(string message)
    {
      _logger.Info(message);
    }

    public void LogWarn(string message)
    {
      _logger.Warn(message);
    }
  }
}
