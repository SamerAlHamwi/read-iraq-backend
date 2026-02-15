using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.Cities.Dto;
using System;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Reviews
{
    public class ReviewManager : DomainService, IReviewManager
    {
        private readonly IRepository<Review> _reviewRepository;

        public ReviewManager(IRepository<Review> repository)
        {
            _reviewRepository = repository;
        }

        public async Task<OutPutBooleanStatuesDto> CheckIfUserRateOfferOrNot(long userId, Guid offerId)
        {
            if (!await _reviewRepository.GetAll().AnyAsync(x => x.OfferId == offerId && x.UserId == userId))
                return new OutPutBooleanStatuesDto { BooleanStatues = false };
            return new OutPutBooleanStatuesDto { BooleanStatues = true };
        }

        public async Task InserReviewToCompanyOrCompanyBranch(Review review)
        {
            await _reviewRepository.InsertAndGetIdAsync(review);
        }
    }
}
