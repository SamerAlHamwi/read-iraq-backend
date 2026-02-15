using AutoMapper;
using ReadIraq.Domain.PushNotifications;
using ReadIraq.PushNotifications.Dto;

namespace ReadIraq.PushNotifications.Mapper
{
    public class PushNotificationMapProfile : Profile
    {
        public PushNotificationMapProfile()
        {

            CreateMap<CreatePushNotificationDto, PushNotificationDto>();
            CreateMap<PushNotificationDto, PushNotification>();
            CreateMap<PushNotification, PushNotificationDto>();
            CreateMap<PushNotification, LitePushNotificationDto>();
            CreateMap<PushNotification, PushNotificationDetailsDto>();
            CreateMap<PushNotification, UpdatePushNotificationDto>();
            CreateMap<CreatePushNotificationDto, PushNotification>();
            CreateMap<PushNotification, CreatePushNotificationDto>();
        }
    }
}
