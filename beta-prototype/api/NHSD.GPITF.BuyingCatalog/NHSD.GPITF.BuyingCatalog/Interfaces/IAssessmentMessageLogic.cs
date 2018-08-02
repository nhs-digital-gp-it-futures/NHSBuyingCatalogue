using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IAssessmentMessageLogic
  {
    IQueryable<AssessmentMessage> BySolution(string solutionId);
    AssessmentMessage Create(AssessmentMessage assMess);
    void Update(AssessmentMessage assMess);
    void Delete(AssessmentMessage assMess);
  }
#pragma warning restore CS1591
}
