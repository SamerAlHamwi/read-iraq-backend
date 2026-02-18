using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Teachers;
using ReadIraq.Teachers.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Teachers
{
    [AbpAuthorize]
    public class TeacherReviewAppService : ReadIraqAsyncCrudAppService<TeacherReview, TeacherReviewDto, Guid, TeacherReviewDto, PagedTeacherReviewResultRequestDto, CreateTeacherReviewDto, UpdateTeacherReviewDto>, ITeacherReviewAppService
    {
        public TeacherReviewAppService(IRepository<TeacherReview, Guid> repository)
            : base(repository)
        {
        }

        protected override IQueryable<TeacherReview> CreateFilteredQuery(PagedTeacherReviewResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .Include(x => x.User)
                .WhereIf(input.TeacherProfileId.HasValue, x => x.TeacherProfileId == input.TeacherProfileId.Value)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.ReviewText.Contains(input.Keyword) || x.User.Name.Contains(input.Keyword));
        }

        public override async Task<TeacherReviewDto> CreateAsync(CreateTeacherReviewDto input)
        {
            var userId = AbpSession.GetUserId();

            // Check if user already reviewed this teacher
            var existingReview = await Repository.FirstOrDefaultAsync(x => x.TeacherProfileId == input.TeacherProfileId && x.UserId == userId);

            if (existingReview != null)
            {
                // Update existing instead of creating new? Or throw error?
                // Usually "rate + review" updates the old one if it exists.
                existingReview.Rating = input.Rating;
                existingReview.ReviewText = input.ReviewText;
                await Repository.UpdateAsync(existingReview);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToEntityDto(existingReview);
            }

            var entity = MapToEntity(input);
            entity.UserId = userId;

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(entity);
        }

        public async Task<List<TeacherRatingBreakdownDto>> GetRatingBreakdownAsync(Guid teacherProfileId)
        {
            var breakdown = await Repository.GetAll()
                .Where(x => x.TeacherProfileId == teacherProfileId)
                .GroupBy(x => x.Rating)
                .Select(g => new TeacherRatingBreakdownDto
                {
                    Rating = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Ensure all ratings 1-5 are present
            for (int i = 1; i <= 5; i++)
            {
                if (!breakdown.Any(b => b.Rating == i))
                {
                    breakdown.Add(new TeacherRatingBreakdownDto { Rating = i, Count = 0 });
                }
            }

            return breakdown.OrderByDescending(b => b.Rating).ToList();
        }

        protected override TeacherReviewDto MapToEntityDto(TeacherReview entity)
        {
            var dto = base.MapToEntityDto(entity);
            if (entity.User != null)
            {
                dto.UserName = entity.User.Name;
            }
            return dto;
        }
    }
}
