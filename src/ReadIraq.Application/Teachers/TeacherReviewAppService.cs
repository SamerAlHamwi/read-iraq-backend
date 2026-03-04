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
using ReadIraq.Domain.Attachments;
using ReadIraq.Domain.Enrollments;
using Abp.UI;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Teachers
{
    [AbpAuthorize]
    public class TeacherReviewAppService : ReadIraqAsyncCrudAppService<TeacherReview, TeacherReviewDto, Guid, TeacherReviewDto, PagedTeacherReviewResultRequestDto, CreateTeacherReviewDto, UpdateTeacherReviewDto>, ITeacherReviewAppService
    {
        private readonly IAttachmentManager _attachmentManager;
        private readonly IRepository<Enrollment, Guid> _enrollmentRepository;
        private readonly ITeacherProfileManager _teacherProfileManager;

        public TeacherReviewAppService(
            IRepository<TeacherReview, Guid> repository,
            IAttachmentManager attachmentManager,
            IRepository<Enrollment, Guid> enrollmentRepository,
            ITeacherProfileManager teacherProfileManager)
            : base(repository)
        {
            _attachmentManager = attachmentManager;
            _enrollmentRepository = enrollmentRepository;
            _teacherProfileManager = teacherProfileManager;
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

            // Check enrollment
            var isEnrolled = await _enrollmentRepository.GetAll().AnyAsync(x => x.UserId == userId && x.TeacherId == input.TeacherProfileId);
            if (!isEnrolled)
            {
                throw new UserFriendlyException("You must be enrolled in one of the teacher's subjects to leave a review.");
            }

            // Check if user already reviewed this teacher
            var existingReview = await Repository.FirstOrDefaultAsync(x => x.TeacherProfileId == input.TeacherProfileId && x.UserId == userId);

            if (existingReview != null)
            {
                existingReview.Rating = input.Rating;
                existingReview.ReviewText = input.ReviewText;
                await Repository.UpdateAsync(existingReview);
                await CurrentUnitOfWork.SaveChangesAsync();

                await _teacherProfileManager.UpdateRatingAsync(input.TeacherProfileId);

                return await MapToEntityDtoAsync(existingReview);
            }

            var entity = MapToEntity(input);
            entity.UserId = userId;

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            await _teacherProfileManager.UpdateRatingAsync(input.TeacherProfileId);

            return await MapToEntityDtoAsync(entity);
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

        public override async Task<PagedResultDto<TeacherReviewDto>> GetAllAsync(PagedTeacherReviewResultRequestDto input)
        {
            var result = await base.GetAllAsync(input);
            foreach (var dto in result.Items)
            {
                var attachment = await _attachmentManager.GetElementByRefAsync(dto.UserId.ToString(), AttachmentRefType.Profile);
                if (attachment != null)
                {
                    dto.UserAvatar = _attachmentManager.GetUrl(attachment);
                }
            }
            return result;
        }

        private async Task<TeacherReviewDto> MapToEntityDtoAsync(TeacherReview entity)
        {
            var dto = MapToEntityDto(entity);
            var attachment = await _attachmentManager.GetElementByRefAsync(entity.UserId.ToString(), AttachmentRefType.Profile);
            if (attachment != null)
            {
                dto.UserAvatar = _attachmentManager.GetUrl(attachment);
            }
            return dto;
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
