using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Translations.Dto;
using System;

namespace ReadIraq.Translations
{
    public interface ITranslationAppService : IReadIraqAsyncCrudAppService<TranslationDto, Guid, TranslationDto, PagedTranslationResultRequestDto, CreateTranslationDto, UpdateTranslationDto>
    {
    }
}
