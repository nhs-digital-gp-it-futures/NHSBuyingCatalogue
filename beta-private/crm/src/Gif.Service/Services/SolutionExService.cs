#pragma warning disable 1591
using Gif.Service.Contracts;
using Gif.Service.Crm;
using Gif.Service.Models;

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
            //TODO: wrap in a transaction using a web api BATCH post
            foreach (var standardEvidence in solnEx.ClaimedStandardEvidence)
            {
                Repository.UpdateEntity(standardEvidence.EntityName, standardEvidence.Id, standardEvidence.SerializeToODataPut("cc_evidenceid"));
            }

            foreach (var standardReview in solnEx.ClaimedStandardReview)
            {
                Repository.UpdateEntity(standardReview.EntityName, standardReview.Id, standardReview.SerializeToODataPut("cc_reviewid"));
            }

            foreach (var capabilityEvidence in solnEx.ClaimedCapabilityEvidence)
            {
                Repository.UpdateEntity(capabilityEvidence.EntityName, capabilityEvidence.Id, capabilityEvidence.SerializeToODataPut("cc_evidenceid"));
            }

            foreach (var capabilityReview in solnEx.ClaimedCapabilityReview)
            {
                Repository.UpdateEntity(capabilityReview.EntityName, capabilityReview.Id, capabilityReview.SerializeToODataPut("cc_reviewid"));
            }

            foreach (var capabilityImplemented in solnEx.ClaimedCapability)
            {
                Repository.UpdateEntity(capabilityImplemented.EntityName, capabilityImplemented.Id, capabilityImplemented.SerializeToODataPut("cc_capabilityimplementedid"));
            }

            foreach (var standardApplicable in solnEx.ClaimedStandard)
            {
                Repository.UpdateEntity(standardApplicable.EntityName, standardApplicable.Id, standardApplicable.SerializeToODataPut("cc_standardapplicableid"));
            }

            foreach (var technicalContact in solnEx.TechnicalContact)
            {
                Repository.UpdateEntity(technicalContact.EntityName, technicalContact.Id, technicalContact.SerializeToODataPut("cc_technicalcontactid"));
            }

            Repository.UpdateEntity(solnEx.Solution.EntityName, solnEx.Solution.Id, solnEx.Solution.SerializeToODataPut("cc_solutionid"));
        }
    }
}
#pragma warning restore 1591
