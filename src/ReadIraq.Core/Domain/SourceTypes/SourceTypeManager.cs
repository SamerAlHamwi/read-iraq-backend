using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.Attachments;
using ReadIraq.Domain.AttributeChoices;
using ReadIraq.Domain.AttributesForSourceType;
using ReadIraq.Domain.SourceTypes.Dto;
using ReadIraq.Localization.SourceFiles;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.SourceTypes
{
    public class SourceTypeManager : DomainService, ISourceTypeManager
    {
        private readonly IRepository<SourceType> _sourceRepository;
        private readonly IRepository<SourceTypeTranslation> _sourceTypeTranslation;
        private readonly IAttachmentManager _attachmentManager;
        private readonly IRepository<AttributeChoiceTranslation> _attributeChoiceTranslationRepository;
        private readonly IAttributeForSourceTypeManager _attributeForSourceTypeManager;
        private readonly AttributeChoiceManger _attributeChoiceManger;
        private readonly IRepository<AttributeForSourceTypeTranslation> _attributeForSourceTypeTranslationRepository;
        private readonly IRepository<AttributeForSourceType> _attributeForSourceRepository;
        private readonly IRepository<AttributeChoice> _attributeChoiceRepository;

        public SourceTypeManager(IRepository<SourceType> sourceRepository, IRepository<SourceTypeTranslation> sourceTypeTranslation,
            IRepository<AttributeChoiceTranslation> attributeChoiceTranslationRepository,
            IAttributeForSourceTypeManager attributeForSourceTypeManager,
            AttributeChoiceManger attributeChoiceManger, IAttachmentManager attachmentManager,
            IRepository<AttributeForSourceTypeTranslation> attributeForSourceTypeTranslationRepository,
            IRepository<AttributeForSourceType> attributeForSourceRepository, IRepository<AttributeChoice> attributeChoiceRepository)
        {
            _sourceRepository = sourceRepository;
            _sourceTypeTranslation = sourceTypeTranslation;
            _attachmentManager = attachmentManager;
            _attributeChoiceTranslationRepository = attributeChoiceTranslationRepository;
            _attributeForSourceTypeManager = attributeForSourceTypeManager;
            _attributeChoiceManger = attributeChoiceManger;
            _attributeForSourceTypeTranslationRepository = attributeForSourceTypeTranslationRepository;
            _attributeForSourceRepository = attributeForSourceRepository;
            _attributeChoiceRepository = attributeChoiceRepository;
        }

        public async Task<bool> CheckIfEntityExict(int id)
        {
            return await _sourceRepository.GetAll().AsNoTracking().AnyAsync(x => x.Id == id);
        }

        public async Task<List<SourceTypeDto>> GetAllSourceTypesIntoDto()
        {
            return ObjectMapper.Map(await _sourceRepository.GetAll().Include(x => x.Translations).ToListAsync(), new List<SourceTypeDto>());
        }

        public async Task<SourceTypeDto> GetEntityDtoByIdAsync(int id)
        {
            var sourceType = ObjectMapper.Map<SourceTypeDto>(await GetLiteEntityByIdAsync(id));
            var attachment = await _attachmentManager.GetElementByRefAsync(id, AttachmentRefType.SourceTypeIcon);
            if (attachment is not null)
            {
                sourceType.Attachment = new LiteAttachmentDto
                {
                    Id = attachment.Id,
                    Url = _attachmentManager.GetUrl(attachment),
                    LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment)
                };
            }
            return sourceType;
        }

        public async Task<SourceType> GetLiteEntityByIdAsync(int id)
        {
            var entity = await _sourceRepository.GetAll()
                       .Include(c => c.Translations).Include(x => x.Attributes).ThenInclude(x => x.Translations).Where(c => c.Id == id).FirstOrDefaultAsync();
            return entity ??
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.SourceType));
        }
        public async Task<SourceType> GetEntityByIdAsync(int id)
        {
            var entity = await _sourceRepository.GetAll().Include(x => x.Translations).Include(c => c.Attributes).ThenInclude(x => x.Translations)
                .Include(x => x.Attributes).ThenInclude(x => x.AttributeChoices).ThenInclude(x => x.Translations).AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null)
                throw new EntityNotFoundException(typeof(SourceType), id);
            return entity;
        }

        public async Task HardDeleteForEntityTranslation(List<SourceTypeTranslation> translations)
        {
            foreach (var item in translations)
            {
                await _sourceTypeTranslation.HardDeleteAsync(item);
            }
        }
        public async Task SoftDeleteForEntityTranslation(List<SourceTypeTranslation> translations)
        {
            foreach (var item in translations)
            {
                await _sourceTypeTranslation.DeleteAsync(item);
            }
        }

        public async Task DeleteAllAttributeInSourceTypeBySourceTypeId(int sourceTypeId)
        {
            var sourceType = await _sourceRepository.GetAll().Include(x => x.Attributes).ThenInclude(x => x.Translations)
                .Include(x => x.Attributes).ThenInclude(x => x.AttributeChoices).Where(x => x.Id == sourceTypeId).FirstOrDefaultAsync();
            foreach (var attrbuiteForSourceType in sourceType.Attributes)
            {
                if (attrbuiteForSourceType.AttributeChoices.Any())
                    await _attributeChoiceManger.DeleteAllAttributeChoiceInAttribute(attrbuiteForSourceType.Id);
                await _attributeForSourceTypeManager.SoftDeleteForEntityTranslation(attrbuiteForSourceType.Translations.ToList());
                await _attributeForSourceRepository.DeleteAsync(attrbuiteForSourceType);
            }
        }

        public async Task<bool> CheckIfSourceTypeIsExist(int sourceTypeId)
        {
            return await _sourceRepository.GetAll().AnyAsync(x => x.Id == sourceTypeId);
        }

        public async Task<int> GetPointsToGetRequestBySourceTypeIdAsync(int sourceTypeId)
        {
            var sourceType = await _sourceRepository.GetAsync(sourceTypeId);
            if (sourceType.IsMainForPoints)
                return sourceType.PointsToBuyRequest;
            else
                return 222222222;
        }

        public async Task<int> GetPointsToGiftCompanyBySourceTypeIdAsync(int sourceTypeId)
        {
            var sourceType = await _sourceRepository.GetAsync(sourceTypeId);
            if (sourceType.IsMainForPoints)
                return sourceType.PointsToGiftToCompany;
            else
                return 111111111;
        }

        public async Task CheckIfSourceTypeWithMainPointsToGiftOrNotByAttributeId(int attributeId)
        {
            if (await _sourceRepository.GetAll().Include(x => x.Attributes)
                .Where(x => x.Attributes.Any(x => x.Id == attributeId)).Select(x => x.IsMainForPoints).FirstOrDefaultAsync())
                throw new UserFriendlyException(Exceptions.YouCannotDoThisAction, "Source Type Is Main For Gifting Points");
        }
    }
}
