/*
 * catalogue-api
 *
 * NHS Digital GP IT Futures Buying Catalog API
 *
 * OpenAPI spec version: 1.0.0-private-beta
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Gif.Service.Models
{
    /// <summary>
    /// A paged list of objects
    /// </summary>
    [DataContract]
    public class PaginatedListIEnumerableStandardsApplicableReviews : IEquatable<PaginatedListIEnumerableStandardsApplicableReviews>
    {
        /// <summary>
        /// 1-based index of which page this page  Defaults to 1
        /// </summary>
        /// <value>1-based index of which page this page  Defaults to 1</value>
        [DataMember(Name = "pageIndex")]
        public int? PageIndex { get; set; }

        /// <summary>
        /// Total number of pages based on NHSD.GPITF.BuyingCatalog.Models.PaginatedList&#x60;1.PageSize
        /// </summary>
        /// <value>Total number of pages based on NHSD.GPITF.BuyingCatalog.Models.PaginatedList&#x60;1.PageSize</value>
        [DataMember(Name = "totalPages")]
        public int? TotalPages { get; set; }

        /// <summary>
        /// Maximum number of items in this page  Defaults to 20
        /// </summary>
        /// <value>Maximum number of items in this page  Defaults to 20</value>
        [DataMember(Name = "pageSize")]
        public int? PageSize { get; set; }

        /// <summary>
        /// List of items
        /// </summary>
        /// <value>List of items</value>
        [DataMember(Name = "items")]
        public IEnumerable<IEnumerable<Review>> Items { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class PaginatedListIEnumerableStandardsApplicableReviews {\n");
            sb.Append("  PageIndex: ").Append(PageIndex).Append("\n");
            sb.Append("  TotalPages: ").Append(TotalPages).Append("\n");
            sb.Append("  PageSize: ").Append(PageSize).Append("\n");
            sb.Append("  Items: ").Append(Items).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((PaginatedListIEnumerableStandardsApplicableReviews)obj);
        }

        /// <summary>
        /// Returns true if PaginatedListIEnumerableStandardsApplicableReviews instances are equal
        /// </summary>
        /// <param name="other">Instance of PaginatedListIEnumerableStandardsApplicableReviews to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(PaginatedListIEnumerableStandardsApplicableReviews other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    PageIndex == other.PageIndex ||
                    PageIndex != null &&
                    PageIndex.Equals(other.PageIndex)
                ) &&
                (
                    TotalPages == other.TotalPages ||
                    TotalPages != null &&
                    TotalPages.Equals(other.TotalPages)
                ) &&
                (
                    PageSize == other.PageSize ||
                    PageSize != null &&
                    PageSize.Equals(other.PageSize)
                ) &&
                (
                    Items == other.Items ||
                    Items != null &&
                    Items.SequenceEqual(other.Items)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                if (PageIndex != null)
                    hashCode = hashCode * 59 + PageIndex.GetHashCode();
                if (TotalPages != null)
                    hashCode = hashCode * 59 + TotalPages.GetHashCode();
                if (PageSize != null)
                    hashCode = hashCode * 59 + PageSize.GetHashCode();
                if (Items != null)
                    hashCode = hashCode * 59 + Items.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
#pragma warning disable 1591

        public static bool operator ==(PaginatedListIEnumerableStandardsApplicableReviews left, PaginatedListIEnumerableStandardsApplicableReviews right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PaginatedListIEnumerableStandardsApplicableReviews left, PaginatedListIEnumerableStandardsApplicableReviews right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591
        #endregion Operators
    }
}
