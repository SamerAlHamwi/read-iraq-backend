using Abp.Application.Services;
using ReadIraq.Teachers.Dto;
using System;

namespace ReadIraq.Teachers
{
    public interface ITeacherReportAppService : IAsyncCrudAppService<TeacherReportDto, Guid, PagedTeacherReportResultRequestDto, CreateTeacherReportDto, TeacherReportDto>
    {
    }
}
