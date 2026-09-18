using CodeArena.Application.AlgorithmProblems.Commands.CreateAlgorithmProblem;
using CodeArena.Application.AlgorithmProblems.Commands.DeleteAlgorithmProblem;
using CodeArena.Application.AlgorithmProblems.Commands.UpdateAlgorithmProblem;
using CodeArena.Application.AlgorithmProblems.Queries.GetProblems;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CodeArena.Web.Endpoints;

public class AlgorithmProblems : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();

        groupBuilder.MapGet(GetAlgorithmProblems);
        groupBuilder.MapPost(CreateAlgorithmProblem);
        groupBuilder.MapPut(UpdateAlgorithmProblem, "{id}");
        groupBuilder.MapDelete(DeleteAlgorithmProblem, "{id}");
    }

    [EndpointSummary("Get all Problem Lists")]
    [EndpointDescription("Retrieves all Problem lists along with their items.")]
    public static async Task<Ok<ProblemsVm>> GetAlgorithmProblems(ISender sender)
    {
        var vm = await sender.Send(new GetProblemsQuery());

        return TypedResults.Ok(vm);
    }

    [EndpointSummary("Create a new Problem List")]
    [EndpointDescription("Creates a new Problem list using the provided details and returns the ID of the created list.")]
    public static async Task<Created<int>> CreateAlgorithmProblem(ISender sender, CreateAlgorithmProblemCommand command)
    {
        var id = await sender.Send(command);

        return TypedResults.Created($"/{nameof(AlgorithmProblems)}/{id}", id);
    }

    [EndpointSummary("Update a Problem List")]
    [EndpointDescription("Updates the specified Problem list. The ID in the URL must match the ID in the payload.")]
    public static async Task<Results<NoContent, BadRequest>> UpdateAlgorithmProblem(ISender sender, int id, UpdateAlgorithmProblemCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();

        await sender.Send(command);

        return TypedResults.NoContent();
    }

    [EndpointSummary("Delete a Problem List")]
    [EndpointDescription("Deletes the Problem list with the specified ID.")]
    public static async Task<NoContent> DeleteAlgorithmProblem(ISender sender, int id)
    {
        await sender.Send(new DeleteAlgorithmProblemCommand(id));

        return TypedResults.NoContent();
    }
}

