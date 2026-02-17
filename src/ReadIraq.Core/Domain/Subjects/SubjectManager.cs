using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using ReadIraq.Localization.SourceFiles;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Subjects
{
    public class SubjectManager : DomainService, ISubjectManager
    {
        private readonly IRepository<Subject, Guid> _subjectRepository;

        public SubjectManager(IRepository<Subject, Guid> subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<Subject> GetByIdAsync(Guid id)
        {
            var subject = await _subjectRepository.GetAsync(id);
            if (subject == null)
            {
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Subject));
            }
            return subject;
        }

        public async Task<List<Subject>> GetAllAsync()
        {
            return await _subjectRepository.GetAllListAsync();
        }

        public async Task<Subject> CreateAsync(Subject subject)
        {
            return await _subjectRepository.InsertAsync(subject);
        }

        public async Task<Subject> UpdateAsync(Subject subject)
        {
            return await _subjectRepository.UpdateAsync(subject);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _subjectRepository.DeleteAsync(id);
        }
    }
}
