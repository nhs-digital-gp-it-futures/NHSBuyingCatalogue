using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public abstract class EvidenceDatastoreBase<T> : CommonTableExpressionDatastoreBase<T>, IEvidenceDatastore<EvidenceBase> where T : EvidenceBase
  {
    protected EvidenceDatastoreBase(
      IDbConnectionFactory dbConnectionFactory,
      ILogger<EvidenceDatastoreBase<T>> logger,
      ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IEnumerable<IEnumerable<T>> ByClaim(string claimId)
    {
      return ByOwner(claimId);
    }

    protected override string GetAllSqlCurrent(string tableName)
    {
      return
$@"
-- select all current versions
select ev.* from {tableName} ev where Id not in 
(
  select PreviousId from {tableName} where PreviousId is not null
) and
ClaimId = @ownerId
";
    }

    public override string GetSqlCurrent(string tableName)
    {
      return
 $@"
-- get all previous versions from a specified (CurrentId) version
with recursive Links(CurrentId, Id, PreviousId, ClaimId, CreatedById, CreatedOn, OriginalDate, Evidence, HasRequestedLiveDemo, BlobId) as (
  select
    Id, Id, PreviousId, ClaimId, CreatedById, CreatedOn, OriginalDate, Evidence, HasRequestedLiveDemo, BlobId
  from {tableName}
  where PreviousId is null
  
  union all
  select
    Id, Id, PreviousId, ClaimId, CreatedById, CreatedOn, OriginalDate, Evidence, HasRequestedLiveDemo, BlobId
  from {tableName} 
  where PreviousId is not null
  
  union all
  select
    Links.CurrentId,
    {tableName}.Id,
    {tableName}.PreviousId,
    {tableName}.ClaimId,
    {tableName}.CreatedById,
    {tableName}.CreatedOn,
    {tableName}.OriginalDate,
    {tableName}.Evidence,
    {tableName}.HasRequestedLiveDemo,
    {tableName}.BlobId
  from Links
  join {tableName}
  on Links.PreviousId = {tableName}.Id
)
  select Links.Id, Links.PreviousId, Links.ClaimId, Links.CreatedById, Links.CreatedOn, Links.OriginalDate, Links.Evidence, Links.HasRequestedLiveDemo, Links.BlobId
  from Links
  where CurrentId = @currentId;
";
    }

    IEnumerable<IEnumerable<EvidenceBase>> IEvidenceDatastore<EvidenceBase>.ByClaim(string claimId)
    {
      return ByClaim(claimId);
    }

    EvidenceBase IEvidenceDatastore<EvidenceBase>.ById(string id)
    {
      return ById(id);
    }

    EvidenceBase IEvidenceDatastore<EvidenceBase>.Create(EvidenceBase evidence)
    {
      return Create((T)evidence);
    }
  }
}
