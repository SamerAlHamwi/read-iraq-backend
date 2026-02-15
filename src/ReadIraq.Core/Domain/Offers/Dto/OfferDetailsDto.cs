using Abp.Application.Services.Dto;
using ReadIraq.Domain.SelectedCompaniesByUsers.Dto;
using ReadIraq.Domain.services.Dto;
using System;
using System.Collections.Generic;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Offers.Dto
{
    public class OfferDetailsDto : EntityDto<Guid>
    {

        public OfferStatues Statues { get; set; }
        public OfferProvider Provider { get; set; }
        public double Price { get; set; }
        public SelectedCompaniesBySystemForRequestDto SelectedCompanies { get; set; }
        public List<ServiceDetailsDto> ServiceValueForOffers { get; set; }
        public string Note { get; set; }
        /*public string RejectReasonDescription { get; set; }*/

        public RejectReasonAndDescriptionForOffer RejectReasonAndDescription { get; set; }
        public string ReasonRefuse { get; set; }
        public bool IsExtendStorage { get; set; }
        public double? PriceForOnDayStorage { get; set; }

    }
}
