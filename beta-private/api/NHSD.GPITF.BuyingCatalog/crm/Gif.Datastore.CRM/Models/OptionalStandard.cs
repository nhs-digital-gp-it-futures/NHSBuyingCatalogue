using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Gif.Service.Models
{
  /// <summary>
  /// A Standard and a flag associated with a Capability through a CapabilityMapping
  /// </summary>
  [DataContract]
    public class OptionalStandard
    { 
        /// <summary>
        /// Unique identifier of Standard
        /// </summary>
        /// <value>Unique identifier of Standard</value>
        [Required]
        [DataMember(Name="standardId")]
        public string StandardId { get; set; }

        /// <summary>
        /// True if the Standard does not have to be supported in order to support the Capability
        /// </summary>
        /// <value>True if the Standard does not have to be supported in order to support the Capability</value>
        [DataMember(Name="isOptional")]
        public bool? IsOptional { get; set; }
    }
}
