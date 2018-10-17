using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database.Porcelain
{
  public sealed class SolutionsExDatastore : DatastoreBase<SolutionEx>, ISolutionsExDatastore
  {
    private readonly ISolutionsDatastore _solutionDatastore;
    private readonly ITechnicalContactsDatastore _technicalContactDatastore;
    private readonly ICapabilitiesImplementedDatastore _claimedCapabilityDatastore;
    private readonly IStandardsApplicableDatastore _claimedStandardDatastore;

    public SolutionsExDatastore(
      IDbConnectionFactory dbConnectionFactory,
      ILogger<SolutionsExDatastore> logger,
      ISyncPolicyFactory policy,
      ISolutionsDatastore solutionDatastore,
      ITechnicalContactsDatastore technicalContactDatastore,
      ICapabilitiesImplementedDatastore claimedCapabilityDatastore,
      IStandardsApplicableDatastore claimedStandardDatastore) :
      base(dbConnectionFactory, logger, policy)
    {
      _solutionDatastore = solutionDatastore;
      _technicalContactDatastore = technicalContactDatastore;
      _claimedCapabilityDatastore = claimedCapabilityDatastore;
      _claimedStandardDatastore = claimedStandardDatastore;
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

          // TODO   Evidence + Reviews

          // delete all ClaimedCapability & re-insert
          _dbConnection.Value.Execute($@"delete from {nameof(CapabilitiesImplemented)} where SolutionId = '{solnEx.Solution.Id}';", transaction: trans);
          solnEx.ClaimedCapability.ForEach(cc => { cc.Id = cc.Id == Guid.Empty.ToString() ? Guid.NewGuid().ToString() : cc.Id; });
          _dbConnection.Value.Insert(solnEx.ClaimedCapability, trans);

          // delete all ClaimedStandard & re-insert
          _dbConnection.Value.Execute($@"delete from {nameof(StandardsApplicable)} where SolutionId = '{solnEx.Solution.Id}';", transaction: trans);
          solnEx.ClaimedStandard.ForEach(cs => { cs.Id = cs.Id == Guid.Empty.ToString() ? Guid.NewGuid().ToString() : cs.Id; });
          _dbConnection.Value.Insert(solnEx.ClaimedStandard, trans);

          // delete all TechnicalContact & re-insert
          _dbConnection.Value.Execute($@"delete from {nameof(TechnicalContacts)} where SolutionId = '{solnEx.Solution.Id}';", transaction: trans);
          solnEx.TechnicalContact.ForEach(tc => { tc.Id = tc.Id == Guid.Empty.ToString() ? Guid.NewGuid().ToString() : tc.Id; });
          _dbConnection.Value.Insert(solnEx.TechnicalContact, trans);

          trans.Commit();
        }

        return 0;
      });
    }
  }
}
