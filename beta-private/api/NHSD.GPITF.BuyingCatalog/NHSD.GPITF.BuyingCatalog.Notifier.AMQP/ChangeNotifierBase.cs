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

    protected ChangeNotifierBase(IConfiguration config)
    {
      var connStr = Settings.AMQP_CONNECTION_STRING(config);
      _address = new Address(connStr);
      _ttlMins = Settings.AMQP_TTL_MINS(config);
    }

    public void Notify(ChangeRecord<T> record)
    {
      var connection = new Connection(_address);
      var session = new Session(connection);
      var sender = new SenderLink(session, GetType().Name, $"topic://{typeof(T).Name}");
      var json = JsonConvert.SerializeObject(record);
      var data = new Data
      {
        Binary = Encoding.UTF8.GetBytes(json)
      };
      var msg = new Message
      {
        Header = new Header
        {
          Ttl = _ttlMins * 60 * 1000
        },
        BodySection = data
      };

      sender.Send(msg);
    }
  }
}
