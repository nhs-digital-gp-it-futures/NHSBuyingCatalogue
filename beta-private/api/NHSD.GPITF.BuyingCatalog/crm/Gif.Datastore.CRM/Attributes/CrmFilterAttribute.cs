using System;

namespace Gif.Service.Attributes
{
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class CrmFilterAttribute : Attribute
  {
    public string Name { get; }
    public string FilterName { get; set; }
    public string FilterValue { get; set; }
    public bool? QuotesRequired { get; set; }
    public bool? MultiConditional { get; set; }

    public CrmFilterAttribute(string name)
    {
      Name = name;
    }
  }

}
