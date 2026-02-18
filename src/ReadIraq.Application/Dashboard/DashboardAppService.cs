using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.Subjects;
using ReadIraq.Domain.Subscriptions;
using ReadIraq.Domain.Teachers;
using ReadIraq.Dashboard.Dto;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Dashboard
{
    [AbpAuthorize]
    public class DashboardAppService : ApplicationService, IDashboardAppService
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Subscription, Guid> _subscriptionRepository;
        private readonly IRepository<Subject, Guid> _subjectRepository;
        private readonly IRepository<TeacherProfile, Guid> _teacherRepository;

        public DashboardAppService(
            IRepository<User, long> userRepository,
            IRepository<Subscription, Guid> subscriptionRepository,
            IRepository<Subject, Guid> subjectRepository,
            IRepository<TeacherProfile, Guid> teacherRepository)
        {
            _userRepository = userRepository;
            _subscriptionRepository = subscriptionRepository;
            _subjectRepository = subjectRepository;
            _teacherRepository = teacherRepository;
        }

        public async Task<DashboardSummaryOutput> GetSummaryAsync()
        {
            var today = DateTime.Today;

            return new DashboardSummaryOutput
            {
                TotalUsers = await _userRepository.CountAsync(),
                NewUsersToday = await _userRepository.GetAll().Where(x => x.CreationTime >= today).CountAsync(),
                ActiveSubscriptions = await _subscriptionRepository.GetAll().Where(x => x.IsActive && x.ExpiresAt >= DateTime.UtcNow).CountAsync(),
                TotalSubjects = await _subjectRepository.CountAsync(),
                TotalTeachers = await _teacherRepository.CountAsync()
            };
        }
    }
}
