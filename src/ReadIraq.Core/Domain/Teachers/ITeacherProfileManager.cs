using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Teachers
{
    public interface ITeacherProfileManager : IDomainService
    {
        Task<TeacherProfile> GetByIdAsync(Guid id);
        Task<List<TeacherProfile>> GetAllAsync();
        Task<TeacherProfile> CreateAsync(TeacherProfile teacherProfile);
        Task<TeacherProfile> UpdateAsync(TeacherProfile teacherProfile);
        Task DeleteAsync(Guid id);
        Task UpdateRatingAsync(Guid teacherProfileId);
    }
}
