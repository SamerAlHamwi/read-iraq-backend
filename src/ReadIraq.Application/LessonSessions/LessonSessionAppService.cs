using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.LessonSessions;
using ReadIraq.LessonSessions.Dto;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.LessonSessions
{
    [AbpAuthorize]
    public class LessonSessionAppService : ReadIraqAsyncCrudAppService<LessonSession, LessonSessionDto, Guid, LiteLessonSessionDto,
        PagedLessonSessionResultRequestDto, CreateLessonSessionDto, UpdateLessonSessionDto>,
        ILessonSessionAppService
    {
        public LessonSessionAppService(IRepository<LessonSession, Guid> repository)
            : base(repository)
        {
        }

        protected override IQueryable<LessonSession> CreateFilteredQuery(PagedLessonSessionResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword),
                    x => x.Title.Contains(input.Keyword) || x.Description.Contains(input.Keyword))
                .WhereIf(input.TeacherProfileId.HasValue, x => x.TeacherProfileId == input.TeacherProfileId.Value);
        }

        public override async Task<LessonSessionDto> GetAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAll()
                .Include(x => x.Attachments)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            var dto = MapToEntityDto(entity);
            dto.AttachmentIds = entity.Attachments.Select(a => a.AttachmentId).ToList();
            return dto;
        }

        public override async Task<LessonSessionDto> CreateAsync(CreateLessonSessionDto input)
        {
            CheckCreatePermission();

            var entity = MapToEntity(input);
            entity.ViewsCount = 0;
            entity.LikesCount = 0;

            if (input.AttachmentIds != null)
            {
                foreach (var attachmentId in input.AttachmentIds.Distinct())
                {
                    entity.Attachments.Add(new LessonSessionAttachment { AttachmentId = attachmentId });
                }
            }

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            var dto = MapToEntityDto(entity);
            dto.AttachmentIds = entity.Attachments.Select(a => a.AttachmentId).ToList();
            return dto;
        }

        public override async Task<LessonSessionDto> UpdateAsync(UpdateLessonSessionDto input)
        {
            CheckUpdatePermission();

            var entity = await Repository.GetAll()
                .Include(x => x.Attachments)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            MapToEntity(input, entity);

            entity.Attachments.Clear();
            if (input.AttachmentIds != null)
            {
                foreach (var attachmentId in input.AttachmentIds.Distinct())
                {
                    entity.Attachments.Add(new LessonSessionAttachment
                    {
                        LessonSessionId = entity.Id,
                        AttachmentId = attachmentId
                    });
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            var dto = MapToEntityDto(entity);
            dto.AttachmentIds = entity.Attachments.Select(a => a.AttachmentId).ToList();
            return dto;
        }
    }
}

