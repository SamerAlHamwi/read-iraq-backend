using Abp.Application.Services;
using ReadIraq.UserPreferredSubjects.Dto;
using System;

namespace ReadIraq.UserPreferredSubjects
{
    public interface IUserPreferredSubjectAppService : IAsyncCrudAppService<UserPreferredSubjectDto, Guid, PagedUserPreferredSubjectResultRequestDto, CreateUserPreferredSubjectDto, UpdateUserPreferredSubjectDto>
    {
    }
}
