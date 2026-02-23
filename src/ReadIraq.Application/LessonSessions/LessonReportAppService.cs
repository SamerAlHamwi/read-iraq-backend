using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.LessonSessions.Dto;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Domain.LessonSessions
{
    [AbpAuthorize]
    public class LessonReportAppService : ReadIraqAsyncCrudAppService<LessonReport, LessonReportDto, Guid, LessonReportDto, PagedLessonReportResultRequestDto, CreateLessonReportDto, LessonReportDto>, ILessonReportAppService
    {
        public LessonReportAppService(IRepository<LessonReport, Guid> repository) : base(repository)
        {
        }

        public override async Task<LessonReportDto> CreateAsync(CreateLessonReportDto input)
        {
            var report = ObjectMapper.Map<LessonReport>(input);
            report.UserId = AbpSession.GetUserId();
            report.IsProcessed = false;

            await Repository.InsertAsync(report);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(report);
        }

        protected override IQueryable<LessonReport> CreateFilteredQuery(PagedLessonReportResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .Include(x => x.LessonSession)
                .Include(x => x.User)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Message.Contains(input.Keyword) || x.LessonSession.Title.Contains(input.Keyword))
                .WhereIf(input.IsProcessed.HasValue, x => x.IsProcessed == input.IsProcessed.Value);
        }
    }
}
