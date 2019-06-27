using Amqp;
using Amqp.Framing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NHSD.GPITF.BuyingCatalog.Interfaces.Interfaces;
using System.Text;

namespace NHSD.GPITF.BuyingCatalog.Logic.Notifiers
{
  public abstract class ChangeNotifierBase<T>
  {
    private readonly Address _address;

    protected ChangeNotifierBase(IConfiguration config)
    {
      var connStr = Settings.AMQP_CONNECTION_STRING(config);
      _address = new Address(connStr);
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
        BodySection = data
      };

      sender.Send(msg);
    }
  }
}
