using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
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
using Abp.Domain.Uow;
using Microsoft.AspNetCore.Mvc;

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
            var userId = AbpSession.GetUserId();
            input.UserId = userId; // Always set to current user ID

            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var existingItem = await Repository.FirstOrDefaultAsync(x => x.UserId == userId && x.ItemType == input.ItemType && x.ItemId == input.ItemId);
                if (existingItem != null)
                {
                    if (existingItem.IsDeleted)
                    {
                        existingItem.IsDeleted = false;
                        existingItem.DeletionTime = null;
                        existingItem.DeleterUserId = null;
                        await Repository.UpdateAsync(existingItem);
                        await CurrentUnitOfWork.SaveChangesAsync();
                        return MapToEntityDto(existingItem);
                    }
                    throw new UserFriendlyException(L("AlreadySaved"));
                }
            }

            // Verify if the item exists before saving
            if (input.ItemType == SavedItemType.Teacher)
            {
                var teacherExists = await _teacherRepository.GetAll().AnyAsync(x => x.Id == input.ItemId);
                if (!teacherExists)
                {
                    throw new UserFriendlyException(L("TeacherNotFound"));
                }
            }
            else if (input.ItemType == SavedItemType.Session)
            {
                var lessonExists = await _lessonRepository.GetAll().AnyAsync(x => x.Id == input.ItemId);
                if (!lessonExists)
                {
                    throw new UserFriendlyException(L("LessonNotFound"));
                }
            }

            return await base.CreateAsync(input);
        }

        [HttpPost]
        public async Task UnsaveAsync(UnsaveItemInput input)
        {
            var userId = AbpSession.GetUserId();
            var item = await Repository.FirstOrDefaultAsync(x => x.UserId == userId && x.ItemType == input.ItemType && x.ItemId == input.ItemId);
            if (item != null)
            {
                await Repository.DeleteAsync(item.Id);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }

        protected override IQueryable<UserSavedItem> CreateFilteredQuery(PagedUserSavedItemResultRequestDto input)
        {
            var userId = AbpSession.UserId;
            return base.CreateFilteredQuery(input)
                .WhereIf(userId.HasValue, x => x.UserId == userId.Value) // Default to current user's items
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
                    var teacher = await _teacherRepository.FirstOrDefaultAsync(item.ItemId);
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
