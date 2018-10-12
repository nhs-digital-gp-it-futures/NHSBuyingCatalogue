using Dapper;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class KeywordSearchHistoryDatastore : DatastoreBase<Log>, IKeywordSearchHistoryDatastore
  {
    public KeywordSearchHistoryDatastore(
      IDbConnectionFactory dbConnectionFactory,
      ILogger<DatastoreBase<Log>> logger,
      ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IEnumerable<Log> Get(DateTime startDate, DateTime endDate)
    {
      return GetInternal(() =>
      {
        const string SearchByKeywordCallsite = "NHSD.GPITF.BuyingCatalog.Search.Porcelain.SearchDatastore.ByKeyword";
        var sql = $@"
select log.* from Log log
where Callsite like '{SearchByKeywordCallsite}'
";
        // cannot use GetAll() because this requires an explicit key
        var retval = _dbConnection.Value.Query<Log>(sql)
          .Where(x => x.Timestamp >= startDate && x.Timestamp <= endDate);
        return retval;
      });
    }
  }
}
