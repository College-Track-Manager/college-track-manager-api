using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeTrackAPI.Models
{
    // Enum for TrackType
    public enum TrackTypeEnum
    {
        Academic = 1,
        Professional = 2
    }


    // Enum for TrackDegree
    public enum TrackDegreeEnum
    {
        Diploma = 1,
        Master = 2,
        PhD = 3
    }

    public enum StudyType
    {
        Offline = 1,
        Online = 2

    }
    public enum StudentRegistrationType
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public class Course
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string CourseCode { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int Credits { get; set; }
        public ICollection<TrackCourse> TrackCourses { get; set; } = new List<TrackCourse>();


    }

    public class Track
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; } = "";
        public string TitleEn { get; set; } = "";

        // Using the enum for TrackType
        public TrackTypeEnum TrackType { get; set; } = TrackTypeEnum.Academic; // Default is Academic

        // Using the enum for TrackDegree
        public TrackDegreeEnum TrackDegree { get; set; } = TrackDegreeEnum.Diploma; // Default is Diploma

        public string ShortDescription { get; set; } = "";
        public string FullDescription { get; set; } = "";
        public string Duration { get; set; } = "";
        public string CareerOutlook { get; set; } = "";
        public string Image { get; set; } = "";

        public List<string> Requirements { get; set; } = new();
        public ICollection<TrackCourse> TrackCourses { get; set; } = new List<TrackCourse>();


        public ICollection<StudentRegistration> Registrations { get; set; } = new List<StudentRegistration>();

    }
}
