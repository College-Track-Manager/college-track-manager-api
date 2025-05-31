using CollegeTrackAPI.Models;
using System.ComponentModel.DataAnnotations;

public class StudentRegistration
{
    public int Id { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    

    [Required]
    public string? AcademicYear { get; set; } = "";

    [Required]
    public TrackTypeEnum TrackType { get; set; } = TrackTypeEnum.Academic; // Default is Academic

    // Using the enum for TrackDegree
    [Required]
    public TrackDegreeEnum? TrackDegree { get; set; } = TrackDegreeEnum.Diploma; // Default is Diploma

    [Required]
    public StudyType? StudyType { get; set; }

    [Required]
    public int TrackId { get; set; }

    public Track? Track { get; set; } // navigation property

    [Required]
    public string? Education { get; set; } = "";

    [Required]
    public string? Statement { get; set; } = "";

    public string? ResumePath { get; set; } = "";
    public string? TranscriptPath { get; set; } = "";
    public string? IdCardPath { get; set; } = "";

    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

}
