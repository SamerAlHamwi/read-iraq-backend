using Abp.Application.Services;
using ReadIraq.UserSessionProgresses.Dto;
using System;

namespace ReadIraq.UserSessionProgresses
{
    public interface IUserSessionProgressAppService : IAsyncCrudAppService<UserSessionProgressDto, Guid, PagedUserSessionProgressResultRequestDto, CreateUserSessionProgressDto, UpdateUserSessionProgressDto>
    {
    }
}
