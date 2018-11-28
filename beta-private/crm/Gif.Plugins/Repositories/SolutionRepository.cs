using Gif.Plugins.Contracts;
using Microsoft.Xrm.Sdk;

namespace Gif.Plugins.Repositories
{
    public class SolutionRepository : Repository, ISolutionRepository
    {
        public SolutionRepository(IOrganizationService svc) : base(svc)
        {
        }

    }
}
