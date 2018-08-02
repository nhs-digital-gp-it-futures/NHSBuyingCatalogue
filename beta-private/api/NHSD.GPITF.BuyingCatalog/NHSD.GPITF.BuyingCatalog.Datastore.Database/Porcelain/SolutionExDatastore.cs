using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database.Porcelain
{
  public sealed class SolutionExDatastore : DatastoreBase<SolutionEx>, ISolutionExDatastore
  {
    private readonly ISolutionDatastore _solutionDatastore;
    private readonly ITechnicalContactDatastore _technicalContactDatastore;
    private readonly IClaimedCapabilityDatastore _claimedCapabilityDatastore;
    private readonly IClaimedStandardDatastore _claimedStandardDatastore;
    private readonly IClaimedCapabilityStandardDatastore _claimedCapabilityStandardDatastore;

    public SolutionExDatastore(
      IDbConnectionFactory dbConnectionFactory,
      ILogger<SolutionExDatastore> logger,
      ISyncPolicyFactory policy,
      ISolutionDatastore solutionDatastore,
      ITechnicalContactDatastore technicalContactDatastore,
      IClaimedCapabilityDatastore claimedCapabilityDatastore,
      IClaimedStandardDatastore claimedStandardDatastore,
      IClaimedCapabilityStandardDatastore claimedCapabilityStandardDatastore) :
      base(dbConnectionFactory, logger, policy)
    {
      _solutionDatastore = solutionDatastore;
      _technicalContactDatastore = technicalContactDatastore;
      _claimedCapabilityDatastore = claimedCapabilityDatastore;
      _claimedStandardDatastore = claimedStandardDatastore;
      _claimedCapabilityStandardDatastore = claimedCapabilityStandardDatastore;
    }

    public SolutionEx BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        var retval = new SolutionEx
        {
          Solution = _solutionDatastore.ById(solutionId),
          TechnicalContact = _technicalContactDatastore.BySolution(solutionId).ToList(),
          ClaimedStandard = _claimedStandardDatastore.BySolution(solutionId).ToList(),
          ClaimedCapability = _claimedCapabilityDatastore.BySolution(solutionId).ToList()
        };
        retval.ClaimedCapabilityStandard = retval
          .ClaimedCapability
          .SelectMany(cc => _claimedCapabilityStandardDatastore.ByClaimedCapability(cc.Id).ToList())
          .ToList();

        return retval;
      });
    }

    public void Update(SolutionEx solnEx)
    {
      if (solnEx.ClaimedCapability.Any(cc => cc.SolutionId != solnEx.Solution.Id))
      {
        throw new InvalidOperationException("ClaimedCapability does not belong to Solution");
      }

      if (solnEx.ClaimedStandard.Any(cs => cs.SolutionId != solnEx.Solution.Id))
      {
        throw new InvalidOperationException("ClaimedStandard does not belong to Solution");
      }

      if (solnEx.TechnicalContact.Any(tc => tc.SolutionId != solnEx.Solution.Id))
      {
        throw new InvalidOperationException("TechnicalContact does not belong to Solution");
      }

      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          // update Solution
          _dbConnection.Value.Update(solnEx.Solution, trans);

          // delete all ClaimedCapability & re-insert
          _dbConnection.Value.Execute($@"delete from ClaimedCapability where SolutionId = '{solnEx.Solution.Id}';", trans);
          solnEx.ClaimedCapability.ForEach(cc => { cc.Id = cc.Id == Guid.Empty.ToString() ? Guid.NewGuid().ToString() : cc.Id; });
          _dbConnection.Value.Insert(solnEx.ClaimedCapability);

          // delete all ClaimedStandard & re-insert
          _dbConnection.Value.Execute($@"delete from ClaimedStandard where SolutionId = '{solnEx.Solution.Id}';", trans);
          solnEx.ClaimedStandard.ForEach(cs => { cs.Id = cs.Id == Guid.Empty.ToString() ? Guid.NewGuid().ToString() : cs.Id; });
          _dbConnection.Value.Insert(solnEx.ClaimedStandard);

          // delete all ClaimedCapabilityStandard & re-insert
          solnEx.ClaimedCapabilityStandard.ForEach(ccs => { _dbConnection.Value.Delete(ccs); });
          _dbConnection.Value.Insert(solnEx.ClaimedCapabilityStandard);

          // delete all TechnicalContact & re-insert
          _dbConnection.Value.Execute($@"delete from TechnicalContact where SolutionId = '{solnEx.Solution.Id}';", trans);
          solnEx.TechnicalContact.ForEach(tc => { tc.Id = tc.Id == Guid.Empty.ToString() ? Guid.NewGuid().ToString() : tc.Id; });
          _dbConnection.Value.Insert(solnEx.TechnicalContact);

          trans.Commit();
        }

        return 0;
      });
    }
  }
}
