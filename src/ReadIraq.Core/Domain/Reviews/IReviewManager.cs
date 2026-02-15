using Abp.Domain.Services;
using ReadIraq.Domain.Cities.Dto;
using System;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Reviews
{
    public interface IReviewManager : IDomainService
    {
        Task InserReviewToCompanyOrCompanyBranch(Review review);
        Task<OutPutBooleanStatuesDto> CheckIfUserRateOfferOrNot(long userId, Guid offerId);
    }
}
