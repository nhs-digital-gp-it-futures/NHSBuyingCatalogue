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
            Trace("Get evidences");
            var evidences = SolutionRepository.GetEvidencesByStandardApplicable(target.Id);
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
        }
    }
}
#pragma warning restore CS1591