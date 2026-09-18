using AlgoJudge.Application.Common.Exceptions;
using AlgoJudge.Application.AlgorithmProblems.Commands.CreateAlgorithmProblem;
using AlgoJudge.Application.AlgorithmProblems.Commands.UpdateAlgorithmProblem;
using AlgoJudge.Domain.Entities;

namespace AlgoJudge.Application.FunctionalTests.AlgorithmProblems.Commands;

public class UpdateAlgorithmProblemTests : TestBase
{
    [Test]
    public async Task ShouldRequireValidAlgorithmProblemId()
    {
        var command = new UpdateAlgorithmProblemCommand { Id = 99, Title = "New Title" };
        await Should.ThrowAsync<NotFoundException>(() => TestApp.SendAsync(command));
    }

    [Test]
    public async Task ShouldRequireUniqueTitle()
    {
        var listId = await TestApp.SendAsync(new CreateAlgorithmProblemCommand
        {
            Title = "New List"
        });

        await TestApp.SendAsync(new CreateAlgorithmProblemCommand
        {
            Title = "Other List"
        });

        var command = new UpdateAlgorithmProblemCommand
        {
            Id = listId,
            Title = "Other List"
        };

        var ex = await Should.ThrowAsync<ValidationException>(() => TestApp.SendAsync(command));

        ex.Errors.ShouldContainKey("Title");
        ex.Errors["Title"].ShouldContain("'Title' must be unique.");
    }

    [Test]
    public async Task ShouldUpdateAlgorithmProblem()
    {
        var userId = await TestApp.RunAsDefaultUserAsync();

        var listId = await TestApp.SendAsync(new CreateAlgorithmProblemCommand
        {
            Title = "New List"
        });

        var command = new UpdateAlgorithmProblemCommand
        {
            Id = listId,
            Title = "Updated List Title"
        };

        await TestApp.SendAsync(command);

        var list = await TestApp.FindAsync<AlgorithmProblem>(listId);

        list.ShouldNotBeNull();
        list!.Title.ShouldBe(command.Title);
        list.LastModifiedBy.ShouldNotBeNull();
        list.LastModifiedBy.ShouldBe(userId);
        list.LastModified.ShouldBe(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
