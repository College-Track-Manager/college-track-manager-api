using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeTrackAPI.Models
{

    public class Course
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string CourseCode { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int Credits { get; set; }
    }

    public class Track
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; } = "";
        public string ShortDescription { get; set; } = "";
        public string FullDescription { get; set; } = "";
        public string Duration { get; set; } = "";
        public string CareerOutlook { get; set; } = "";
        public string Image { get; set; } = "";

        public List<string> Requirements { get; set; } = new();
        public List<Course> Courses { get; set; } = new();
    }
}