using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IReviewsDatastore
  {
    IQueryable<Reviews> BySolution(string solutionId);
    Reviews Create(Reviews assMess);
    void Update(Reviews assMess);
    void Delete(Reviews assMess);
  }
#pragma warning restore CS1591
}
