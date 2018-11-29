#pragma warning disable CS1591 
using Gif.Plugins.Contracts;
using Microsoft.Xrm.Sdk;

namespace Gif.Plugins.Business_Logic
{
    public class CascadeDeleteLogic : BusinessLogic, ICascadeDeleteLogic
    {

        #region Properties

        public ISolutionRepository SolutionRepository { get; set; }

        #endregion

        public CascadeDeleteLogic(ISolutionRepository solutionRepository, string pluginName)
        {
            SolutionRepository = solutionRepository;
            PluginName = pluginName;
        }

        public void OnSolutionDelete(EntityReference target)
        {
            Trace("Get technical contacts");
            var technicalContacts = SolutionRepository.GetTechnicalContactsBySolution(target.Id);
            foreach (var technicalContact in technicalContacts)
            {
                Trace("Delete technical contact");
                SolutionRepository.Delete(technicalContact);
            }

            Trace("Get capabilities implemented");
            var capabilitiesImplemented = SolutionRepository.GetCapabilitiesImplementedBySolution(target.Id);
            foreach (var capabilityImplemented in capabilitiesImplemented)
            {
                Trace("Get evidences");
                var evidences = SolutionRepository.GetEvidencesByCapabilityImplemented(capabilityImplemented.Id);
                foreach (var evidence in evidences)
                {
                    Trace("Get reviews");
                    var reviews = SolutionRepository.GetReviewsByEvidence(evidence.Id);
                    foreach (var review in reviews)
                    {
                        Trace("Delete review");
                        SolutionRepository.Delete(review);
                    }

                    Trace("Delete evidence");
                    SolutionRepository.Delete(evidence);
                }

                Trace("Delete capability implemented");
                SolutionRepository.Delete(capabilityImplemented);
            }

            Trace("Get standards applicable");
            var standardsApplicable = SolutionRepository.GetStandardsApplicableBySolution(target.Id);
            foreach (var standardApplicable in standardsApplicable)
            {
                Trace("Get evidences");
                var evidences = SolutionRepository.GetEvidencesByStandardApplicable(standardApplicable.Id);
                foreach (var evidence in evidences)
                {
                    Trace("Get reviews");
                    var reviews = SolutionRepository.GetReviewsByEvidence(evidence.Id);
                    foreach (var review in reviews)
                    {
                        Trace("Delete review");
                        SolutionRepository.Delete(review);
                    }

                    Trace("Delete evidence");
                    SolutionRepository.Delete(evidence);
                }

                Trace("Delete standard applicable");
                SolutionRepository.Delete(standardApplicable);
            }
        }
    }
}
#pragma warning restore CS1591