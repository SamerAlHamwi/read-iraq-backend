using System.Threading.Tasks;

namespace ReadIraq.Domain.ContactUses
{
    public interface IContactUsManager
    {
        Task<bool> IsAnyCotactUSFound();
        Task<ContactUs> GetContactUs();

    }
}