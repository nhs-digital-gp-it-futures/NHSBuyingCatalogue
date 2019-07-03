using Amqp;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;

namespace NHSD.GPITF.BuyingCatalog.Receiver.AMQP
{
  public abstract class ChangeReceiverBase<T>
  {
    private readonly bool _isAzure;
    private readonly string _topicPrefix;

    protected ChangeReceiverBase(IConfiguration config)
    {
      _topicPrefix = Settings.AMQP_TOPIC_PREFIX(config);
      _isAzure = Settings.USE_AZURE_SERVICE_BUS(config);

      var protocol = Settings.AMQP_PROTOCOL(config);
      var policyName = Settings.AMQP_POLICY_NAME(config);
      var policyKey = Settings.AMQP_POLICY_KEY(config);
      var namespaceUrl = Settings.AMQP_NAMESPACE_URL(config);
      var connStr = $"{protocol}://{policyName}:{policyKey}@{namespaceUrl}/";
      var address = new Address(connStr);
      var connection = new Connection(address);
      var session = new Session(connection);
      var topicAddress = GetTopicAddress();
      var receiver = new ReceiverLink(session, GetType().Name, topicAddress);

      receiver.Start(100, (rec, msg) =>
      {
        ProcessMessage(rec, msg);

        rec.Accept(msg);
      });
    }

    protected abstract void ProcessMessage(IReceiverLink rec, Message msg);

    private string GetTopicAddress()
    {
      return _isAzure ?
        $"{_topicPrefix}{typeof(T).Name}/Subscriptions/{typeof(T).Name}" :
        $"{_topicPrefix}{typeof(T).Name}";
    }
  }
}
