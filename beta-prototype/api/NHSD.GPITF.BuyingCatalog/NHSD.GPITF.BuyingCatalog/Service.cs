﻿using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NHSD.GPITF.BuyingCatalog
{
  internal sealed class Service
  {
    public string ServiceType { get; set; }

    public string ImplementationType { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public ServiceLifetime Lifetime { get; set; }
  }
}
