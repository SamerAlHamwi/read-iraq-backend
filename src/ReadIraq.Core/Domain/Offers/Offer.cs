using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.RejectReasons;
using ReadIraq.Domain.SelectedCompaniesByUsers;
using ReadIraq.Domain.ServiceValueForOffers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Offers
{
    public class Offer : FullAuditedEntity<Guid>
    {

        public Guid SelectedCompaniesId { get; set; }
        [ForeignKey(nameof(SelectedCompaniesId))]
        public virtual SelectedCompaniesBySystemForRequest SelectedCompanies { get; set; }
        public virtual ICollection<ServiceValueForOffer> ServiceValueForOffers { get; set; } = new List<ServiceValueForOffer>();
        public double Price { get; set; }
        public OfferStatues Statues { get; set; }
        public OfferProvider Provider { get; set; }
        public string Note { get; set; }

        public int? RejectReasonId { get; set; }
        [ForeignKey(nameof(RejectReasonId))]
        public virtual RejectReason RejectReason { get; set; }

        public string RejectReasonDescription { get; set; }
        public string ReasonRefuse { get; set; }
        public bool IsExtendStorage { get; set; }
        public double? PriceForOnDayStorage { get; set; }

    }
}
