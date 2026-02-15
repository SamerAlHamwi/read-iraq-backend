using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.Attachments;
using ReadIraq.Domain.AttributeChoices.Dto;
using ReadIraq.Domain.RequestForQuotations.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Domain.AttributeAndAttachments
{
    public class AttributeChoiceAndAttachmentManager : DomainService, IAttributeChoiceAndAttachmentManager
    {
        private readonly IRepository<AttributeChoiceAndAttachment> _attributeChoiceAndAttachmentRepository;
        private readonly AttachmentManager _attachmentManager;
        public AttributeChoiceAndAttachmentManager(IRepository<AttributeChoiceAndAttachment> attributeChoiceAndAttachmentRepository, AttachmentManager attachmentManager)
        {
            _attributeChoiceAndAttachmentRepository = attributeChoiceAndAttachmentRepository;
            _attachmentManager = attachmentManager;
        }

        public async Task<List<AttributeChoiceAndAttachmentDetailsDto>> GetAttributeChoiceAndAttachmentDetailsAsyncByRequestId(long requestId)
        {
            List<AttributeChoiceAndAttachment> attributeAndAttachments = await _attributeChoiceAndAttachmentRepository.GetAll()
                .AsNoTracking()
                .Include(c => c.AttributeChoice).ThenInclude(x => x.Translations)
                .Include(c => c.Attachments)
                .Where(x => x.RequestForQuotationId == requestId)
                .ToListAsync();
            List<AttributeChoiceAndAttachmentDetailsDto> detailsDtos = new List<AttributeChoiceAndAttachmentDetailsDto>();
            foreach (var item in attributeAndAttachments)
            {
                List<LiteAttachmentDto> liteAttachmentDtos = new List<LiteAttachmentDto>();

                foreach (var attachment in item.Attachments)
                {
                    liteAttachmentDtos.Add(new LiteAttachmentDto
                    {
                        Id = attachment.Id,
                        Url = _attachmentManager.GetUrl(attachment),
                        LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment),
                    });
                }
                detailsDtos.Add(new AttributeChoiceAndAttachmentDetailsDto
                {
                    AttributeChoice = ObjectMapper.Map<AttributeChoiceDetailsDto>(item.AttributeChoice),
                    Attachments = liteAttachmentDtos
                });
            }
            //foreach (var id in attributeChoiceIds)
            //{
            //    var attributeChoice = attributeAndAttachments.Where(x => x.AttributeChoice.AttributeChociceParentId == id).Select(x => x.AttributeChoice).FirstOrDefault();
            //    var attachments = attributeAndAttachments.Where(x => x.AttributeChoice.AttributeChociceParentId == id).SelectMany(x => x.Attachments).ToList();
            //    List<LiteAttachmentDto> liteAttachmentDtos = new List<LiteAttachmentDto>();
            //    foreach (var attachment in attachments)
            //    {
            //        liteAttachmentDtos.Add(new LiteAttachmentDto
            //        {
            //            Id = attachment.Id,
            //            Url = _attachmentManager.GetUrl(attachment),
            //            LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment),
            //        });

            //    }
            //    detailsDtos.Add(new AttributeChoiceAndAttachmentDetailsDto
            //    {
            //        AttributeChoice = ObjectMapper.Map<AttributeChoiceDetailsDto>(attributeChoice),
            //        Attachments = liteAttachmentDtos

            //    });
            //}
            return detailsDtos;

        }
    }
}
