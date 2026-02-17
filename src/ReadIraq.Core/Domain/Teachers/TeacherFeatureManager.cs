using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using ReadIraq.Localization.SourceFiles;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Teachers
{
    public class TeacherFeatureManager : DomainService, ITeacherFeatureManager
    {
        private readonly IRepository<TeacherFeature, Guid> _teacherFeatureRepository;

        public TeacherFeatureManager(IRepository<TeacherFeature, Guid> teacherFeatureRepository)
        {
            _teacherFeatureRepository = teacherFeatureRepository;
        }

        public async Task<TeacherFeature> GetByIdAsync(Guid id)
        {
            var feature = await _teacherFeatureRepository.GetAsync(id);
            if (feature == null)
            {
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.TeacherFeature));
            }
            return feature;
        }

        public async Task<List<TeacherFeature>> GetAllAsync()
        {
            return await _teacherFeatureRepository.GetAllListAsync();
        }

        public async Task<TeacherFeature> CreateAsync(TeacherFeature teacherFeature)
        {
            return await _teacherFeatureRepository.InsertAsync(teacherFeature);
        }

        public async Task<TeacherFeature> UpdateAsync(TeacherFeature teacherFeature)
        {
            return await _teacherFeatureRepository.UpdateAsync(teacherFeature);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _teacherFeatureRepository.DeleteAsync(id);
        }
    }
}
