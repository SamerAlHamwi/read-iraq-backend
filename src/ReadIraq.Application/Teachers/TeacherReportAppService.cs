using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Teachers;
using ReadIraq.Teachers.Dto;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Teachers
{
    [AbpAuthorize]
    public class TeacherReportAppService : ReadIraqAsyncCrudAppService<TeacherReport, TeacherReportDto, Guid, TeacherReportDto, PagedTeacherReportResultRequestDto, CreateTeacherReportDto, TeacherReportDto>, ITeacherReportAppService
    {
        public TeacherReportAppService(IRepository<TeacherReport, Guid> repository) : base(repository)
        {
        }

        public override async Task<TeacherReportDto> CreateAsync(CreateTeacherReportDto input)
        {
            var report = ObjectMapper.Map<TeacherReport>(input);
            report.UserId = AbpSession.GetUserId();
            report.IsProcessed = false;

            await Repository.InsertAsync(report);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(report);
        }

        protected override IQueryable<TeacherReport> CreateFilteredQuery(PagedTeacherReportResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .Include(x => x.TeacherProfile)
                .Include(x => x.User)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Message.Contains(input.Keyword) || x.TeacherProfile.Name.Contains(input.Keyword))
                .WhereIf(input.IsProcessed.HasValue, x => x.IsProcessed == input.IsProcessed.Value);
        }
    }
}
