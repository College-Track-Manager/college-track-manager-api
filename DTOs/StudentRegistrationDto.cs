namespace CollegeTrackAPI.DTOs
{
    public class StudentRegistrationDto
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Track { get; set; }
        public string RegistrationDate { get; set; }
        public string Education { get; set; }
        public string Statement { get; set; }
        public string ResumePath { get; set; }
        public string TranscriptPath { get; set; }
        public string IdCardPath { get; set; }
        public IFormFile Resume { get; set; }
        public IFormFile Transcript { get; set; }
        public IFormFile IdCard { get; set; }
        public int Status { get; set; }
    }
}
