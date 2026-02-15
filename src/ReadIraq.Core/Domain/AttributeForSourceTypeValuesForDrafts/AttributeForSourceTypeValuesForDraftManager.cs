using Abp.Domain.Repositories;
using Abp.Domain.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Domain.AttributeForSourceTypeValuesForDrafts
{
    public class AttributeForSourceTypeValuesForDraftManager : DomainService, IAttributeForSourceTypeValuesForDraftManager
    {

        private readonly IRepository<AttributeForSourceTypeValuesForDraft> _attributeForSourceTypeValueForDraftRepository;
        private readonly IMapper _mapper;
        public AttributeForSourceTypeValuesForDraftManager(IRepository<AttributeForSourceTypeValuesForDraft> attributeForSourceTypeValueForDraftRepository, IMapper mapper)
        {
            _attributeForSourceTypeValueForDraftRepository = attributeForSourceTypeValueForDraftRepository;
            _mapper = mapper;
        }

        public async Task DeleteSourceTypeValuesForDraftByDraftId(int draftId)
        {
            var SourceTypeValues = await _attributeForSourceTypeValueForDraftRepository.GetAll().Where(x => x.DraftId == draftId).FirstOrDefaultAsync();
            await _attributeForSourceTypeValueForDraftRepository.DeleteAsync(SourceTypeValues);
        }

        public async Task DeleteSourceTypeValuesForDraftByDraftsIds(List<int> draftsIds)
        {

            var SourceTypeValues = await _attributeForSourceTypeValueForDraftRepository.GetAll().Where(x => draftsIds.Contains(x.DraftId)).ToListAsync();
            foreach (var item in SourceTypeValues)
            {
                await _attributeForSourceTypeValueForDraftRepository.DeleteAsync(item);
            }


        }

        public async Task HardDeleteEntity(List<AttributeForSourceTypeValuesForDraft> attributeForSourceTypeValuesForDraft)
        {
            try
            {
                foreach (var item in attributeForSourceTypeValuesForDraft)
                {

                    await _attributeForSourceTypeValueForDraftRepository.HardDeleteAsync(item);
                }
            }
            catch (Exception ex) { throw; }
        }
        //public async Task<List<AttributeForSourceTypeValueDto>> GetAllAttributeForSourceTypeValuesByDraftId(int draftId)
        //{
        //   var values = await _attributeForSourceTypeValueForDraftRepository.GetAll().AsNoTracking()
        //        .Include(x => x.AttributeForSourcType).ThenInclude(x => x.Translations).AsNoTracking()
        //        .Include(x => x.AttributeChoice).ThenInclude(x => x.Translations).AsNoTracking()
        //        .Where(x => x.DraftId == draftId).ToListAsync();
        //    var valuesDto = new List<AttributeForSourceTypeValueDto>();
        //    valuesDto = _mapper.Map<List<AttributeForSourceTypeValuesForDraft>, List<AttributeForSourceTypeValueDto>>(values);
        //    return valuesDto;
        //}
    }
}