namespace CollegeTrackAPI.DTOs
{
    public class ApplicationDataDto
    {
      
        public string Name { get; set; }

        public string Email { get; set; }

       
        public string TrackType { get; set; } // "academic" or "professional"

        public string Track { get; set; }

       
        public string EducationLevel { get; set; }

       
        public string StudyType { get; set; } // "online" or "offline"

       
        public string Education { get; set; }

     
        public string? Statement { get; set; }

       
        public string? ResumeUrl { get; set; } // URL to download resume

    
        public string? TranscriptUrl { get; set; } // URL to download transcript

     
        public string? IdCardUrl { get; set; } // URL to download ID card

    
        public string SubmissionDate { get; set; }

   
        public string? AdminComments { get; set; } // Comments from the admin
    }
}
