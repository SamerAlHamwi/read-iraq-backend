using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.RequestForQuotationContacts.Dto
{
    public class RequestForQuotationContactDto
    {
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string DailCode { get; set; }
        public bool IsWhatsAppAvailable { get; set; }
        public bool IsTelegramAvailable { get; set; }
        public bool IsCallAvailable { get; set; }
        public RequestForQuotationContactType RequestForQuotationContactType { get; set; }

    }
}
