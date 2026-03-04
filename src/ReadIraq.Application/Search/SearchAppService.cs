using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.LessonSessions;
using ReadIraq.Domain.Subjects;
using ReadIraq.Domain.Teachers;
using ReadIraq.Search.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadIraq.Domain.Attachments;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Search
{
    public class SearchAppService : ApplicationService, ISearchAppService
    {
        private readonly IRepository<Subject, Guid> _subjectRepository;
        private readonly IRepository<LessonSession, Guid> _sessionRepository;
        private readonly IRepository<TeacherProfile, Guid> _teacherRepository;
        private readonly IRepository<TeacherSubject, Guid> _teacherSubjectRepository;
        private readonly IAttachmentManager _attachmentManager;

        public SearchAppService(
            IRepository<Subject, Guid> subjectRepository,
            IRepository<LessonSession, Guid> sessionRepository,
            IRepository<TeacherProfile, Guid> teacherRepository,
            IRepository<TeacherSubject, Guid> teacherSubjectRepository,
            IAttachmentManager attachmentManager)
        {
            _subjectRepository = subjectRepository;
            _sessionRepository = sessionRepository;
            _teacherRepository = teacherRepository;
            _teacherSubjectRepository = teacherSubjectRepository;
            _attachmentManager = attachmentManager;
        }

        public async Task<SearchOutput> SearchAsync(SearchInput input)
        {
            var results = new List<SearchResultItem>();
            int totalCount = 0;

            if (string.IsNullOrEmpty(input.Type) || input.Type == "subject")
            {
                var query = _subjectRepository.GetAll().Include(x => x.Name)
                    .WhereIf(!string.IsNullOrEmpty(input.Q), x => (x.Description != null && x.Description.Contains(input.Q)) || x.Name.Any(n => n.Name.Contains(input.Q)));

                totalCount += await query.CountAsync();
                var items = await query.OrderBy(x => x.CreationTime).Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
                foreach (var item in items)
                {
                    var lessonsCount = await _sessionRepository.CountAsync(x => x.SubjectId == item.Id);
                    var teacherName = await _teacherSubjectRepository.GetAll()
                        .Include(x => x.TeacherProfile)
                        .Where(x => x.SubjectId == item.Id)
                        .Select(x => x.TeacherProfile.Name)
                        .FirstOrDefaultAsync();

                    var searchResult = new SearchResultItem
                    {
                        Id = item.Id.ToString(),
                        Title = item.Name.FirstOrDefault()?.Name ?? item.Description ?? "Subject",
                        Description = item.Description,
                        Type = "subject",
                        LessonsCount = lessonsCount,
                        Color = item.Color,
                        TeacherName = teacherName
                    };
                    var attachment = await _attachmentManager.GetElementByRefAsync(item.Id.ToString(), AttachmentRefType.Subject);
                    if (attachment != null) searchResult.ImageUrl = _attachmentManager.GetUrl(attachment);
                    results.Add(searchResult);
                }
            }

            if (string.IsNullOrEmpty(input.Type) || input.Type == "session")
            {
                var query = _sessionRepository.GetAll().Include(x => x.Subject).ThenInclude(x => x.Name).Include(x => x.TeacherProfile)
                    .WhereIf(!string.IsNullOrEmpty(input.Q), x => x.Title.Contains(input.Q) || (x.Description != null && x.Description.Contains(input.Q)));

                totalCount += await query.CountAsync();
                var items = await query.OrderBy(x => x.CreationTime).Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
                foreach (var item in items)
                {
                    var searchResult = new SearchResultItem
                    {
                        Id = item.Id.ToString(),
                        Title = item.Title,
                        Description = item.Description,
                        Subtitle = item.Subject?.Description, // Or Level
                        Type = "session",
                        Duration = TimeSpan.FromSeconds(item.DurationSeconds).ToString(@"mm\:ss"),
                        SubjectName = item.Subject?.Name?.FirstOrDefault()?.Name ?? item.Subject?.Description,
                        TeacherName = item.TeacherProfile?.Name
                    };
                    var attachment = await _attachmentManager.GetElementByRefAsync(item.Id.ToString(), AttachmentRefType.LessonSessionThumbnail);
                    if (attachment != null) searchResult.ImageUrl = _attachmentManager.GetUrl(attachment);
                    results.Add(searchResult);
                }
            }

            if (string.IsNullOrEmpty(input.Type) || input.Type == "teacher")
            {
                var query = _teacherRepository.GetAll()
                    .WhereIf(!string.IsNullOrEmpty(input.Q), x => x.Name.Contains(input.Q));

                totalCount += await query.CountAsync();
                var items = await query.OrderBy(x => x.CreationTime).Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
                foreach (var item in items)
                {
                    var lessonsCount = await _sessionRepository.CountAsync(x => x.TeacherProfileId == item.Id);
                    var searchResult = new SearchResultItem
                    {
                        Id = item.Id.ToString(),
                        Title = item.Name,
                        Description = item.Bio,
                        Subtitle = item.Specialization,
                        Type = "teacher",
                        Rating = (double)item.AverageRating,
                        LessonsCount = lessonsCount
                    };
                    var attachment = await _attachmentManager.GetElementByRefAsync(item.Id.ToString(), AttachmentRefType.TeacherProfile);
                    if (attachment != null) searchResult.ImageUrl = _attachmentManager.GetUrl(attachment);
                    results.Add(searchResult);
                }
            }

            return new SearchOutput
            {
                Items = results.Take(input.MaxResultCount).ToList(),
                TotalCount = totalCount
            };
        }

        public async Task<List<string>> GetSuggestionsAsync(string q)
        {
            if (string.IsNullOrEmpty(q)) return new List<string>();

            var suggestions = new List<string>();

            suggestions.AddRange(await _sessionRepository.GetAll()
                .Where(x => x.Title.Contains(q))
                .Select(x => x.Title)
                .Take(5)
                .ToListAsync());

            suggestions.AddRange(await _teacherRepository.GetAll()
                .Where(x => x.Name.Contains(q))
                .Select(x => x.Name)
                .Take(5)
                .ToListAsync());

            return suggestions.Distinct().ToList();
        }
    }
}
