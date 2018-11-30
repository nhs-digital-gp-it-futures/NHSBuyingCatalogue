#pragma warning disable 1591
using Gif.Service.Contracts;
using Gif.Service.Crm;
using Gif.Service.Models;
using System.Collections.Generic;
using Gif.Service.Enums;

namespace Gif.Service.Services
{
    public class SolutionExService : ServiceBase, ISolutionsExDatastore
    {
        public SolutionExService(IRepository repository) : base(repository)
        {
        }

        public SolutionEx BySolution(string solutionId)
        {
            throw new System.NotImplementedException();
        }

        public void Update(SolutionEx solnEx)
        {
            var batchData = new List<BatchData>
            {
                new BatchData
                {
                    Id = solnEx.Solution.Id,
                    Name = solnEx.Solution.EntityName,
                    Type = BatchTypeEnum.Delete,
                    EntityData = "{}"
                },
                new BatchData
                {
                    Id = solnEx.Solution.Id,
                    Name = solnEx.Solution.EntityName,
                    EntityData = solnEx.Solution.SerializeToODataPut("cc_solutionid")
                }
            };

            //Sort Evidence/Reviews in order by previous Id
            solnEx.ClaimedCapabilityEvidence = GetInsertionTree(solnEx.ClaimedCapabilityEvidence);
            solnEx.ClaimedCapabilityReview = GetInsertionTree(solnEx.ClaimedCapabilityReview);
            solnEx.ClaimedStandardEvidence = GetInsertionTree(solnEx.ClaimedStandardEvidence);
            solnEx.ClaimedStandardReview = GetInsertionTree(solnEx.ClaimedStandardReview);

            foreach (var technicalContact in solnEx.TechnicalContact)
            {
                batchData.Add(new BatchData { Id = technicalContact.Id, Name = technicalContact.EntityName, EntityData = technicalContact.SerializeToODataPut("cc_technicalcontactid") });
            }

            foreach (var standardApplicable in solnEx.ClaimedStandard)
            {
                batchData.Add(new BatchData { Id = standardApplicable.Id, Name = standardApplicable.EntityName, EntityData = standardApplicable.SerializeToODataPut("cc_standardapplicableid") });
            }

            foreach (var capabilityImplemented in solnEx.ClaimedCapability)
            {
                batchData.Add(new BatchData { Id = capabilityImplemented.Id, Name = capabilityImplemented.EntityName, EntityData = capabilityImplemented.SerializeToODataPut("cc_capabilityimplementedid") });
            }

            foreach (var standardEvidence in solnEx.ClaimedStandardEvidence)
            {
                batchData.Add(new BatchData { Id = standardEvidence.Id, Name = standardEvidence.EntityName, EntityData = standardEvidence.SerializeToODataPut("cc_evidenceid") });
            }

            foreach (var capabilityEvidence in solnEx.ClaimedCapabilityEvidence)
            {
                batchData.Add(new BatchData { Id = capabilityEvidence.Id, Name = capabilityEvidence.EntityName, EntityData = capabilityEvidence.SerializeToODataPut("cc_evidenceid") });
            }

            foreach (var standardReview in solnEx.ClaimedStandardReview)
            {
                batchData.Add(new BatchData { Id = standardReview.Id, Name = standardReview.EntityName, EntityData = standardReview.SerializeToODataPut("cc_reviewid") });
            }

            foreach (var capabilityReview in solnEx.ClaimedCapabilityReview)
            {
                batchData.Add(new BatchData { Id = capabilityReview.Id, Name = capabilityReview.EntityName, EntityData = capabilityReview.SerializeToODataPut("cc_reviewid") });
            }

            Repository.CreateBatch(batchData);
        }
    }


}
#pragma warning restore 1591
