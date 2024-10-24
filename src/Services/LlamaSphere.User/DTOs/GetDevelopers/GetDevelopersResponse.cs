using LlamaSphere.User.Models;

namespace LlamaSphere.User.DTOs.GetDevelopers;

public class GetDevelopersResponse
{
    public IEnumerable<Developer> Developers { get; set; }
}
