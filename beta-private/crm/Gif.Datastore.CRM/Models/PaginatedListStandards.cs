using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gif.Service.Models
{
  /// <summary>
  /// A paged list of objects
  /// </summary>
  [DataContract]
    public class PaginatedListStandards
    { 
        /// <summary>
        /// 1-based index of which page this page  Defaults to 1
        /// </summary>
        /// <value>1-based index of which page this page  Defaults to 1</value>
        [DataMember(Name="pageIndex")]
        public int? PageIndex { get; set; }

        /// <summary>
        /// Total number of pages based on NHSD.GPITF.BuyingCatalog.Models.PaginatedList&#x60;1.PageSize
        /// </summary>
        /// <value>Total number of pages based on NHSD.GPITF.BuyingCatalog.Models.PaginatedList&#x60;1.PageSize</value>
        [DataMember(Name="totalPages")]
        public int? TotalPages { get;  set; }

        /// <summary>
        /// Maximum number of items in this page  Defaults to 20
        /// </summary>
        /// <value>Maximum number of items in this page  Defaults to 20</value>
        [DataMember(Name="pageSize")]
        public int? PageSize { get;  set; }

        /// <summary>
        /// List of items
        /// </summary>
        /// <value>List of items</value>
        [DataMember(Name="items")]
        public List<Standard> Items { get; set; }
    }
}
