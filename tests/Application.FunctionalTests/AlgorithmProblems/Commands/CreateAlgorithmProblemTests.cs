using CodeArena.Application.Common.Exceptions;
using CodeArena.Application.AlgorithmProblems.Commands.CreateAlgorithmProblem;
using CodeArena.Domain.Entities;

namespace CodeArena.Application.FunctionalTests.AlgorithmProblems.Commands;

public class CreateAlgorithmProblemTests : TestBase
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateAlgorithmProblemCommand();
        await Should.ThrowAsync<ValidationException>(() => TestApp.SendAsync(command));
    }

    [Test]
    public async Task ShouldRequireUniqueTitle()
    {
        await TestApp.SendAsync(new CreateAlgorithmProblemCommand
        {
            Title = "Shopping"
        });

        var command = new CreateAlgorithmProblemCommand
        {
            Title = "Shopping"
        };

        await Should.ThrowAsync<ValidationException>(() => TestApp.SendAsync(command));
    }

    [Test]
    public async Task ShouldCreateAlgorithmProblem()
    {
        var userId = await TestApp.RunAsDefaultUserAsync();

        var command = new CreateAlgorithmProblemCommand
        {
            Title = "Tasks"
        };

        var id = await TestApp.SendAsync(command);

        var list = await TestApp.FindAsync<AlgorithmProblem>(id);

        list.ShouldNotBeNull();
        list!.Title.ShouldBe(command.Title);
        list.CreatedBy.ShouldBe(userId);
        list.Created.ShouldBe(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}

