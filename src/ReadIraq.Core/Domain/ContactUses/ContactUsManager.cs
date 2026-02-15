using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Localization.SourceFiles;
using System.Threading.Tasks;

namespace ReadIraq.Domain.ContactUses
{
    public class ContactUsManager : DomainService, IContactUsManager
    {
        private readonly IRepository<ContactUs> _contactUsRepository;
        public ContactUsManager(IRepository<ContactUs> contactUsRepository)
        {
            _contactUsRepository = contactUsRepository;

        }
        public async Task<ContactUs> GetContactUs()
        {
            var contact = await _contactUsRepository.GetAll().Include(x => x.Translations).FirstOrDefaultAsync();
            if (contact is null)
                throw new UserFriendlyException(700, Exceptions.ObjectWasNotFound + "700", Tokens.Contact);
            return contact;
        }
        public async Task<bool> IsAnyCotactUSFound()
        {
            if (await _contactUsRepository.GetAll().CountAsync() > 0)
            {
                return true;
            }
            return false;
        }
    }
}
