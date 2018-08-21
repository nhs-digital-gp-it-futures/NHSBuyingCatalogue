using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class OrganisationsDatastore : DatastoreBase<Organisations>, IOrganisationsDatastore
  {
    public OrganisationsDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<OrganisationsDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public Organisations ByEmail(string email)
    {
      return GetInternal(() =>
      {
        // TODO   change to use IOrganisationsDatastore.ByEmail
        return _dbConnection.Value.GetAll<Organisations>().SingleOrDefault(org => org.OdsCode == email);
      });
    }
  }
}
