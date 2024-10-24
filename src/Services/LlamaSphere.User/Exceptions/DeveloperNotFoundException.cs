using Core.Exceptions;

namespace LlamaSphere.User.Exceptions;

public class DeveloperNotFoundException : NotFoundException
{
    public DeveloperNotFoundException(Guid id) : base("Developer", id)
    {
    }
}
