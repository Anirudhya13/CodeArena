using CodeArena.Application.CodeSubmissions.Commands.CreateCodeSubmission;
using CodeArena.Application.CodeSubmissions.Commands.DeleteCodeSubmission;
using CodeArena.Application.AlgorithmProblems.Commands.CreateAlgorithmProblem;
using CodeArena.Domain.Entities;

namespace CodeArena.Application.FunctionalTests.CodeSubmissions.Commands;

public class DeleteCodeSubmissionTests : TestBase
{
    [Test]
    public async Task ShouldRequireValidCodeSubmissionId()
    {
        var command = new DeleteCodeSubmissionCommand(99);

        await Should.ThrowAsync<NotFoundException>(() => TestApp.SendAsync(command));
    }

    [Test]
    public async Task ShouldDeleteCodeSubmission()
    {
        var listId = await TestApp.SendAsync(new CreateAlgorithmProblemCommand
        {
            Title = "New List"
        });

        var itemId = await TestApp.SendAsync(new CreateCodeSubmissionCommand
        {
            ListId = listId,
            Title = "New Item"
        });

        await TestApp.SendAsync(new DeleteCodeSubmissionCommand(itemId));

        var item = await TestApp.FindAsync<CodeSubmission>(itemId);

        item.ShouldBeNull();
    }
}

