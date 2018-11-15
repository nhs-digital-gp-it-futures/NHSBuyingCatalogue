using System.Collections.Generic;
using Gif.Service.Models;

namespace Gif.Service.Contracts
{
#pragma warning disable CS1591
  public interface ICapabilityStandardDatastore
  {
    IEnumerable<CapabilityStandard> GetAll();
  }
#pragma warning restore CS1591
}
