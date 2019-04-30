using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public abstract class CommonTableExpressionDatastoreBase<T> : DatastoreBase<T> where T : IHasId
  {
    protected CommonTableExpressionDatastoreBase(
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
    /// 'Recursive' keyword is definitely required for MySQL and PostgreSql
    /// but is ignored by SQLite.
    /// </summary>
    /// <param name="sql">generic SQL statement containing keyword 'recursive'</param>
    /// <returns>SQL statement for specific database</returns>
    protected string AmendCommonTableExpression(string sql)
    {
      if (!sql.Contains("recursive"))
      {
        throw new ArgumentException("SQL Common Table Expression modified - cannot find 'recursive'");
      }

      var dbType = _dbConnection.Value.GetType().ToString();
      switch (dbType)
      {
        case "Microsoft.Data.Sqlite.SqliteConnection":
        case "MySql.Data.MySqlClient.MySqlConnection":
        case "Npgsql.NpgsqlConnection":
          return sql;

        case "System.Data.SqlClient.SqlConnection":
          return sql.Replace("recursive", "");

        default:
          throw new ArgumentOutOfRangeException($"Untested database: {dbType}");
      }
    }

    protected abstract string GetAllSqlCurrent(string tableName);
    public abstract string GetSqlCurrent(string tableName);

    protected IEnumerable<IEnumerable<T>> BySelf(string id)
    {
      return GetInternal(() =>
      {
        var table = typeof(T).GetCustomAttribute<TableAttribute>(true);
        var chains = new List<IEnumerable<T>>();
        var sqlAllCurrent = GetAllSqlCurrent(table.Name);
        var allCurrent = _dbConnection.Value.Query<T>(sqlAllCurrent, new { id });
        foreach (var current in allCurrent)
        {
          var sqlCurrent = GetSqlCurrent(table.Name);
          var amendedSql = AmendCommonTableExpression(sqlCurrent);
          var chain = _dbConnection.Value.Query<T>(amendedSql, new { currentId = current.Id });
          chains.Add(chain);
        }

        return chains;
      });
    }
  }
}
