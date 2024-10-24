using LlamaSphere.AppUser.Models;

namespace LlamaSphere.AppUser.DTOs.GetDevelopers;

public class GetDevelopersResponse
{
    public IEnumerable<Developer> Developers { get; set; }
}
