using CollegeTrackAPI.Models;

namespace CollegeTrackAPI.DTOs
{
    public class TrackCreateDto
    {
        public string Title { get; set; } = "";
        public string TitleEn { get; set; } = "";
        public TrackTypeEnum TrackType { get; set; }
        public TrackDegreeEnum TrackDegree { get; set; }
        public string ShortDescription { get; set; } = "";
        public string FullDescription { get; set; } = "";
        public string Duration { get; set; } = "";
        public string CareerOutlook { get; set; } = "";
        public string Image { get; set; } = "";
        public List<string> Requirements { get; set; } = new();
        public List<int> CourseIds { get; set; } = new();
    }
}
