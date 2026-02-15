using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Uow;
using Abp.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Cities.Dto;
using ReadIraq.Domain.FrequentlyQuestions;
using ReadIraq.Domain.FrequentlyQuestions.Dto;
using ReadIraq.EntityFrameworkCore;
using ReadIraq.FrequentlyQuestions;
using ReadIraq.Localization.SourceFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.FrequentlyQuestionService
{
    public class FrequentlyQuestionAppService : ReadIraqAsyncCrudAppService<FrequentlyQuestion, FrequentlyQuestionDetailsDto, int, LiteFrequentlyQuestionDto, PagedFrequentlyQuestionResultRequestDto,
         CreateFrequentlyQuestionDto, UpdateFrequentlyQuestionDto>, IFrequentlyQuestionAppService
    {
        private readonly IFrequentlyQuestionManager _frequentlyQuestionManager;
        public FrequentlyQuestionAppService(IRepository<FrequentlyQuestion, int> repository, IFrequentlyQuestionManager frequentlyQuestionManager) : base(repository)
        {
            _frequentlyQuestionManager = frequentlyQuestionManager;
        }
        public override async Task<FrequentlyQuestionDetailsDto> GetAsync(EntityDto<int> input)
        {
            var frequentlyQuestion = await _frequentlyQuestionManager.GetEntityByIdAsync(input.Id);
            if (frequentlyQuestion is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.FrequentlyQuestion));
            return MapToEntityDto(frequentlyQuestion);
        }
        public override async Task<PagedResultDto<LiteFrequentlyQuestionDto>> GetAllAsync(PagedFrequentlyQuestionResultRequestDto input)
        {
            var result = await base.GetAllAsync(input);
            return result;
        }
        [AbpAuthorize(PermissionNames.FrequencyQuestion_FullControl)]
        public override async Task<FrequentlyQuestionDetailsDto> CreateAsync(CreateFrequentlyQuestionDto input)
        {
            var Translation = ObjectMapper.Map<List<FrequentlyQuestionTranslation>>(input.Translations);
            var frequentlyQuestion = ObjectMapper.Map<FrequentlyQuestion>(input);
            frequentlyQuestion.IsActive = true;
            await Repository.InsertAsync(frequentlyQuestion);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return ObjectMapper.Map<FrequentlyQuestionDetailsDto>(frequentlyQuestion);

        }
        [AbpAuthorize(PermissionNames.FrequencyQuestion_FullControl)]
        public override async Task<FrequentlyQuestionDetailsDto> UpdateAsync(UpdateFrequentlyQuestionDto input)
        {
            var frequentlyQuestion = await _frequentlyQuestionManager.GetEntityByIdAsync(input.Id);
            if (frequentlyQuestion is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.FrequentlyQuestion));
            frequentlyQuestion.Translations.Clear();
            MapToEntity(input, frequentlyQuestion);
            frequentlyQuestion.LastModificationTime = DateTime.UtcNow;
            await Repository.UpdateAsync(frequentlyQuestion);
            return MapToEntityDto(frequentlyQuestion);
        }
        [AbpAuthorize(PermissionNames.FrequencyQuestion_FullControl)]
        public override async Task DeleteAsync(EntityDto<int> input)
        {
            var frequentlyQuestion = await _frequentlyQuestionManager.GetEntityByIdAsync(input.Id);
            if (frequentlyQuestion is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.FrequentlyQuestion));
            await Repository.HardDeleteAsync(frequentlyQuestion);
        }

        protected override IQueryable<FrequentlyQuestion> CreateFilteredQuery(PagedFrequentlyQuestionResultRequestDto input)
        {
            var data = base.CreateFilteredQuery(input);
            data = data.Include(x => x.Translations);
            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                var keyword = input.Keyword.ToLower();
                data = data.Where(x => x.Translations.Any(x => x.Question.ToLower().Contains(keyword)
                || x.Answer.ToLower().Contains(keyword)));
            }
            if (input.App.HasValue)
                data = data.Where(x => x.App == input.App.Value || x.App == AppType.Both);
            if (input.IsActive.HasValue)
                data = data.Where(x => x.IsActive == input.IsActive.Value);
            return data;

        }
        protected override IQueryable<FrequentlyQuestion> ApplySorting(IQueryable<FrequentlyQuestion> query, PagedFrequentlyQuestionResultRequestDto input)
        {
            return query.OrderBy(x => x.Id);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.FrequencyQuestion_FullControl)]
        public async Task<FrequentlyQuestionDetailsDto> SwitchActivationAsync(SwitchActivationInputDto input)
        {
            CheckUpdatePermission();
            var entity = await Repository.GetAsync(input.Id);
            entity.IsActive = input.IsActive;
            await Repository.UpdateAsync(entity);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return MapToEntityDto(entity);
        }
        /// <summary>
        /// Call this EndPoint Only When To UpdateDatabase
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Tags("Update-DataBase")]
        public async Task<object> MigrateDatabaseAsync()
        {
            try
            {

                UnitOfWorkManager.Current.GetDbContext<ReadIraqDbContext>().Database.Migrate();
                return new UserFriendlyException(200, "Update Database Complete  Successfully");
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message + " " + ex.InnerException);
            }
        }
    }

}
