#pragma warning disable 1591
using Gif.Service.Attributes;
using Gif.Service.Const;
using Gif.Service.Contracts;
using Gif.Service.Crm;
using Gif.Service.Enums;
using Gif.Service.Models;
using Gif.Service.Services.Equality;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gif.Service.Services
{
  public class SolutionExService : ServiceBase, ISolutionsExDatastore
  {
    private readonly ISolutionsDatastore _solutionsDatastore;
    private readonly ITechnicalContactsDatastore _technicalContactsDatastore;
    private readonly ICapabilitiesImplementedDatastore _claimedCapabilityDatastore;
    private readonly IStandardsApplicableDatastore _claimedStandardDatastore;
    private readonly ICapabilitiesImplementedEvidenceDatastore _claimedCapabilityEvidenceDatastore;
    private readonly ICapabilitiesImplementedReviewsDatastore _claimedCapabilityReviewsDatastore;
    private readonly IStandardsApplicableEvidenceDatastore _claimedStandardEvidenceDatastore;
    private readonly IStandardsApplicableReviewsDatastore _claimedStandardReviewsDatastore;

    public SolutionExService(IRepository repository,
        ISolutionsDatastore solutionsDatastore,
        ITechnicalContactsDatastore technicalContactsDatastore,
        ICapabilitiesImplementedDatastore claimedCapabilityDatastore,
        IStandardsApplicableDatastore claimedStandardDatastore,
        ICapabilitiesImplementedEvidenceDatastore claimedCapabilityEvidenceDatastore,
        ICapabilitiesImplementedReviewsDatastore claimedCapabilityReviewsDatastore,
        IStandardsApplicableEvidenceDatastore claimedStandardEvidenceDatastore,
        IStandardsApplicableReviewsDatastore claimedStandardReviewsDatastore) : base(repository)
    {
      _solutionsDatastore = solutionsDatastore;
      _technicalContactsDatastore = technicalContactsDatastore;
      _claimedCapabilityDatastore = claimedCapabilityDatastore;
      _claimedStandardDatastore = claimedStandardDatastore;
      _claimedCapabilityEvidenceDatastore = claimedCapabilityEvidenceDatastore;
      _claimedCapabilityReviewsDatastore = claimedCapabilityReviewsDatastore;
      _claimedStandardEvidenceDatastore = claimedStandardEvidenceDatastore;
      _claimedStandardReviewsDatastore = claimedStandardReviewsDatastore;
    }

    public SolutionEx BySolution(string solutionId)
    {
      var filterAttributes = new List<CrmFilterAttribute>
            {
                new CrmFilterAttribute("SolutionId") {FilterName = "cc_solutionid", FilterValue = solutionId},
                new CrmFilterAttribute("StateCode") {FilterName = "statecode", FilterValue = "0"}
            };

      var appJson = Repository.RetrieveMultiple(new SolutionExFullRetrieve().GetQueryString(null, filterAttributes, true, true));
      var solutionJson = appJson?.FirstOrDefault();

      if (solutionJson == null)
      {
        return null;
      }

      var technicalContacts = new List<TechnicalContact>();
      var claimedCapabilities = new List<CapabilityImplemented>();
      var claimedStandard = new List<StandardApplicable>();

      if (solutionJson?[RelationshipNames.SolutionTechnicalContact] != null)
      {
        foreach (var technicalContact in solutionJson[RelationshipNames.SolutionTechnicalContact].Children())
        {
          technicalContacts.Add(new TechnicalContact(technicalContact));
        }
      }

      if (solutionJson?[RelationshipNames.SolutionCapabilityImplemented] != null)
      {
        foreach (var capabilityImplemented in solutionJson[RelationshipNames.SolutionCapabilityImplemented].Children())
        {
          claimedCapabilities.Add(new CapabilityImplemented(capabilityImplemented));
        }
      }

      if (solutionJson?[RelationshipNames.SolutionStandardApplicable] != null)
      {
        foreach (var standardApplicable in solutionJson[RelationshipNames.SolutionStandardApplicable].Children())
        {
          claimedStandard.Add(new StandardApplicable(standardApplicable));
        }
      }

      var solution = new SolutionEx
      {
        Solution = new Solution(solutionJson),
        TechnicalContact = technicalContacts,
        ClaimedCapability = claimedCapabilities,
        ClaimedStandard = claimedStandard,
        ClaimedCapabilityEvidence = new List<CapabilityEvidence>(),
        ClaimedStandardEvidence = new List<StandardApplicableEvidence>(),
        ClaimedCapabilityReview = new List<Review>(),
        ClaimedStandardReview = new List<Review>()
      };

      var capabilitiesImplemented = solution.ClaimedCapability.Select(cc => cc.Id).ToList();
      var standardsApplicable = solution.ClaimedStandard.Select(cs => cs.Id).ToList();

      if (capabilitiesImplemented.Any())
        solution.ClaimedCapabilityEvidence = _claimedCapabilityEvidenceDatastore.ByClaimMultiple(capabilitiesImplemented)
                .SelectMany(x => x)
                .ToList();

      if (standardsApplicable.Any())
        solution.ClaimedStandardEvidence = _claimedStandardEvidenceDatastore.ByClaimMultiple(standardsApplicable)
            .SelectMany(x => x)
            .ToList();

      var capabilityEvidences = solution.ClaimedCapabilityEvidence.Select(cc => cc.Id).ToList();
      var standardApplicableEvidences = solution.ClaimedStandardEvidence.Select(cs => cs.Id).ToList();

      if (capabilityEvidences.Any())
        solution.ClaimedCapabilityReview = _claimedCapabilityReviewsDatastore.ByEvidenceMultiple(capabilityEvidences)
            .SelectMany(x => x)
            .ToList();

      if (standardApplicableEvidences.Any())
        solution.ClaimedStandardReview = _claimedStandardReviewsDatastore.ByEvidenceMultiple(standardApplicableEvidences)
            .SelectMany(x => x)
            .ToList();

      return solution;
    }

    public void Update(SolutionEx solnEx)
    {
      var batchData = new List<BatchData>();
      var existingSolution = BySolution(solnEx.Solution.Id.ToString());

      if (existingSolution != null)
        batchData = ComposeDeleteRequests(existingSolution, solnEx, batchData);

      //Sort Evidence/Reviews in order by previous Id
      solnEx.ClaimedCapabilityEvidence = GetInsertionTree(solnEx.ClaimedCapabilityEvidence);
      solnEx.ClaimedCapabilityReview = GetInsertionTree(solnEx.ClaimedCapabilityReview);
      solnEx.ClaimedStandardEvidence = GetInsertionTree(solnEx.ClaimedStandardEvidence);
      solnEx.ClaimedStandardReview = GetInsertionTree(solnEx.ClaimedStandardReview);

      foreach (var technicalContact in solnEx.TechnicalContact)
      {
        batchData.Add(
          new BatchData
          {
            Id = technicalContact.Id,
            Name = technicalContact.EntityName,
            EntityData = technicalContact.SerializeToODataPut("cc_technicalcontactid")
          });
      }

      foreach (var standardApplicable in solnEx.ClaimedStandard)
      {
        batchData.Add(
          new BatchData
          {
            Id = standardApplicable.Id,
            Name = standardApplicable.EntityName,
            EntityData = standardApplicable.SerializeToODataPut("cc_standardapplicableid")
          });
      }

      foreach (var capabilityImplemented in solnEx.ClaimedCapability)
      {
        batchData.Add(
          new BatchData
          {
            Id = capabilityImplemented.Id,
            Name = capabilityImplemented.EntityName,
            EntityData = capabilityImplemented.SerializeToODataPut("cc_capabilityimplementedid")
          });
      }

      foreach (var standardEvidence in solnEx.ClaimedStandardEvidence)
      {
        batchData.Add(
          new BatchData
          {
            Id = standardEvidence.Id,
            Name = standardEvidence.EntityName,
            EntityData = standardEvidence.SerializeToODataPut("cc_evidenceid")
          });
      }

      foreach (var capabilityEvidence in solnEx.ClaimedCapabilityEvidence)
      {
        batchData.Add(
          new BatchData
          {
            Id = capabilityEvidence.Id,
            Name = capabilityEvidence.EntityName,
            EntityData = capabilityEvidence.SerializeToODataPut("cc_evidenceid")
          });
      }

      foreach (var standardReview in solnEx.ClaimedStandardReview)
      {
        batchData.Add(
          new BatchData
          {
            Id = standardReview.Id,
            Name = standardReview.EntityName,
            EntityData = standardReview.SerializeToODataPut("cc_reviewid")
          });
      }

      foreach (var capabilityReview in solnEx.ClaimedCapabilityReview)
      {
        batchData.Add(
          new BatchData
          {
            Id = capabilityReview.Id,
            Name = capabilityReview.EntityName,
            EntityData = capabilityReview.SerializeToODataPut("cc_reviewid")
          });
      }

      batchData.Add(
          new BatchData
          {
            Id = solnEx.Solution.Id,
            Name = solnEx.Solution.EntityName,
            EntityData = solnEx.Solution.SerializeToODataPut("cc_solutionid")
          });

      Repository.UpsertBatch(batchData);

      // START_HACK
      // We are encountering some issues with batch updates in CRM.
      // In particular, Status of a ClaimedStandard is not being updated,
      // even though CRM says the batch update has been successful.
      // The workaround is to wait and then retry the batch update.
      // IMDFWI
      Thread.Sleep(TimeSpan.FromSeconds(5));
      Repository.UpsertBatch(batchData);
      // END_HACK
    }

    public IEnumerable<SolutionEx> ByOrganisation(string organisationId)
    {
      var solns = _solutionsDatastore.ByOrganisation(organisationId);

      var tasks = solns.Select(soln =>
      {
        return Task<SolutionEx>.Factory.StartNew(() =>
        {
      return BySolution(soln.Id.ToString());
    });
      });
      Task.WaitAll(tasks.ToArray());

      var retval = tasks.Select(t => t.Result);

      return retval;
    }

    private List<BatchData> ComposeDeleteRequests(SolutionEx existingSolution, SolutionEx updatedSolution, List<BatchData> batchData)
    {
      var claimedCapabilityComparator = new ClaimedCapabilityComparator();
      var claimedStandardComparator = new ClaimedStandardComparator();
      var techContactComparator = new TechnicalContactComparator();

      var claimedCapabilityDeletes =
          existingSolution.ClaimedCapability.Except(updatedSolution.ClaimedCapability, claimedCapabilityComparator);
      var claimedStandardDeletes =
          existingSolution.ClaimedStandard.Except(updatedSolution.ClaimedStandard, claimedStandardComparator);
      var technicalContactDeletes = existingSolution.TechnicalContact.Except(updatedSolution.TechnicalContact, techContactComparator);

      batchData.AddRange(claimedCapabilityDeletes.Select(claimedCapability => new BatchData { Id = claimedCapability.Id, Name = claimedCapability.EntityName, Type = BatchTypeEnum.Delete, EntityData = "{}" }));
      batchData.AddRange(claimedStandardDeletes.Select(claimedStandard => new BatchData { Id = claimedStandard.Id, Name = claimedStandard.EntityName, Type = BatchTypeEnum.Delete, EntityData = "{}" }));
      batchData.AddRange(technicalContactDeletes.Select(technicalContact => new BatchData { Id = technicalContact.Id, Name = technicalContact.EntityName, Type = BatchTypeEnum.Delete, EntityData = "{}" }));

      return batchData;
    }
  }

}
#pragma warning restore 1591
