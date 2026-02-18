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

namespace ReadIraq.Search
{
    public class SearchAppService : ApplicationService, ISearchAppService
    {
        private readonly IRepository<Subject, Guid> _subjectRepository;
        private readonly IRepository<LessonSession, Guid> _sessionRepository;
        private readonly IRepository<TeacherProfile, Guid> _teacherRepository;

        public SearchAppService(
            IRepository<Subject, Guid> subjectRepository,
            IRepository<LessonSession, Guid> sessionRepository,
            IRepository<TeacherProfile, Guid> teacherRepository)
        {
            _subjectRepository = subjectRepository;
            _sessionRepository = sessionRepository;
            _teacherRepository = teacherRepository;
        }

        public async Task<SearchOutput> SearchAsync(SearchInput input)
        {
            var results = new List<SearchResultItem>();
            int totalCount = 0;

            if (string.IsNullOrEmpty(input.Type) || input.Type == "subject")
            {
                var query = _subjectRepository.GetAll()
                    .WhereIf(!string.IsNullOrEmpty(input.Q), x => x.Description.Contains(input.Q)); // simplified search

                totalCount += await query.CountAsync();
                var items = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
                results.AddRange(items.Select(x => new SearchResultItem { Id = x.Id.ToString(), Title = "Subject", Description = x.Description, Type = "subject", ImageUrl = x.ImageUrl }));
            }

            if (string.IsNullOrEmpty(input.Type) || input.Type == "session")
            {
                var query = _sessionRepository.GetAll()
                    .WhereIf(!string.IsNullOrEmpty(input.Q), x => x.Title.Contains(input.Q) || x.Description.Contains(input.Q));

                totalCount += await query.CountAsync();
                var items = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
                results.AddRange(items.Select(x => new SearchResultItem { Id = x.Id.ToString(), Title = x.Title, Description = x.Description, Type = "session", ImageUrl = x.ThumbnailUrl }));
            }

            if (string.IsNullOrEmpty(input.Type) || input.Type == "teacher")
            {
                var query = _teacherRepository.GetAll()
                    .WhereIf(!string.IsNullOrEmpty(input.Q), x => x.Name.Contains(input.Q));

                totalCount += await query.CountAsync();
                var items = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
                results.AddRange(items.Select(x => new SearchResultItem { Id = x.Id.ToString(), Title = x.Name, Description = x.Bio, Type = "teacher", ImageUrl = x.AvatarUrl }));
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
