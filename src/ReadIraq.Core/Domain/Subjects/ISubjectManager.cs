using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Subjects
{
    public interface ISubjectManager : IDomainService
    {
        Task<Subject> GetByIdAsync(Guid id);
        Task<List<Subject>> GetAllAsync();
        Task<Subject> CreateAsync(Subject subject);
        Task<Subject> UpdateAsync(Subject subject);
        Task DeleteAsync(Guid id);
    }
}
