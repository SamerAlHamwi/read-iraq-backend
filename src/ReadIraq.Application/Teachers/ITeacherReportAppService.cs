using Abp.Application.Services;
using ReadIraq.Domain.Teachers.Dto;
using System;

namespace ReadIraq.Domain.Teachers
{
    public interface ITeacherReportAppService : IAsyncCrudAppService<TeacherReportDto, Guid, PagedTeacherReportResultRequestDto, CreateTeacherReportDto, TeacherReportDto>
    {
    }
}
