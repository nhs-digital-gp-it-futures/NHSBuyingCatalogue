using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class KeywordSearchHistoryLogic : LogicBase, IKeywordSearchHistoryLogic
  {
    private readonly IKeywordSearchHistoryDatastore _datastore;

    public KeywordSearchHistoryLogic(
      IKeywordSearchHistoryDatastore datastore,
      IHttpContextAccessor context) :
      base(context)
    {
      _datastore = datastore;
    }

    public IEnumerable<Log> Get(DateTime startDate, DateTime endDate)
    {
      return _datastore.Get(startDate, endDate);
    }
  }
}
