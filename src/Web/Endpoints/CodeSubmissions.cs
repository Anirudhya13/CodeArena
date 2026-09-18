using CodeArena.Application.CodeSubmissions.Commands.DeleteCodeSubmission;
using Microsoft.AspNetCore.Http.HttpResults;
using MediatR;

namespace CodeArena.Web.Endpoints;

public class CodeSubmissions : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();
        groupBuilder.MapDelete(DeleteCodeSubmission, "{id}");
        groupBuilder.MapPost(EvaluateSubmission, "Evaluate");
        groupBuilder.MapGet(GetUserSubmissions, "MySubmissions");
    }

    [EndpointSummary("Delete a Code Submission")]
    public static async Task<NoContent> DeleteCodeSubmission(ISender sender, int id)
    {
        await sender.Send(new DeleteCodeSubmissionCommand(id));
        return TypedResults.NoContent();
    }

    [EndpointSummary("Evaluate Submission with AI Judge")]
    public static async Task<string> EvaluateSubmission(ISender sender, [Microsoft.AspNetCore.Mvc.FromBody] CodeArena.Application.CodeSubmissions.Commands.EvaluateSubmission.EvaluateSubmissionCommand command)
    {
        return await sender.Send(command);
    }

    [EndpointSummary("Get current user submissions")]
    public static async Task<List<CodeArena.Application.CodeSubmissions.Queries.GetUserSubmissions.SubmissionDto>> GetUserSubmissions(ISender sender)
    {
        return await sender.Send(new CodeArena.Application.CodeSubmissions.Queries.GetUserSubmissions.GetUserSubmissionsQuery());
    }
}

