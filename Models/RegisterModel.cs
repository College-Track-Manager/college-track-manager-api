public class RegisterModel
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Password { get; set; }

     // Constructor to initialize properties
    public RegisterModel()
    {
        Username = string.Empty;
        Email = string.Empty;
        FullName = string.Empty;
        Password = string.Empty;
    }
}