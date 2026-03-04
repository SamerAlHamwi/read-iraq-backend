using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using ReadIraq.Localization.SourceFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ReadIraq.Domain.Teachers
{
    public class TeacherProfileManager : DomainService, ITeacherProfileManager
    {
        private readonly IRepository<TeacherProfile, Guid> _teacherProfileRepository;
        private readonly IRepository<TeacherReview, Guid> _teacherReviewRepository;

        public TeacherProfileManager(
            IRepository<TeacherProfile, Guid> teacherProfileRepository,
            IRepository<TeacherReview, Guid> teacherReviewRepository)
        {
            _teacherProfileRepository = teacherProfileRepository;
            _teacherReviewRepository = teacherReviewRepository;
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

        public async Task UpdateRatingAsync(Guid teacherProfileId)
        {
            var reviews = await _teacherReviewRepository.GetAll()
                .Where(x => x.TeacherProfileId == teacherProfileId)
                .ToListAsync();

            var teacher = await _teacherProfileRepository.GetAsync(teacherProfileId);

            if (reviews.Count == 0)
            {
                teacher.AverageRating = 0;
                teacher.ReviewsCount = 0;
            }
            else
            {
                teacher.AverageRating = (decimal)reviews.Average(x => x.Rating);
                teacher.ReviewsCount = reviews.Count;
            }

            await _teacherProfileRepository.UpdateAsync(teacher);
        }
    }
}
