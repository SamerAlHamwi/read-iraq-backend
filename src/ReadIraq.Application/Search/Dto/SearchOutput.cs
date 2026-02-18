using System.Collections.Generic;

namespace ReadIraq.Search.Dto
{
    public class SearchOutput
    {
        public List<SearchResultItem> Items { get; set; }
        public int TotalCount { get; set; }
    }

    public class SearchResultItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string ImageUrl { get; set; }
    }
}
