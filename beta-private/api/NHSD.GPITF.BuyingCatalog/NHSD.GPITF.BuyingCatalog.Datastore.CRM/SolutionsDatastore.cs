﻿using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class SolutionsDatastore : DatastoreBase<Solutions>, ISolutionsDatastore
  {
    public SolutionsDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<SolutionsDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public IQueryable<Solutions> ByFramework(string frameworkId)
    {
      throw new NotImplementedException();
    }

    public Solutions ById(string id)
    {
      throw new NotImplementedException();
    }

    public IQueryable<Solutions> ByOrganisation(string organisationId)
    {
      throw new NotImplementedException();
    }

    public Solutions Create(Solutions solution)
    {
      throw new NotImplementedException();
    }

    public void Update(Solutions solution)
    {
      throw new NotImplementedException();
    }
  }
}