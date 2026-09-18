using CodeArena.Application.AlgorithmProblems.Commands.CreateAlgorithmProblem;
using CodeArena.Application.AlgorithmProblems.Commands.DeleteAlgorithmProblem;
using CodeArena.Domain.Entities;

namespace CodeArena.Application.FunctionalTests.AlgorithmProblems.Commands;

public class DeleteAlgorithmProblemTests : TestBase
{
    [Test]
    public async Task ShouldRequireValidAlgorithmProblemId()
    {
        var command = new DeleteAlgorithmProblemCommand(99);
        await Should.ThrowAsync<NotFoundException>(() => TestApp.SendAsync(command));
    }

    [Test]
    public async Task ShouldDeleteAlgorithmProblem()
    {
        var listId = await TestApp.SendAsync(new CreateAlgorithmProblemCommand
        {
            Title = "New List"
        });

        await TestApp.SendAsync(new DeleteAlgorithmProblemCommand(listId));

        var list = await TestApp.FindAsync<AlgorithmProblem>(listId);

        list.ShouldBeNull();
    }
}

