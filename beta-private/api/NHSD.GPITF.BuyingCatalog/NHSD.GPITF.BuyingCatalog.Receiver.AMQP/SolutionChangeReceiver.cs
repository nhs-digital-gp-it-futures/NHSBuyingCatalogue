using Amqp;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Text;

namespace NHSD.GPITF.BuyingCatalog.Receiver.AMQP
{
  public sealed class SolutionChangeReceiver : ChangeReceiverBase<Solutions>
  {
    private readonly IEvidenceBlobStoreLogic _evidenceBlobStoreLogic;

    public SolutionChangeReceiver(
      IConfiguration config,
      IEvidenceBlobStoreLogic evidenceBlobStoreLogic) :
      base(config)
    {
      _evidenceBlobStoreLogic = evidenceBlobStoreLogic;
    }

    protected override void ProcessMessage(IReceiverLink rec, Message msg)
    {
      var json = Encoding.UTF8.GetString((byte[])msg.Body);
      var record = JsonConvert.DeserializeObject<ChangeRecord<Solutions>>(json);
      var solution = record.NewVersion;

      // create SharePoint folder structure
      if (solution.Status == SolutionStatus.Registered)
      {
        _evidenceBlobStoreLogic.PrepareForSolution(solution.Id);
      }
    }
  }
}
