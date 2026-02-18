using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ReadIraq.Comments.Dto;
using System;
using System.Threading.Tasks;

namespace ReadIraq.Comments
{
    public interface ISessionCommentAppService : IAsyncCrudAppService<SessionCommentDto, Guid, PagedSessionCommentResultRequestDto, CreateSessionCommentDto, UpdateSessionCommentDto>
    {
        Task<SessionCommentDto> ReplyAsync(Guid id, CreateSessionCommentDto input);
    }
}
