#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Gif.Service.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Gif.Service.Models
{
    [CrmEntity("cc_reviews")]
    [DataContract]
    public class Review : EntityBase
    {

        [DataMember]
        [CrmIdField]
        [CrmFieldName("cc_reviewid")]
        public Guid Id { get; set; }

        [DataMember]
        [CrmFieldName("cc_name")]
        public string Name { get; set; }

        [DataMember]
        [CrmFieldName("_cc_reviewcontactsid_value")]
        [CrmFieldNameDataBind("cc_ReviewContactsId@odata.bind")]
        [CrmFieldEntityDataBind("contacts")]
        public Guid? ReviewContacts { get; set; }

        [DataMember]
        [CrmFieldName("_cc_evidence_value")]
        [CrmFieldNameDataBind("cc_Evidence@odata.bind")]
        [CrmFieldEntityDataBind("cc_evidences")]
        public Guid? Evidence { get; set; }

        [DataMember]
        [CrmFieldName("_cc_createdbyid_value")]
        [CrmFieldNameDataBind("cc_CreatedByID@odata.bind")]
        [CrmFieldEntityDataBind("contacts")]
        public Guid? CreatedById { get; set; }

        [DataMember]
        [CrmFieldName("_cc_previousversion_value")]
        [CrmFieldNameDataBind("cc_PreviousVersion@odata.bind")]
        [CrmFieldEntityDataBind("cc_reviews")]
        public Guid? PreviousId { get; set; }

        public int Order { get; set; }

        public Review() { }

        public Review(JToken token) : base(token)
        {
        }

        public static IEnumerable<Review> OrderLinkedReviews(IEnumerable<Review> reviews)
        {
            var enumReviews = reviews.ToList();
            var review = enumReviews.FirstOrDefault(x => x.PreviousId == null);
            int count = enumReviews.Count();

            if (review != null)
            {
                var prevReview = review;
                prevReview.Order = count;

                while (count > 0)
                {
                    count--;
                    prevReview = enumReviews.FirstOrDefault(x => prevReview != null && (x.PreviousId != null && x.PreviousId.Value == prevReview.Id));
                    if (prevReview != null)
                        prevReview.Order = count;
                }
            }

            var orderedReviews = enumReviews.OrderBy(x => x.Order);
            return orderedReviews;
        }


    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
