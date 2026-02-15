using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Localization.SourceFiles;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Domain.RejectReasons
{
    public class RejectReasonManager : DomainService, IRejectReasonManager
    {
        private readonly IRepository<RejectReason> _rejectReasonRepository;

        public RejectReasonManager(IRepository<RejectReason> repository)
        {
            _rejectReasonRepository = repository;
        }

        public async Task<RejectReason> GetEntityByIdAsync(int Id)
        {
            var entity = await _rejectReasonRepository.GetAllIncluding(x => x.Translations).Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (entity == null) throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.RejectReason);
            return entity;
        }

        public async Task CheckIfRejectReasonIsExist(int Id)
        {

            if (!await _rejectReasonRepository.GetAll().AnyAsync(x => x.Id == Id))
                throw new UserFriendlyException(Exceptions.ObjectWasNotFound, Tokens.RejectReason);
        }

        public async Task<Enums.Enum.PossibilityPotentialClient> GetRejectResonTypeByIdAsync(int Id)
        {
            return (await _rejectReasonRepository.GetAsync(Id)).PossibilityPotentialClient;
        }
    }
}
