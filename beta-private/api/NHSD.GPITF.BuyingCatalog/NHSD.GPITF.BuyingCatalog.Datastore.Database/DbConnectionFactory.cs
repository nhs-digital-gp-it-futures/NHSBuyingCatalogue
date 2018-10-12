using Microsoft.Extensions.Configuration;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using System;
using System.Data;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class DbConnectionFactory : IDbConnectionFactory
  {
    private readonly IConfiguration _config;

    public DbConnectionFactory(IConfiguration config)
    {
      _config = config;
    }

    public IDbConnection Get()
    {
      // TODO   read from Environment
      var connection = _config["RepositoryDatabase:Connection"];
      var connType = _config[$"RepositoryDatabase:{connection}:Type"];
      var dbType = Enum.Parse<DataAccessProviderTypes>(connType);
      var dbFact = DbProviderFactoryUtils.GetDbProviderFactory(dbType);
      var dbConn = dbFact.CreateConnection();

      dbConn.ConnectionString = _config[$"RepositoryDatabase:{connection}:ConnectionString"].Replace("|DataDirectory|", AppDomain.CurrentDomain.BaseDirectory);
      dbConn.Open();

      return dbConn;
    }
  }
}
