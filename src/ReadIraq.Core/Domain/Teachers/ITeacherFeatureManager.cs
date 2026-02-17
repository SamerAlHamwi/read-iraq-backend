using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Teachers
{
    public interface ITeacherFeatureManager : IDomainService
    {
        Task<TeacherFeature> GetByIdAsync(Guid id);
        Task<List<TeacherFeature>> GetAllAsync();
        Task<TeacherFeature> CreateAsync(TeacherFeature teacherFeature);
        Task<TeacherFeature> UpdateAsync(TeacherFeature teacherFeature);
        Task DeleteAsync(Guid id);
    }
}
