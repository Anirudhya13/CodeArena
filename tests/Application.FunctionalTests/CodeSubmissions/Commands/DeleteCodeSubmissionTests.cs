using AlgoJudge.Application.CodeSubmissions.Commands.CreateCodeSubmission;
using AlgoJudge.Application.CodeSubmissions.Commands.DeleteCodeSubmission;
using AlgoJudge.Application.AlgorithmProblems.Commands.CreateAlgorithmProblem;
using AlgoJudge.Domain.Entities;

namespace AlgoJudge.Application.FunctionalTests.CodeSubmissions.Commands;

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
