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
        public string Subtitle { get; set; }
        public string Type { get; set; }
        public string ImageUrl { get; set; }
        public double? Rating { get; set; }
        public int? LessonsCount { get; set; }
        public string Duration { get; set; }
        public double? Progress { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
    }
}
