#pragma warning disable 1591
/*
 * catalogue-api
 *
 * NHS Digital GP IT Futures Buying Catalog API
 *
 * OpenAPI spec version: 1.0.0-private-beta
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gif.Service.Models
{
    /// <summary>
    /// An Extended Solution with its corresponding Technical Contacts, ClaimedCapability, ClaimedStandard et al
    /// </summary>
    [DataContract]
    public partial class SolutionEx : EntityBase //: IEquatable<SolutionEx>
    {
        /// <summary>
        /// Solution
        /// </summary>
        /// <value>Solution</value>
        [DataMember(Name = "solution")]
        public Solution Solution { get; set; }

        /// <summary>
        /// A list of ClaimedCapability
        /// </summary>
        /// <value>A list of ClaimedCapability</value>
        [FromBody]
        [DataMember(Name = "claimedCapability")]
        public List<CapabilityImplemented> ClaimedCapability { get; set; }

        /// <summary>
        /// A list of ClaimedCapabilityEvidence
        /// </summary>
        /// <value>A list of ClaimedCapabilityEvidence</value>
        [FromBody]
        [DataMember(Name = "claimedCapabilityEvidence")]
        public List<Evidence> ClaimedCapabilityEvidence { get; set; }

        /// <summary>
        /// A list of ClaimedCapabilityReview
        /// </summary>
        /// <value>A list of ClaimedCapabilityReview</value>
        [FromBody]
        [DataMember(Name = "claimedCapabilityReview")]
        public List<Review> ClaimedCapabilityReview { get; set; }

        /// <summary>
        /// A list of ClaimedStandard
        /// </summary>
        /// <value>A list of ClaimedStandard</value>
        [FromBody]
        [DataMember(Name = "claimedStandard")]
        public List<StandardApplicable> ClaimedStandard { get; set; }

        /// <summary>
        /// A list of ClaimedStandardEvidence
        /// </summary>
        /// <value>A list of ClaimedStandardEvidence</value>
        [FromBody]
        [DataMember(Name = "claimedStandardEvidence")]
        public List<Evidence> ClaimedStandardEvidence { get; set; }

        /// <summary>
        /// A list of ClaimedStandardReview
        /// </summary>
        /// <value>A list of ClaimedStandardReview</value>
        /// A list of TechnicalContact
        [FromBody]
        [DataMember(Name = "claimedStandardReview")]
        public List<Review> ClaimedStandardReview { get; set; }

        /// <summary>
        /// A list of TechnicalContact
        /// </summary>
        /// <value>A list of TechnicalContact</value>
        [FromBody]
        [DataMember(Name = "technicalContact")]
        public List<TechnicalContact> TechnicalContact { get; set; }
        /*
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SolutionEx {\n");
            sb.Append("  Solution: ").Append(Solution).Append("\n");
            sb.Append("  ClaimedCapability: ").Append(ClaimedCapability).Append("\n");
            sb.Append("  ClaimedCapabilityEvidence: ").Append(ClaimedCapabilityEvidence).Append("\n");
            sb.Append("  ClaimedCapabilityReview: ").Append(ClaimedCapabilityReview).Append("\n");
            sb.Append("  ClaimedStandard: ").Append(ClaimedStandard).Append("\n");
            sb.Append("  ClaimedStandardEvidence: ").Append(ClaimedStandardEvidence).Append("\n");
            sb.Append("  ClaimedStandardReview: ").Append(ClaimedStandardReview).Append("\n");
            sb.Append("  TechnicalContact: ").Append(TechnicalContact).Append("\n");
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
            return obj.GetType() == GetType() && Equals((SolutionEx)obj);
        }

        /// <summary>
        /// Returns true if SolutionEx instances are equal
        /// </summary>
        /// <param name="other">Instance of SolutionEx to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SolutionEx other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    Solution == other.Solution ||
                    Solution != null &&
                    Solution.Equals(other.Solution)
                ) &&
                (
                    ClaimedCapability == other.ClaimedCapability ||
                    ClaimedCapability != null &&
                    ClaimedCapability.SequenceEqual(other.ClaimedCapability)
                ) &&
                (
                    ClaimedCapabilityEvidence == other.ClaimedCapabilityEvidence ||
                    ClaimedCapabilityEvidence != null &&
                    ClaimedCapabilityEvidence.SequenceEqual(other.ClaimedCapabilityEvidence)
                ) &&
                (
                    ClaimedCapabilityReview == other.ClaimedCapabilityReview ||
                    ClaimedCapabilityReview != null &&
                    ClaimedCapabilityReview.SequenceEqual(other.ClaimedCapabilityReview)
                ) &&
                (
                    ClaimedStandard == other.ClaimedStandard ||
                    ClaimedStandard != null &&
                    ClaimedStandard.SequenceEqual(other.ClaimedStandard)
                ) &&
                (
                    ClaimedStandardEvidence == other.ClaimedStandardEvidence ||
                    ClaimedStandardEvidence != null &&
                    ClaimedStandardEvidence.SequenceEqual(other.ClaimedStandardEvidence)
                ) &&
                (
                    ClaimedStandardReview == other.ClaimedStandardReview ||
                    ClaimedStandardReview != null &&
                    ClaimedStandardReview.SequenceEqual(other.ClaimedStandardReview)
                ) &&
                (
                    TechnicalContact == other.TechnicalContact ||
                    TechnicalContact != null &&
                    TechnicalContact.SequenceEqual(other.TechnicalContact)
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
                if (Solution != null)
                    hashCode = hashCode * 59 + Solution.GetHashCode();
                if (ClaimedCapability != null)
                    hashCode = hashCode * 59 + ClaimedCapability.GetHashCode();
                if (ClaimedCapabilityEvidence != null)
                    hashCode = hashCode * 59 + ClaimedCapabilityEvidence.GetHashCode();
                if (ClaimedCapabilityReview != null)
                    hashCode = hashCode * 59 + ClaimedCapabilityReview.GetHashCode();
                if (ClaimedStandard != null)
                    hashCode = hashCode * 59 + ClaimedStandard.GetHashCode();
                if (ClaimedStandardEvidence != null)
                    hashCode = hashCode * 59 + ClaimedStandardEvidence.GetHashCode();
                if (ClaimedStandardReview != null)
                    hashCode = hashCode * 59 + ClaimedStandardReview.GetHashCode();
                if (TechnicalContact != null)
                    hashCode = hashCode * 59 + TechnicalContact.GetHashCode();
                return hashCode;
            }
        }

        #region Operators

        public static bool operator ==(SolutionEx left, SolutionEx right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SolutionEx left, SolutionEx right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
        */
    }
}
#pragma warning restore 1591