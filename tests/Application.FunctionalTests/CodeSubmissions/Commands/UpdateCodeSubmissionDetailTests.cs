using CodeArena.Application.CodeSubmissions.Commands.CreateCodeSubmission;
using CodeArena.Application.CodeSubmissions.Commands.UpdateCodeSubmission;
using CodeArena.Application.CodeSubmissions.Commands.UpdateCodeSubmissionDetail;
using CodeArena.Application.AlgorithmProblems.Commands.CreateAlgorithmProblem;
using CodeArena.Domain.Entities;
using CodeArena.Domain.Enums;

namespace CodeArena.Application.FunctionalTests.CodeSubmissions.Commands;

public class UpdateCodeSubmissionDetailTests : TestBase
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

        var command = new UpdateCodeSubmissionDetailCommand
        {
            Id = itemId,
            ListId = listId,
            Note = "This is the note.",
            Priority = PriorityLevel.High
        };

        await TestApp.SendAsync(command);

        var item = await TestApp.FindAsync<CodeSubmission>(itemId);

        item.ShouldNotBeNull();
        item!.ListId.ShouldBe(command.ListId);
        item.Note.ShouldBe(command.Note);
        item.Priority.ShouldBe(command.Priority);
        item.LastModifiedBy.ShouldNotBeNull();
        item.LastModifiedBy.ShouldBe(userId);
        item.LastModified.ShouldBe(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}

