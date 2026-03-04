using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using ReadIraq.ActivationCodes.Dto;
using ReadIraq.Domain.Codes;
using ReadIraq.Domain.Enrollments;
using ReadIraq.Domain.Grades;
using ReadIraq.Domain.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ReadIraq.ActivationCodes
{
    [AbpAuthorize]
    public class ActivationCodeAppService : ReadIraqAppServiceBase, IActivationCodeAppService
    {
        private readonly IRepository<ActivationCode, Guid> _codeRepository;
        private readonly IRepository<Enrollment, Guid> _enrollmentRepository;
        private readonly IRepository<GradeSubject> _gradeSubjectRepository;

        public ActivationCodeAppService(
            IRepository<ActivationCode, Guid> codeRepository,
            IRepository<Enrollment, Guid> enrollmentRepository,
            IRepository<GradeSubject> gradeSubjectRepository)
        {
            _codeRepository = codeRepository;
            _enrollmentRepository = enrollmentRepository;
            _gradeSubjectRepository = gradeSubjectRepository;
        }

        [AbpAuthorize]
        public async Task GenerateCodes(CreateActivationCodeInput input)
        {
            for (int i = 0; i < input.Count; i++)
            {
                var code = new ActivationCode
                {
                    Code = GenerateRandomCode(),
                    SubjectId = input.SubjectId,
                    TeacherId = input.TeacherId,
                    GradeId = input.GradeId,
                    Price = input.Price
                };

                await _codeRepository.InsertAsync(code);
            }
        }

        private string GenerateRandomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<ActivationCodeDto> GetCode(Guid id)
        {
            var code = await _codeRepository.GetAll()
                .Include(x => x.Subject).ThenInclude(s => s.Name)
                .Include(x => x.Teacher)
                .Include(x => x.Grade).ThenInclude(g => g.Name)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (code == null) throw new Abp.UI.UserFriendlyException("Code not found");

            var dto = ObjectMapper.Map<ActivationCodeDto>(code);

            dto.SubjectName = code.Subject?.Name?.FirstOrDefault()?.Name;
            dto.TeacherName = code.Teacher?.Name;
            dto.GradeName = code.Grade?.Name?.FirstOrDefault()?.Name;
            dto.UserName = code.User?.UserName;

            return dto;
        }

        public async Task<PagedResultDto<ActivationCodeDto>> GetAll(GetActivationCodesInput input)
        {
            var query = _codeRepository.GetAll()
                .Include(x => x.Subject).ThenInclude(s => s.Name)
                .Include(x => x.Teacher)
                .Include(x => x.Grade).ThenInclude(g => g.Name)
                .Include(x => x.User)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), x => x.Code.Contains(input.Filter))
                .WhereIf(input.IsUsed.HasValue, x => x.UserId.HasValue == input.IsUsed.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(input.Sorting ?? "CreationTime DESC")
                .PageBy(input)
                .ToListAsync();

            var dtos = items.Select(code => {
                var d = ObjectMapper.Map<ActivationCodeDto>(code);
                d.SubjectName = code.Subject?.Name?.FirstOrDefault()?.Name;
                d.TeacherName = code.Teacher?.Name;
                d.GradeName = code.Grade?.Name?.FirstOrDefault()?.Name;
                d.UserName = code.User?.UserName;
                return d;
            }).ToList();

            return new PagedResultDto<ActivationCodeDto>(
                totalCount,
                dtos
            );
        }

        public async Task<ActivationCodeStatisticsDto> GetStatistics()
        {
            var totalCodes = await _codeRepository.CountAsync();
            var usedCodes = await _codeRepository.CountAsync(x => x.UserId.HasValue);
            var revenue = await _codeRepository.GetAll()
                .Where(x => x.UserId.HasValue)
                .SumAsync(x => x.Price);

            return new ActivationCodeStatisticsDto
            {
                TotalCodes = totalCodes,
                UsedCodes = usedCodes,
                UnusedCodes = totalCodes - usedCodes,
                TotalRevenue = revenue
            };
        }

        public async Task UseCode(UseCodeInput input)
        {
            var userId = AbpSession.GetUserId();

            // Handle Master Code
            if (input.Code == "RS2931")
            {
                if (!input.SubjectId.HasValue) throw new Abp.UI.UserFriendlyException("Subject ID is required for this code.");

                var gradeSubject = await _gradeSubjectRepository.FirstOrDefaultAsync(x => x.SubjectId == input.SubjectId.Value);
                int? gradeId = gradeSubject?.GradeId;

                var existingEnrollment = await _enrollmentRepository.FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.SubjectId == input.SubjectId.Value);

                if (existingEnrollment == null)
                {
                    await _enrollmentRepository.InsertAsync(new Enrollment
                    {
                        UserId = userId,
                        SubjectId = input.SubjectId.Value,
                        GradeId = gradeId,
                        StartedAt = DateTime.Now,
                        ProgressPercent = 0
                    });
                }
                return;
            }

            var activationCode = await _codeRepository.FirstOrDefaultAsync(x => x.Code == input.Code);

            if (activationCode == null) throw new Abp.UI.UserFriendlyException("Invalid code");

            if (activationCode.UserId.HasValue && activationCode.UserId != userId)
            {
                throw new Abp.UI.UserFriendlyException("This code has already been used by another user.");
            }

            if (!activationCode.UserId.HasValue)
            {
                activationCode.UserId = userId;
                activationCode.UsedDate = DateTime.Now;
                await _codeRepository.UpdateAsync(activationCode);

                // Create Enrollment
                var targetSubjectId = activationCode.SubjectId ?? input.SubjectId;
                if (targetSubjectId.HasValue)
                {
                    var gradeId = activationCode.GradeId;
                    if (!gradeId.HasValue) {
                         var gradeSubject = await _gradeSubjectRepository.FirstOrDefaultAsync(x => x.SubjectId == targetSubjectId.Value);
                         gradeId = gradeSubject?.GradeId;
                    }

                    var existingEnrollment = await _enrollmentRepository.FirstOrDefaultAsync(x =>
                        x.UserId == userId &&
                        x.SubjectId == targetSubjectId.Value &&
                        x.TeacherId == activationCode.TeacherId &&
                        x.GradeId == gradeId);

                    if (existingEnrollment == null)
                    {
                        var enrollment = new Enrollment
                        {
                            UserId = userId,
                            SubjectId = targetSubjectId.Value,
                            TeacherId = activationCode.TeacherId,
                            GradeId = gradeId,
                            StartedAt = DateTime.Now,
                            ProgressPercent = 0
                        };
                        await _enrollmentRepository.InsertAsync(enrollment);
                    }
                }
            }
        }
    }
}
