using Abp.Application.Services;
using ReadIraq.Search.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Search
{
    public interface ISearchAppService : IApplicationService
    {
        Task<SearchOutput> SearchAsync(SearchInput input);
        Task<List<string>> GetSuggestionsAsync(string q);
    }
}
