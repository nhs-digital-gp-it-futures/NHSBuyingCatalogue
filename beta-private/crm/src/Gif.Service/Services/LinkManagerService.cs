using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Gif.Service.Const;
using Gif.Service.Contracts;
using Gif.Service.Crm;
using Gif.Service.Models;

namespace Gif.Service.Services
{
    public class LinkManagerService : ServiceBase, ILinkManagerDatastore 
    {
        public LinkManagerService(IRepository repository) : base(repository)
        {
        }

        public void FrameworkSolutionAssociate(Guid frameworkId, Guid solutionId)
        {
            Repository.Associate(frameworkId, "cc_frameworks", solutionId, "cc_solutions", RelationshipNames.SolutionFramework);
        }
    }
}
