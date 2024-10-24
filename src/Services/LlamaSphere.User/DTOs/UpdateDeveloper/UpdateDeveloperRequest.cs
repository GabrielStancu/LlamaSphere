namespace LlamaSphere.User.DTOs.UpdateDeveloper;

public class UpdateDeveloperRequest
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}