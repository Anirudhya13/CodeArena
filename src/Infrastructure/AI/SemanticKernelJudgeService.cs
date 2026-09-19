using CodeArena.Application.Common.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace CodeArena.Infrastructure.AI;

public class SemanticKernelJudgeService : IAiJudgeService
{
    private readonly Kernel _kernel;
    private readonly ISubmissionNotifier _notifier;
    private readonly IUser _user;

    public SemanticKernelJudgeService(Kernel kernel, ISubmissionNotifier notifier, IUser user)
    {
        _kernel = kernel;
        _notifier = notifier;
        _user = user;
    }

    public async Task<string> EvaluateSubmissionAsync(string problemTitle, string problemDescription, string userCode, string language, CancellationToken cancellationToken = default)
    {
        bool isHintRequest = problemDescription.StartsWith("Provide hint") || problemDescription.StartsWith("Provide full solution");

        try
        {
            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            var systemPrompt = isHintRequest ? 
                @"You are CodeArena, an expert AI mentor for a LeetCode-style platform. The user is stuck and has asked for a hint or solution.
Analyze their current code and provide what is requested."
                :
                @"You are CodeArena, an expert AI code evaluator for a LeetCode-style platform.
You must evaluate the user's code submission based on the problem description.
Check for correctness, edge cases, and estimate the Big-O Time and Space complexity.
Return a structured markdown response with:
1. **Verdict**: Pass or Fail
2. **Time Complexity**: O(...)
3. **Space Complexity**: O(...)
4. **Feedback**: Brief explanation of bugs, edge cases missed, or optimizations.";

            var userPrompt = $"**Problem**: {problemTitle}\n**Description**: {problemDescription}\n\n**Language**: {language}\n**Code**:\n`{language}\n{userCode}\n`";

            var chatHistory = new ChatHistory(systemPrompt);
            chatHistory.AddUserMessage(userPrompt);

                        var userId = _user.Id ?? "Anonymous";
            await _notifier.NotifyStatusAsync(userId, "Compiling code...");
            await Task.Delay(500, cancellationToken); // Simulate compile step
            await _notifier.NotifyStatusAsync(userId, "Running Tests...");
            await Task.Delay(500, cancellationToken); // Simulate test runner
            await _notifier.NotifyStatusAsync(userId, "Analyzing complexity with AI...");
            
            var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory, cancellationToken: cancellationToken);
            return response.Content ?? (isHintRequest ? "Hint generation failed." : "Evaluation failed.");
        }
        catch (Exception ex)
        {
            var userId = _user.Id ?? "Anonymous";
            await _notifier.NotifyStatusAsync(userId, "AI API unavailable. Request failed.");
            
            return "**[SYSTEM ERROR]**\nThe AI Judging servers are currently overloaded or unavailable. Please check your API key or try submitting your code again in a few moments.\n\n**Error Details:**\n" + ex.Message;
        }
    }
}
