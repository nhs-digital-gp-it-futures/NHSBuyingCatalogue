using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public abstract class ReviewsDatastoreBase<T> : CommonTableExpressionDatastoreBase<T>, IReviewsDatastore<ReviewsBase> where T : ReviewsBase
  {
    protected ReviewsDatastoreBase(
      IDbConnectionFactory dbConnectionFactory,
      ILogger<ReviewsDatastoreBase<T>> logger,
      ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IEnumerable<IEnumerable<T>> ByEvidence(string evidenceId)
    {
      return BySelf(evidenceId);
    }

    protected override string GetAllSqlCurrent(string tableName)
    {
      return
$@"
-- select all current versions
select rev.* from {tableName} rev where Id not in 
(
  select PreviousId from {tableName} where PreviousId is not null
) and
EvidenceId = @evidenceId
";
    }

    public override string GetSqlCurrent(string tableName)
    {
      return
$@"
-- get all previous versions from a specified (CurrentId) version
with recursive Links(CurrentId, Id, PreviousId, EvidenceId, CreatedById, CreatedOn, OriginalDate, Message) as (
  select
    Id, Id, PreviousId, EvidenceId, CreatedById, CreatedOn, OriginalDate, Message
  from {tableName}
  where PreviousId is null
  
  union all
  select
    Id, Id, PreviousId, EvidenceId, CreatedById, CreatedOn, OriginalDate, Message
  from {tableName} 
  where PreviousId is not null
  
  union all
  select
    Links.CurrentId,
    {tableName}.Id,
    {tableName}.PreviousId,
    {tableName}.EvidenceId,
    {tableName}.CreatedById,
    {tableName}.CreatedOn,
    {tableName}.OriginalDate,
    {tableName}.Message
  from Links
  join {tableName}
  on Links.PreviousId = {tableName}.Id
)
  select Links.Id, Links.PreviousId, Links.EvidenceId, Links.CreatedById, Links.CreatedOn, Links.OriginalDate, Links.Message
  from Links
  where CurrentId = @currentId;
";
    }

    public void Delete(T review)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          _dbConnection.Value.Delete(review, trans);
          trans.Commit();

          return 0;
        }
      });
    }

    IEnumerable<IEnumerable<ReviewsBase>> IReviewsDatastore<ReviewsBase>.ByEvidence(string evidenceId)
    {
      return ByEvidence(evidenceId);
    }

    ReviewsBase IReviewsDatastore<ReviewsBase>.ById(string id)
    {
      return ById(id);
    }

    ReviewsBase IReviewsDatastore<ReviewsBase>.Create(ReviewsBase review)
    {
      return Create((T)review);
    }

    void IReviewsDatastore<ReviewsBase>.Delete(ReviewsBase review)
    {
      Delete((T)review);
    }
  }
}
