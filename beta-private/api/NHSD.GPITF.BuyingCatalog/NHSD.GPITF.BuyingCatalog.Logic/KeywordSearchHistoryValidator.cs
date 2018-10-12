using Microsoft.AspNetCore.Http;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class KeywordSearchHistoryValidator : ValidatorBase<object>, IKeywordSearchHistoryValidator
  {
    public KeywordSearchHistoryValidator(IHttpContextAccessor context) :
      base(context)
    {
      MustBeAdminOrSupplier();
    }
  }
}
