using CodeArena.Application.CodeSubmissions.Commands.CreateCodeSubmission;
using CodeArena.Application.CodeSubmissions.Commands.UpdateCodeSubmission;
using CodeArena.Application.AlgorithmProblems.Commands.CreateAlgorithmProblem;
using CodeArena.Domain.Entities;

namespace CodeArena.Application.FunctionalTests.CodeSubmissions.Commands;

public class UpdateCodeSubmissionTests : TestBase
{
    [Test]
    public async Task ShouldRequireValidCodeSubmissionId()
    {
        var command = new UpdateCodeSubmissionCommand { Id = 99, Title = "New Title" };
        await Should.ThrowAsync<NotFoundException>(() => TestApp.SendAsync(command));
    }

    [Test]
    public async Task ShouldUpdateCodeSubmission()
    {
        var userId = await TestApp.RunAsDefaultUserAsync();

        var listId = await TestApp.SendAsync(new CreateAlgorithmProblemCommand
        {
            Title = "New List"
        });

        var itemId = await TestApp.SendAsync(new CreateCodeSubmissionCommand
        {
            ListId = listId,
            Title = "New Item"
        });

        var command = new UpdateCodeSubmissionCommand
        {
            Id = itemId,
            Title = "Updated Item Title"
        };

        await TestApp.SendAsync(command);

        var item = await TestApp.FindAsync<CodeSubmission>(itemId);

        item.ShouldNotBeNull();
        item!.Title.ShouldBe(command.Title);
        item.LastModifiedBy.ShouldNotBeNull();
        item.LastModifiedBy.ShouldBe(userId);
        item.LastModified.ShouldBe(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}

