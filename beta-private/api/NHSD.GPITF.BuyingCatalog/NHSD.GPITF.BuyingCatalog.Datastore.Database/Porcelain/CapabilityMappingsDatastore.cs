using Dapper;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database.Porcelain
{
  public sealed class CapabilityMappingsDatastore : DatastoreBase<CapabilityMapping>, ICapabilityMappingsDatastore
  {
    public CapabilityMappingsDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<CapabilityMappingsDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public CapabilityMappings GetAll()
    {
      return GetInternal(() =>
      {
        var retval = new CapabilityMappings();
        const string sql = @"
select cap.*, '|' as '|', std.*, '|' as '|', cs.*
from CapabilityStandard cs
join Capabilities cap on cap.Id = cs.CapabilityId
join Standards std on std.Id = cs.StandardId
";
        _dbConnection.Value.Query<Capabilities, Standards, CapabilityStandard, CapabilityMapping>(sql,
          (cap, std, cs) =>
          {
            var thisCapMap = retval.CapabilityMapping.SingleOrDefault(x => x.Capability.Id == cap.Id);
            if (thisCapMap == null)
            {
              thisCapMap = new CapabilityMapping
              {
                Capability = cap
              };
              retval.CapabilityMapping.Add(thisCapMap);
            }

            thisCapMap.OptionalStandard.Add(
              new OptionalStandard
              {
                StandardId = std.Id,
                IsOptional = cs.IsOptional
              });

            var thiStd = retval.Standard.SingleOrDefault(x => x.Id == std.Id);
            if (thiStd == null)
            {
              retval.Standard.Add(std);
            }

            return thisCapMap;
          },
          splitOn: "|,|");

        return retval;
      });
    }
  }
}
