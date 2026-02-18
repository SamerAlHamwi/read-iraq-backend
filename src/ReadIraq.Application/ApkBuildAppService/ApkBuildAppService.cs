using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.ApkBuilds;
using ReadIraq.Domain.ApkBuilds.Dtos;
using ReadIraq.Domain.Cities.Dto;
using ReadIraq.Localization.SourceFiles;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.ApkBuildAppService
{
    public class ApkBuildAppService : ReadIraqAsyncCrudAppService<ApkBuild, ApkBuildDetailsDto, int, LiteApkBuildDto, PagedApkBuildResultRequestDto,
        CreateApkBuildDto, UpdateApkBuildDto>, IApkBuildAppService
    {
        private readonly IMapper _mapper;
        public ApkBuildAppService(IRepository<ApkBuild, int> repository, IMapper mapper) : base(repository)
        {
            _mapper = mapper;
        }

        public override async Task<PagedResultDto<LiteApkBuildDto>> GetAllAsync(PagedApkBuildResultRequestDto input)
        {
            return await base.GetAllAsync(input);
        }
        [AbpAuthorize(PermissionNames.ApkBuild_FullControl)]
        public override async Task<ApkBuildDetailsDto> CreateAsync(CreateApkBuildDto input)
        {
            var apkBuildToInsert = ObjectMapper.Map<ApkBuild>(input);
            apkBuildToInsert.UpdateOptions = UpdateOptions.Nothing;
            await Repository.InsertAsync(apkBuildToInsert);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return new ApkBuildDetailsDto()
            {
                Id = apkBuildToInsert.Id
            };
        }
        [AbpAuthorize(PermissionNames.ApkBuild_FullControl)]
        public override async Task<ApkBuildDetailsDto> UpdateAsync(UpdateApkBuildDto input)
        {
            try
            {
                var apk = await Repository.GetAsync(input.Id);
                if (apk is null)
                    throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.Entity);
                var updated = apk.UpdateOptions;
                _mapper.Map(input, apk);
                apk.UpdateOptions = updated;
                await Repository.UpdateAsync(apk);
                await UnitOfWorkManager.Current.SaveChangesAsync();
                return new ApkBuildDetailsDto();
            }
            catch (Exception ex) { throw; }
        }
        [AbpAuthorize(PermissionNames.ApkBuild_FullControl)]
        public async Task<OutPutBooleanStatuesDto> ChangeUpdateOptionsForApk(InputApkNuildStatuesDto input)
        {
            var apk = await Repository.GetAsync(input.Id);
            if (apk is null)
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.Entity);
            apk.UpdateOptions = input.UpdateOptions;
            await Repository.UpdateAsync(apk);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return new OutPutBooleanStatuesDto()
            {
                BooleanStatues = true
            };

        }
        
        protected override IQueryable<ApkBuild> ApplySorting(IQueryable<ApkBuild> query, PagedApkBuildResultRequestDto input)
        {
            return query.OrderByDescending(r => r.CreationTime);
        }
        
        [ApiExplorerSettings(IgnoreApi = true)]
        public override async Task<ApkBuildDetailsDto> GetAsync(EntityDto<int> input)
        {
            return new ApkBuildDetailsDto();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override async Task DeleteAsync(EntityDto<int> input)
        { }
    }
}
