using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System;
using System.Linq;
using System.Reflection;
using GifModels = Gif.Service.Models;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public static class Creator
  {
    private static TTarget Convert<TSource, TTarget>(TSource source) where TTarget : new()
    {
      const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

      var target = new TTarget();
      var sourceProps = source.GetType().GetProperties(bindingFlags);
      var targetProps = target.GetType().GetProperties(bindingFlags);

      foreach (var targetProp in targetProps)
      {
        if (!targetProp.CanWrite)
        {
          continue;
        }

        var sourceProp = sourceProps.SingleOrDefault(prop => prop.Name.Equals(targetProp.Name, StringComparison.InvariantCultureIgnoreCase));
        if (sourceProp == null)
        {
          continue;
        }

        if ((sourceProp.PropertyType == typeof(Guid) ||
            sourceProp.PropertyType == typeof(Guid?)) &&
          targetProp.PropertyType == typeof(string))
        {
          targetProp.SetValue(target, (sourceProp.GetValue(source) as Guid?)?.ToString());
          continue;
        }

        if (sourceProp.PropertyType == typeof(string) &&
          (targetProp.PropertyType == typeof(Guid) ||
            targetProp.PropertyType == typeof(Guid?)))
        {
          var sourceGuidStr = (string)sourceProp.GetValue(source);
          if (!string.IsNullOrEmpty(sourceGuidStr))
          {
            targetProp.SetValue(target, Guid.Parse(sourceGuidStr));
          }
          continue;
        }

        if ((sourceProp.PropertyType.IsEnum ||
            (Nullable.GetUnderlyingType(sourceProp.PropertyType)?.IsEnum ?? false)) &&
          (targetProp.PropertyType.IsEnum ||
          (Nullable.GetUnderlyingType(targetProp.PropertyType)?.IsEnum ?? false)))
        {
          var sourceVal = sourceProp.GetValue(source).ToString();
          var targetPropType = targetProp.PropertyType.IsEnum ? targetProp.PropertyType : Nullable.GetUnderlyingType(targetProp.PropertyType);
          var targetVal = Enum.Parse(targetPropType, sourceVal);

          targetProp.SetValue(target, targetVal);
          continue;
        }

        targetProp.SetValue(target, sourceProp.GetValue(source));
      }

      return target;
    }

    public static GifModels.Review FromApi(StandardsApplicableReviews api)
    {
      return Convert<StandardsApplicableReviews, GifModels.Review>(api);
    }

    public static StandardsApplicableReviews StandardsApplicableReviewsFromCrm(GifModels.Review crm)
    {
      return Convert<GifModels.Review, StandardsApplicableReviews>(crm);
    }

    public static CapabilitiesImplementedReviews CapabilitiesImplementedReviewsFromCrm(GifModels.Review crm)
    {
      return Convert<GifModels.Review, CapabilitiesImplementedReviews>(crm);
    }

    public static GifModels.Review FromApi(CapabilitiesImplementedReviews api)
    {
      return Convert<CapabilitiesImplementedReviews, GifModels.Review>(api);
    }

    public static CapabilitiesImplementedEvidence FromCrm(GifModels.CapabilityEvidence crm)
    {
      return Convert<GifModels.CapabilityEvidence, CapabilitiesImplementedEvidence>(crm);
    }

    public static GifModels.CapabilityEvidence FromApi(CapabilitiesImplementedEvidence api)
    {
      return Convert<CapabilitiesImplementedEvidence, GifModels.CapabilityEvidence>(api);
    }

    public static GifModels.StandardApplicableEvidence FromApi(StandardsApplicableEvidence api)
    {
      return Convert<StandardsApplicableEvidence, GifModels.StandardApplicableEvidence>(api);
    }

    public static StandardsApplicableEvidence FromCrm(GifModels.StandardApplicableEvidence crm)
    {
      return Convert<GifModels.StandardApplicableEvidence, StandardsApplicableEvidence>(crm);
    }

    public static GifModels.StandardApplicable FromApi(StandardsApplicable api)
    {
      return Convert<StandardsApplicable, GifModels.StandardApplicable>(api);
    }

    public static StandardsApplicable FromCrm(GifModels.StandardApplicable crm)
    {
      return Convert<GifModels.StandardApplicable, StandardsApplicable>(crm);
    }

    public static GifModels.CapabilityImplemented FromApi(CapabilitiesImplemented api)
    {
      return Convert<CapabilitiesImplemented, GifModels.CapabilityImplemented>(api);
    }

    public static CapabilitiesImplemented FromCrm(GifModels.CapabilityImplemented crm)
    {
      return Convert<GifModels.CapabilityImplemented, CapabilitiesImplemented>(crm);
    }

    public static Frameworks FromCrm(GifModels.Framework crm)
    {
      return Convert<GifModels.Framework, Frameworks>(crm);
    }

    public static TechnicalContacts FromCrm(GifModels.TechnicalContact crm)
    {
      return Convert<GifModels.TechnicalContact, TechnicalContacts>(crm);
    }

    public static GifModels.TechnicalContact FromApi(TechnicalContacts api)
    {
      return Convert<TechnicalContacts, GifModels.TechnicalContact>(api);
    }

    public static Solutions FromCrm(GifModels.Solution crm)
    {
      return Convert<GifModels.Solution, Solutions>(crm);
    }

    public static GifModels.Solution FromApi(Solutions api)
    {
      return Convert<Solutions, GifModels.Solution>(api);
    }

    public static Capabilities FromCrm(GifModels.Capability crm)
    {
      return Convert<GifModels.Capability, Capabilities>(crm);
    }

    public static Standards FromCrm(GifModels.Standard crm)
    {
      return Convert<GifModels.Standard, Standards>(crm);
    }

    public static Organisations FromCrm(GifModels.Organisation crm)
    {
      return Convert<GifModels.Organisation, Organisations>(crm);
    }

    public static CapabilityStandard FromCrm(GifModels.CapabilityStandard crm)
    {
      return Convert<GifModels.CapabilityStandard, CapabilityStandard>(crm);
    }

    public static Contacts FromCrm(GifModels.Contact crm)
    {
      return Convert<GifModels.Contact, Contacts>(crm);
    }

    public static SolutionEx FromCrm(GifModels.SolutionEx crm)
    {
      var retval = new SolutionEx
      {
        Solution = Convert<GifModels.Solution, Solutions>(crm.Solution),

        ClaimedCapability = crm.ClaimedCapability
          .Select(claim => Convert<GifModels.CapabilityImplemented, CapabilitiesImplemented>(claim))
          .ToList(),
        ClaimedCapabilityEvidence = crm.ClaimedCapabilityEvidence
          .Select(evidence => Convert<GifModels.CapabilityEvidence, CapabilitiesImplementedEvidence>(evidence))
          .ToList(),
        ClaimedCapabilityReview = crm.ClaimedCapabilityReview
          .Select(review => Convert<GifModels.Review, CapabilitiesImplementedReviews>(review))
          .ToList(),

        ClaimedStandard = crm.ClaimedStandard
          .Select(claim => Convert<GifModels.StandardApplicable, StandardsApplicable>(claim))
          .ToList(),
        ClaimedStandardEvidence = crm.ClaimedStandardEvidence
          .Select(evidence => Convert<GifModels.StandardApplicableEvidence, StandardsApplicableEvidence>(evidence))
          .ToList(),
        ClaimedStandardReview = crm.ClaimedStandardReview
          .Select(review => Convert<GifModels.Review, StandardsApplicableReviews>(review))
          .ToList(),

        TechnicalContact = crm.TechnicalContact
          .Select(techCont => Convert<GifModels.TechnicalContact, TechnicalContacts>(techCont))
          .ToList()
      };

      return retval;
    }

    public static GifModels.SolutionEx FromApi(SolutionEx api)
    {
      var retval = new GifModels.SolutionEx
      {
        Solution = Convert<Solutions, GifModels.Solution>(api.Solution),

        ClaimedCapability = api.ClaimedCapability
          .Select(claim => Convert<CapabilitiesImplemented, GifModels.CapabilityImplemented>(claim))
          .ToList(),
        ClaimedCapabilityEvidence = api.ClaimedCapabilityEvidence
          .Select(evidence => Convert<CapabilitiesImplementedEvidence, GifModels.CapabilityEvidence>(evidence))
          .ToList(),
        ClaimedCapabilityReview = api.ClaimedCapabilityReview
          .Select(review => Convert<CapabilitiesImplementedReviews, GifModels.Review>(review))
          .ToList(),

        ClaimedStandard = api.ClaimedStandard
          .Select(claim => Convert<StandardsApplicable, GifModels.StandardApplicable>(claim))
          .ToList(),
        ClaimedStandardEvidence = api.ClaimedStandardEvidence
          .Select(evidence => Convert<StandardsApplicableEvidence, GifModels.StandardApplicableEvidence>(evidence))
          .ToList(),
        ClaimedStandardReview = api.ClaimedStandardReview
          .Select(review => Convert<StandardsApplicableReviews, GifModels.Review>(review))
          .ToList(),

        TechnicalContact = api.TechnicalContact
          .Select(techCont => Convert<TechnicalContacts, GifModels.TechnicalContact>(techCont))
          .ToList()
      };

      return retval;
    }
  }
}
