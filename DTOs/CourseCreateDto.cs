namespace CollegeTrackAPI.DTOs
{
    public class CourseCreateDto
    {
        public string CourseCode { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int Credits { get; set; }
    }
}
