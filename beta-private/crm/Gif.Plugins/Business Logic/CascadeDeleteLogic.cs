#pragma warning disable CS1591 
using Gif.Plugins.Contracts;
using Microsoft.Xrm.Sdk;
using System.Linq;

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

        public void OnSolutionDelete(Entity target)
        {
            Trace($"Solution: {target.Id}");
            Trace("Get technical contacts");
            var technicalContacts = SolutionRepository.GetTechnicalContactsBySolution(target.Id);
            var ccTechnicalcontacts = technicalContacts.ToList();
            Trace($"technicalContacts count: {ccTechnicalcontacts.Count}");
            foreach (var technicalContact in ccTechnicalcontacts)
            {
                Trace("Delete technical contact");
                SolutionRepository.Delete(technicalContact);
            }

            Trace("Get capabilities implemented");
            var capabilitiesImplemented = SolutionRepository.GetCapabilitiesImplementedBySolution(target.Id);
            var ccCapabilitiesImplemented = capabilitiesImplemented.ToList();
            Trace($"capabilitiesImplemented count: {ccCapabilitiesImplemented.Count}");
            foreach (var capabilityImplemented in ccCapabilitiesImplemented)
            {
                Trace("Get evidences");
                var evidences = SolutionRepository.GetEvidencesByCapabilityImplemented(capabilityImplemented.Id);
                var ccEvidences = evidences.ToList();
                Trace($"evidences count: {ccEvidences.Count}");
                foreach (var evidence in ccEvidences)
                {
                    Trace("Get reviews");
                    var reviews = SolutionRepository.GetReviewsByEvidence(evidence.Id);
                    var ccReviews = reviews.ToList();
                    Trace($"reviews count: {ccReviews.Count}");
                    foreach (var review in ccReviews)
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
            var ccStandardsApplicable = standardsApplicable.ToList();
            Trace($"standardsApplicable count: {ccStandardsApplicable.Count}");
            foreach (var standardApplicable in ccStandardsApplicable)
            {
                Trace("Get evidences");
                var evidences = SolutionRepository.GetEvidencesByStandardApplicable(standardApplicable.Id);
                var ccEvidences = evidences.ToList();
                Trace($"evidences count: {ccEvidences.Count}");
                foreach (var evidence in ccEvidences)
                {
                    Trace("Get reviews");
                    var reviews = SolutionRepository.GetReviewsByEvidence(evidence.Id);
                    var ccReviews = reviews.ToList();
                    Trace($"reviews count: {ccReviews.Count}");
                    foreach (var review in ccReviews)
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