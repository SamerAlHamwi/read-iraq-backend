using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Comments.Dto;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Comments
{
    [AbpAuthorize]
    public class SessionCommentAppService : ReadIraqAsyncCrudAppService<SessionComment, SessionCommentDto, Guid, SessionCommentDto, PagedSessionCommentResultRequestDto, CreateSessionCommentDto, UpdateSessionCommentDto>, ISessionCommentAppService
    {
        public SessionCommentAppService(IRepository<SessionComment, Guid> repository)
            : base(repository)
        {
        }

        protected override IQueryable<SessionComment> CreateFilteredQuery(PagedSessionCommentResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .Include(x => x.User)
                .Include(x => x.Replies)
                    .ThenInclude(r => r.User)
                .Where(x => x.ParentCommentId == null)
                .WhereIf(input.LessonSessionId.HasValue, x => x.LessonSessionId == input.LessonSessionId.Value)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Text.Contains(input.Keyword));
        }

        public override async Task<SessionCommentDto> CreateAsync(CreateSessionCommentDto input)
        {
            var entity = MapToEntity(input);
            entity.UserId = AbpSession.GetUserId();

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(entity);
        }

        public async Task<SessionCommentDto> ReplyAsync(Guid id, CreateSessionCommentDto input)
        {
            var parentComment = await Repository.FirstOrDefaultAsync(id);
            if (parentComment == null)
            {
                throw new UserFriendlyException(L("CommentNotFound"));
            }

            var reply = new SessionComment
            {
                LessonSessionId = parentComment.LessonSessionId,
                ParentCommentId = id,
                Text = input.Text,
                UserId = AbpSession.GetUserId()
            };

            await Repository.InsertAsync(reply);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(reply);
        }

        public override async Task<SessionCommentDto> UpdateAsync(UpdateSessionCommentDto input)
        {
            var entity = await Repository.GetAsync(input.Id);

            if (entity.UserId != AbpSession.GetUserId() && !IsAdmin())
            {
                throw new UserFriendlyException(L("OnlyAuthorOrAdminCanUpdateComment"));
            }

            MapToEntity(input, entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(entity);
        }

        public override async Task DeleteAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAsync(input.Id);

            if (entity.UserId != AbpSession.GetUserId() && !IsAdmin())
            {
                throw new UserFriendlyException(L("OnlyAuthorOrAdminCanDeleteComment"));
            }

            await Repository.DeleteAsync(input.Id);
        }

        private bool IsAdmin()
        {
            return PermissionChecker.IsGranted(Authorization.PermissionNames.Pages_Users);
        }

        protected override SessionCommentDto MapToEntityDto(SessionComment entity)
        {
            var dto = base.MapToEntityDto(entity);
            if (entity.User != null)
            {
                dto.UserName = entity.User.Name;
                dto.UserProfilePicture = entity.User.Avatar;
            }
            if (entity.Replies != null && entity.Replies.Any())
            {
                dto.Replies = entity.Replies.Select(MapToEntityDto).ToList();
            }
            return dto;
        }
    }
}
