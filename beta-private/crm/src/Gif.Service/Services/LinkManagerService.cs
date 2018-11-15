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

        public void FrameworkSolutionCreate(string frameworkId, string solutionId)
        {
            Guid frameworkIdParsed, solutionIdParsed;
            Guid.TryParse(frameworkId , out frameworkIdParsed);
            Guid.TryParse(solutionId, out solutionIdParsed);

            if (solutionIdParsed == Guid.Empty || frameworkIdParsed == Guid.Empty)
                throw new CrmApiException("Cannot parse strings into Guids", HttpStatusCode.BadRequest);

            Repository.Associate(frameworkIdParsed, "cc_frameworks", solutionIdParsed, "cc_solutions", RelationshipNames.SolutionFramework);
        }
    }
}
