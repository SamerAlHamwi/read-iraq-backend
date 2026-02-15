using Abp.Application.Services.Dto;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace ReadIraq.Users.Dto
{
    public class UpdateUserDto : CreateUserDto, IEntityDto<long>
    {
        public long Id { get; set; }
        [AllowNull]
        [JsonIgnore]
        internal new string Password { get; set; }
    }
}
