using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IAssessmentMessageContactDatastore
  {
    IQueryable<AssessmentMessageContact> ByAssessmentMessage(string assMessId);
    AssessmentMessageContact Create(AssessmentMessageContact assMessCont);
    void Update(AssessmentMessageContact assMessCont);
    void Delete(AssessmentMessageContact assMessCont);
  }
#pragma warning restore CS1591
}
