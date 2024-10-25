namespace LlamaSphere.Project.Features.Projects;

public record GetProjectByIdQuery(Guid Id);

public record GetProjectByIdResult(Models.Project Project);

public class GetProjectByIdQueryHandler
{
}
