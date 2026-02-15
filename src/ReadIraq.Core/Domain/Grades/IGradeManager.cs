using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Grades
{
    public interface IGradeManager : IDomainService
    {
        Task<Grade> GetByIdAsync(int id);
        Task<List<Grade>> GetAllAsync();
        Task<Grade> CreateAsync(Grade grade);
        Task<Grade> UpdateAsync(Grade grade);
        Task DeleteAsync(int id);
    }
}
