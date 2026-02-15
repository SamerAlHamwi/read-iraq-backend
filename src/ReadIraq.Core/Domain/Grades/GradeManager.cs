using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using ReadIraq.Localization.SourceFiles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Grades
{
    public class GradeManager : DomainService, IGradeManager
    {
        private readonly IRepository<Grade> _gradeRepository;

        public GradeManager(IRepository<Grade> gradeRepository)
        {
            _gradeRepository = gradeRepository;
        }

        public async Task<Grade> GetByIdAsync(int id)
        {
            var grade = await _gradeRepository.GetAsync(id);
            if (grade == null)
            {
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Grade));
            }
            return grade;
        }

        public async Task<List<Grade>> GetAllAsync()
        {
            return await _gradeRepository.GetAllListAsync();
        }

        public async Task<Grade> CreateAsync(Grade grade)
        {
            return await _gradeRepository.InsertAsync(grade);
        }

        public async Task<Grade> UpdateAsync(Grade grade)
        {
            return await _gradeRepository.UpdateAsync(grade);
        }

        public async Task DeleteAsync(int id)
        {
            await _gradeRepository.DeleteAsync(id);
        }
    }
}
