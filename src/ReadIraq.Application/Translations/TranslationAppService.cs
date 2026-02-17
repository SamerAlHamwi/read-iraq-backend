using Abp.Linq.Extensions;
using Abp.Domain.Repositories;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Translations;
using ReadIraq.Domain.Translations.Dto;
using System;
using System.Linq;

namespace ReadIraq.Translations
{
    public class TranslationAppService : ReadIraqAsyncCrudAppService<Translation, TranslationDto, Guid, TranslationDto, PagedTranslationResultRequestDto, CreateTranslationDto, UpdateTranslationDto>, ITranslationAppService
    {
        public TranslationAppService(IRepository<Translation, Guid> repository) : base(repository)
        {
        }

        protected override IQueryable<Translation> CreateFilteredQuery(PagedTranslationResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword) || x.Code.Contains(input.Keyword));
        }

        protected override IQueryable<Translation> ApplySorting(IQueryable<Translation> query, PagedTranslationResultRequestDto input)
        {
            return query.OrderByDescending(x => x.CreationTime);
        }
    }
}
