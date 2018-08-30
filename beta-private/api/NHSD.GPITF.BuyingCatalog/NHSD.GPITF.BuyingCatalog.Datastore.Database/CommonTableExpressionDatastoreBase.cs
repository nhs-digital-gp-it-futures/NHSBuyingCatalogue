using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using System;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public abstract class CommonTableExpressionDatastoreBase<T> : DatastoreBase<T>
  {
    public CommonTableExpressionDatastoreBase(
      IDbConnectionFactory dbConnectionFactory,
      ILogger<CommonTableExpressionDatastoreBase<T>> logger,
      ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    /// <summary>
    /// Removes 'recursive' keyword from Common Table Expression when
    /// database is MS SQL Server as this database does not support this
    /// keyword.
    /// 
    /// 'Recursive' keyword is required for MySQL but is ignored by SQLite.
    /// 
    /// Currently untested on PostgreSql
    /// </summary>
    /// <param name="sql">generic SQL statement containing keyword 'recursive'</param>
    /// <returns>SQL statement for specific database</returns>
    protected string AmendCommonTableExpression(string sql)
    {
      var dbType = _dbConnection.Value.GetType().ToString();

      if (dbType == "Microsoft.Data.Sqlite.SqliteConnection")
      {
        return sql;
      }

      if (dbType == "MySql.Data.MySqlClient.MySqlConnection")
      {
        return sql;
      }

      if (dbType == "System.Data.SqlClient.SqlConnection")
      {
        return sql.Replace("recursive", "");
      }

      throw new ArgumentOutOfRangeException($"Untested database: {dbType}");
    }
  }
}
