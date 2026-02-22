using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.SavedItems;
using ReadIraq.Domain.LessonSessions;
using ReadIraq.Domain.Teachers;
using ReadIraq.Domain.Attachments;
using ReadIraq.SavedItems.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.SavedItems
{
    [AbpAuthorize]
    public class UserSavedItemAppService : ReadIraqAsyncCrudAppService<UserSavedItem, UserSavedItemDto, Guid, LiteUserSavedItemDto,
        PagedUserSavedItemResultRequestDto, CreateUserSavedItemDto, UpdateUserSavedItemDto>,
        IUserSavedItemAppService
    {
        private readonly IRepository<LessonSession, Guid> _lessonRepository;
        private readonly IRepository<TeacherProfile, Guid> _teacherRepository;
        private readonly IAttachmentManager _attachmentManager;

        public UserSavedItemAppService(
            IRepository<UserSavedItem, Guid> repository,
            IRepository<LessonSession, Guid> lessonRepository,
            IRepository<TeacherProfile, Guid> teacherRepository,
            IAttachmentManager attachmentManager)
            : base(repository)
        {
            _lessonRepository = lessonRepository;
            _teacherRepository = teacherRepository;
            _attachmentManager = attachmentManager;
        }

        public override async Task<UserSavedItemDto> CreateAsync(CreateUserSavedItemDto input)
        {
            var exists = await Repository.GetAll().AnyAsync(x => x.UserId == input.UserId && x.ItemType == input.ItemType && x.ItemId == input.ItemId);
            if (exists)
            {
                throw new UserFriendlyException(L("AlreadySaved"));
            }

            return await base.CreateAsync(input);
        }

        protected override IQueryable<UserSavedItem> CreateFilteredQuery(PagedUserSavedItemResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .WhereIf(input.UserId.HasValue, x => x.UserId == input.UserId.Value)
                .WhereIf(input.ItemType.HasValue, x => x.ItemType == input.ItemType.Value)
                .WhereIf(input.ItemId.HasValue, x => x.ItemId == input.ItemId.Value);
        }

        public override async Task<PagedResultDto<LiteUserSavedItemDto>> GetAllAsync(PagedUserSavedItemResultRequestDto input)
        {
            var result = await base.GetAllAsync(input);

            foreach (var item in result.Items)
            {
                if (item.ItemType == SavedItemType.Session)
                {
                    var lesson = await _lessonRepository.GetAll()
                        .Include(x => x.Subject).ThenInclude(x => x.Name)
                        .Include(x => x.TeacherProfile)
                        .FirstOrDefaultAsync(x => x.Id == item.ItemId);

                    if (lesson != null)
                    {
                        item.Title = lesson.Title;
                        item.TeacherName = lesson.TeacherProfile?.Name;
                        item.SubjectName = lesson.Subject?.Name?.FirstOrDefault()?.Name;
                        item.DurationText = $"{(lesson.DurationSeconds / 60):D2}:{(lesson.DurationSeconds % 60):D2}";

                        var thumbnail = await _attachmentManager.GetElementByRefAsync(lesson.Id.ToString(), AttachmentRefType.LessonSessionThumbnail);
                        if (thumbnail != null)
                        {
                            item.ImageUrl = _attachmentManager.GetUrl(thumbnail);
                        }

                        if (lesson.TeacherProfileId != Guid.Empty)
                        {
                            var teacherImg = await _attachmentManager.GetElementByRefAsync(lesson.TeacherProfileId.ToString(), AttachmentRefType.TeacherProfile);
                            if (teacherImg != null)
                            {
                                item.TeacherImageUrl = _attachmentManager.GetUrl(teacherImg);
                            }
                        }
                    }
                }
                else if (item.ItemType == SavedItemType.Teacher)
                {
                    var teacher = await _teacherRepository.GetAsync(item.ItemId);
                    if (teacher != null)
                    {
                        item.Title = teacher.Name;
                        item.Department = teacher.Specialization;
                        item.Rating = (double)teacher.AverageRating;

                        var teacherImg = await _attachmentManager.GetElementByRefAsync(teacher.Id.ToString(), AttachmentRefType.TeacherProfile);
                        if (teacherImg != null)
                        {
                            item.ImageUrl = _attachmentManager.GetUrl(teacherImg);
                        }
                    }
                }
            }

            return result;
        }
    }
}
