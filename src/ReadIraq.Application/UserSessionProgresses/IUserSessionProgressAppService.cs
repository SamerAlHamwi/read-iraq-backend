using Abp.Application.Services;
using ReadIraq.UserSessionProgresses.Dto;
using System;
using System.Threading.Tasks;

namespace ReadIraq.UserSessionProgresses
{
    public interface IUserSessionProgressAppService : IAsyncCrudAppService<UserSessionProgressDto, Guid, PagedUserSessionProgressResultRequestDto, CreateUserSessionProgressDto, UpdateUserSessionProgressDto>
    {
        Task<UserProgressSummaryDto> GetSummaryAsync(long userId);
        Task UpdateProgressAsync(UpdateUserSessionProgressDto input);
        Task<SubjectProgressDto> GetSubjectProgressAsync(Guid subjectId, long userId);
    }
}
