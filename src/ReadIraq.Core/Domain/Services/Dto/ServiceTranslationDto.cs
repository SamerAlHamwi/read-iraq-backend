using Abp.AutoMapper;
using ReadIraq.Domain.services;

namespace ReadIraq.Domain.Services.Dto
{
    [AutoMap(typeof(ServiceTranslation))]
    public class ServiceTranslationDto
    {
        public string Name { get; set; }

        public string Language { get; set; }

    }
}
