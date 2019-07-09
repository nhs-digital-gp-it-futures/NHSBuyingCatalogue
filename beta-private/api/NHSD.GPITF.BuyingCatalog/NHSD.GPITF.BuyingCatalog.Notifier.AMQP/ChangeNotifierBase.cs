using Amqp;
using Amqp.Framing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NHSD.GPITF.BuyingCatalog.Interfaces.Interfaces;
using System.Text;

namespace NHSD.GPITF.BuyingCatalog.Notifier.AMQP
{
  public abstract class ChangeNotifierBase<T>
  {
    private readonly Address _address;
    private readonly uint _ttlMins;
    private readonly string _topicPrefix;

    protected ChangeNotifierBase(IConfiguration config)
    {
      var protocol = Settings.AMQP_PROTOCOL(config);
      var policyName = Settings.AMQP_POLICY_NAME(config);
      var policyKey = Settings.AMQP_POLICY_KEY(config);
      var namespaceUrl = Settings.AMQP_NAMESPACE_URL(config);
      var connStr = $"{protocol}://{policyName}:{policyKey}@{namespaceUrl}/";
      _address = new Address(connStr);
      _ttlMins = Settings.AMQP_TTL_MINS(config);
      _topicPrefix = Settings.AMQP_TOPIC_PREFIX(config);
    }

    public void Notify(ChangeRecord<T> record)
    {
      var connection = new Connection(_address);
      var session = new Session(connection);
      var sender = new SenderLink(session, GetType().Name, $"{_topicPrefix}{typeof(T).Name}");
      var json = JsonConvert.SerializeObject(record);
      var header = new Header
      {
        Ttl = _ttlMins * 60 * 1000
      };
      var data = new Data
      {
        Binary = Encoding.UTF8.GetBytes(json)
      };
      var msg = new Message
      {
        Header = header,
        BodySection = data
      };

      sender.Send(msg);
    }
  }
}
