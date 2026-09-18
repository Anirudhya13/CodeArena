using CodeArena.Application.AlgorithmProblems.Queries.GetProblems;
using CodeArena.Domain.Entities;
using CodeArena.Domain.ValueObjects;

namespace CodeArena.Application.FunctionalTests.AlgorithmProblems.Queries;

public class GetProblemsTests : TestBase
{
    [Test]
    public async Task ShouldReturnPriorityLevels()
    {
        await TestApp.RunAsDefaultUserAsync();

        var query = new GetProblemsQuery();

        var result = await TestApp.SendAsync(query);

        result.PriorityLevels.ShouldNotBeEmpty();
    }

    [Test]
    public async Task ShouldReturnAllListsAndItems()
    {
        await TestApp.RunAsDefaultUserAsync();

        await TestApp.AddAsync(new AlgorithmProblem
        {
            Title = "Shopping",
            Colour = Colour.Blue,
            Items =
                {
                    new CodeSubmission { Title = "Apples", Done = true },
                    new CodeSubmission { Title = "Milk", Done = true },
                    new CodeSubmission { Title = "Bread", Done = true },
                    new CodeSubmission { Title = "Toilet paper" },
                    new CodeSubmission { Title = "Pasta" },
                    new CodeSubmission { Title = "Tissues" },
                    new CodeSubmission { Title = "Tuna" }
                }
        });

        var query = new GetProblemsQuery();

        var result = await TestApp.SendAsync(query);

        result.Lists.Count.ShouldBe(1);
        result.Lists.First().Items.Count.ShouldBe(7);
    }

    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new GetProblemsQuery();

        var action = () => TestApp.SendAsync(query);

        await Should.ThrowAsync<UnauthorizedAccessException>(action);
    }
}

