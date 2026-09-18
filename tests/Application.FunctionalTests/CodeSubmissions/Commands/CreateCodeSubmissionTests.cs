using CodeArena.Application.Common.Exceptions;
using CodeArena.Application.CodeSubmissions.Commands.CreateCodeSubmission;
using CodeArena.Application.AlgorithmProblems.Commands.CreateAlgorithmProblem;
using CodeArena.Domain.Entities;

namespace CodeArena.Application.FunctionalTests.CodeSubmissions.Commands;

public class CreateCodeSubmissionTests : TestBase
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateCodeSubmissionCommand();

        await Should.ThrowAsync<ValidationException>(() => TestApp.SendAsync(command));
    }

    [Test]
    public async Task ShouldCreateCodeSubmission()
    {
        var userId = await TestApp.RunAsDefaultUserAsync();

        var listId = await TestApp.SendAsync(new CreateAlgorithmProblemCommand
        {
            Title = "New List"
        });

        var command = new CreateCodeSubmissionCommand
        {
            ListId = listId,
            Title = "Tasks"
        };

        var itemId = await TestApp.SendAsync(command);

        var item = await TestApp.FindAsync<CodeSubmission>(itemId);

        item.ShouldNotBeNull();
        item!.ListId.ShouldBe(command.ListId);
        item.Title.ShouldBe(command.Title);
        item.CreatedBy.ShouldBe(userId);
        item.Created.ShouldBe(DateTime.Now, TimeSpan.FromMilliseconds(10000));
        item.LastModifiedBy.ShouldBe(userId);
        item.LastModified.ShouldBe(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}

