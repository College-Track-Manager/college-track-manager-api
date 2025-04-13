using System.ComponentModel.DataAnnotations;

public class StudentRegistration
{
    public int Id { get; set; }

    [Required]
    public string FirstName { get; set; } = "";

    [Required]
    public string LastName { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Phone { get; set; } = "";

    [Required]
    public string Address { get; set; } = "";

    [Required]
    public string Track { get; set; } = "";

    [Required]
    public string Education { get; set; } = "";

    [Required]
    public string Statement { get; set; } = "";

    public string ResumePath { get; set; } = "";
    public string TranscriptPath { get; set; } = "";
    public string IdCardPath { get; set; } = "";
}
