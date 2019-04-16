using System;
using System.Linq;
using System.Reflection;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using GifModels = Gif.Service.Models;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  internal static class Creator
  {
    private static TTarget Convert<TSource, TTarget>(TSource source) where TTarget : new()
    {
      const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

      var target = new TTarget();
      var sourceProps = source.GetType().GetProperties(bindingFlags);
      var targetProps = target.GetType().GetProperties(bindingFlags);

      foreach (var targetProp in targetProps)
      {
        var sourceProp = sourceProps.SingleOrDefault(prop => prop.Name.Equals(targetProp.Name, StringComparison.InvariantCultureIgnoreCase));

        if (sourceProp?.PropertyType == typeof(Guid) &&
          targetProp.PropertyType == typeof(string))
        {
          targetProp.SetValue(target, ((Guid)sourceProp.GetValue(source)).ToString());
          continue;
        }

        targetProp.SetValue(target, sourceProp?.GetValue(source));
      }

      return target;
    }

    internal static Frameworks FromCrm(GifModels.Framework crm)
    {
      return Convert<GifModels.Framework, Frameworks>(crm);
    }

    internal static TechnicalContacts FromCrm(GifModels.TechnicalContact crm)
    {
      return Convert<GifModels.TechnicalContact, TechnicalContacts>(crm);
    }

    internal static GifModels.TechnicalContact FromApi(TechnicalContacts api)
    {
      throw new NotImplementedException();
    }

    internal static Solutions FromCrm(GifModels.Solution crm)
    {
      return Convert<GifModels.Solution, Solutions>(crm);
    }

    internal static GifModels.Solution FromApi(Solutions api)
    {
      throw new NotImplementedException();
    }

    internal static Capabilities FromCrm(GifModels.Capability crm)
    {
      return Convert<GifModels.Capability, Capabilities>(crm);
    }

    internal static Standards FromCrm(GifModels.Standard crm)
    {
      return Convert<GifModels.Standard, Standards>(crm);
    }

    internal static Organisations FromCrm(GifModels.Organisation crm)
    {
      return Convert<GifModels.Organisation, Organisations>(crm);
    }

    internal static CapabilityStandard FromCrm(GifModels.CapabilityStandard crm)
    {
      return Convert<GifModels.CapabilityStandard, CapabilityStandard>(crm);
    }

    internal static SolutionEx FromCrm(GifModels.SolutionEx crm)
    {
      return Convert<GifModels.SolutionEx, SolutionEx>(crm);
    }

    internal static Contacts FromCrm(GifModels.Contact crm)
    {
      return Convert<GifModels.Contact, Contacts>(crm);
    }

    internal static GifModels.SolutionEx FromApi(SolutionEx api)
    {
      throw new NotImplementedException();
    }
  }
}
