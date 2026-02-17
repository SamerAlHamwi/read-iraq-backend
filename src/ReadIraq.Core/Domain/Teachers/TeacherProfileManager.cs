using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using ReadIraq.Localization.SourceFiles;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Teachers
{
    public class TeacherProfileManager : DomainService, ITeacherProfileManager
    {
        private readonly IRepository<TeacherProfile, Guid> _teacherProfileRepository;

        public TeacherProfileManager(IRepository<TeacherProfile, Guid> teacherProfileRepository)
        {
            _teacherProfileRepository = teacherProfileRepository;
        }

        public async Task<TeacherProfile> GetByIdAsync(Guid id)
        {
            var profile = await _teacherProfileRepository.GetAsync(id);
            if (profile == null)
            {
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.TeacherProfile));
            }
            return profile;
        }

        public async Task<List<TeacherProfile>> GetAllAsync()
        {
            return await _teacherProfileRepository.GetAllListAsync();
        }

        public async Task<TeacherProfile> CreateAsync(TeacherProfile teacherProfile)
        {
            return await _teacherProfileRepository.InsertAsync(teacherProfile);
        }

        public async Task<TeacherProfile> UpdateAsync(TeacherProfile teacherProfile)
        {
            return await _teacherProfileRepository.UpdateAsync(teacherProfile);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _teacherProfileRepository.DeleteAsync(id);
        }
    }
}
