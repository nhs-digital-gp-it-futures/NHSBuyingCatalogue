#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Gif.Service.Const;
using Gif.Service.Crm;
using Gif.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gif.Service.Services
{

    public class ServiceBase
    {
        protected IRepository Repository;

        public int? Count;

        public ServiceBase(IRepository repository)
        {
            Repository = repository;
        }

        public IEnumerable<T> GetPagingValues<T>(int? pageIndex, int? pageSize, IEnumerable<T> items, out int totalPages)
        {
            var skipPage = Paging.DefaultSkip;

            if (pageIndex != null && pageIndex != 1)
                skipPage = (int)pageIndex;

            var skipValue = pageSize ?? Paging.DefaultSkip;
            pageSize = pageSize ?? Paging.DefaultPageSize;
            totalPages = (int)Math.Ceiling(Convert.ToInt32(Count) / Convert.ToDecimal(pageSize));

            skipPage--;

            if (totalPages == 0 && items.Any())
                totalPages = 1;

            return items.Skip(skipPage * skipValue).Take((int)pageSize);
        }

        protected IEnumerable<Review> OrderLinkedReviews(IEnumerable<Review> reviews)
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

        protected IEnumerable<Evidence> OrderLinkedEvidences(IEnumerable<Evidence> evidences)
        {
            var enumEvidences = evidences.ToList();
            var evidence = enumEvidences.FirstOrDefault(x => x.PreviousId == null);
            int count = enumEvidences.Count();

            if (evidence != null)
            {
                var prevEvidence = evidence;
                prevEvidence.Order = count;

                while (count > 0)
                {
                    count--;
                    prevEvidence = enumEvidences.FirstOrDefault(x => prevEvidence != null && (x.PreviousId != null && x.PreviousId.Value == prevEvidence.Id));
                    if (prevEvidence != null)
                        prevEvidence.Order = count;
                }
            }

            var orderedEvidences = enumEvidences.OrderBy(x => x.Order);
            return orderedEvidences;
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
