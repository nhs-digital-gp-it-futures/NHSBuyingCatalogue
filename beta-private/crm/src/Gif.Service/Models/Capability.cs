using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Gif.Service.Attributes;
using Gif.Service.Const;
using Gif.Service.Enums;
using Newtonsoft.Json.Linq;

namespace Gif.Service.Models
{
    [CrmEntity("cc_capabilities")]
    [DataContract]
    public class Capability : EntityBase
    {
        [DataMember]
        [CrmIdField]
        [CrmFieldName("cc_capabilityid")]
        public Guid Id { get; set; }

        [DataMember]
        [CrmFieldName("cc_name")]
        public string Name { get; set; }

        [DataMember]
        [CrmFieldName("cc_description")]
        public string Description { get; set; }

        [DataMember]
        [CrmFieldName("cc_url")]
        public string Url { get; set; }

        [CrmEntityRelationAttribute(RelationshipNames.CapabilityFramework)]
        public IList<Framework> Frameworks { get; set; }

        public Capability() { }

        public Capability(JToken token) : base(token)
        {
        }
    }
}


