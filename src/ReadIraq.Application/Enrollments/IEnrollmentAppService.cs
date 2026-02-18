using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ReadIraq.Enrollments.Dto;
using System;

namespace ReadIraq.Enrollments
{
    public interface IEnrollmentAppService : IAsyncCrudAppService<EnrollmentDto, Guid, PagedEnrollmentResultRequestDto, CreateEnrollmentDto, UpdateEnrollmentDto>
    {
    }
}
