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
        catch (Exception ex) when (ex.Message.Contains("dummy-key") || ex.Message.Contains("Incorrect API key") || ex.Message.Contains("invalid_api_key"))
        {
            string codeLower = userCode.ToLower();
            bool isEmpty = string.IsNullOrWhiteSpace(userCode) || (!codeLower.Contains("return") && !codeLower.Contains("print") && !codeLower.Contains("yield") && !codeLower.Contains("for") && !codeLower.Contains("while"));
            
            if (problemDescription.StartsWith("Provide hint #1") || problemDescription.StartsWith("Provide hint #2") || problemDescription.StartsWith("Provide hint #3"))
            {
                if (isEmpty) return "**[SMART MOCK HINT]**\nI'm looking at your editor, and it seems you haven't implemented any logic yet! Start by defining the basic variables or data structures you might need to solve this problem.";
                
                if (problemTitle == "Two Sum") {
                    if (!codeLower.Contains("dictionary") && !codeLower.Contains("map") && !codeLower.Contains("dict")) {
                        return "**[SMART MOCK HINT 1/3]**\nI'm analyzing your code, and I don't see a `Dictionary` or `Map` initialized yet! Using nested loops will give you O(N^2) time complexity. Try initializing a Hash Map to store values you've seen.";
                    } else {
                        return "**[SMART MOCK HINT 1/3]**\nGreat job starting with a Map! Now, as you iterate through the array, remember you need to check if the complement (`target - current_number`) exists in the map.";
                    }
                }
                
                return "**[MOCK AI HINT 1/3]**\nTry breaking the problem down into smaller steps. What data structures allow for fast lookups? Nested loops will give you O(N^2) time complexity, but you can do better.";
            }
            else if (problemDescription.StartsWith("Provide hint #2"))
            {
                return "**[MOCK AI HINT 2/3]**\nA Hash Map (or Dictionary) is perfect here. As you iterate through the elements, can you store the ones you've already seen to check against future elements?";
            }
            else if (problemDescription.StartsWith("Provide hint #3"))
            {
                return "**[MOCK AI HINT 3/3]**\nFor each element `x`, mathematically check if the required complementary value (e.g., `target - x`) is already in your Hash Map. If it is, you've found your answer!";
            }
            else if (problemDescription.StartsWith("Provide full solution"))
            {
                string sol = "";
                if (problemTitle == "Two Sum") {
                    if (language == "python") sol = "class Solution:\n    def twoSum(self, nums: List[int], target: int) -> List[int]:\n        prevMap = {} # val : index\n        for i, n in enumerate(nums):\n            diff = target - n\n            if diff in prevMap:\n                return [prevMap[diff], i]\n            prevMap[n] = i\n        return []";
                    else if (language == "java") sol = "class Solution {\n    public int[] twoSum(int[] nums, int target) {\n        HashMap<Integer, Integer> prevMap = new HashMap<>();\n        for (int i = 0; i < nums.length; i++) {\n            int diff = target - nums[i];\n            if (prevMap.containsKey(diff)) {\n                return new int[] { prevMap.get(diff), i };\n            }\n            prevMap.put(nums[i], i);\n        }\n        return new int[0];\n    }\n}";
                    else if (language == "javascript") sol = "var twoSum = function(nums, target) {\n    const prevMap = new Map();\n    for (let i = 0; i < nums.length; i++) {\n        const diff = target - nums[i];\n        if (prevMap.has(diff)) {\n            return [prevMap.get(diff), i];\n        }\n        prevMap.set(nums[i], i);\n    }\n    return [];\n};";
                    else sol = "public class Solution {\n    public int[] TwoSum(int[] nums, int target) {\n        Dictionary<int, int> prevMap = new Dictionary<int, int>();\n        for (int i = 0; i < nums.Length; i++) {\n            int diff = target - nums[i];\n            if (prevMap.ContainsKey(diff)) {\n                return new int[] { prevMap[diff], i };\n            }\n            prevMap[nums[i]] = i;\n        }\n        return new int[0];\n    }\n}";
                }
                else if (problemTitle == "Valid Palindrome") {
                    if (language == "python") sol = "class Solution:\n    def isPalindrome(self, s: str) -> bool:\n        l, r = 0, len(s) - 1\n        while l < r:\n            while l < r and not self.alphaNum(s[l]): l += 1\n            while l < r and not self.alphaNum(s[r]): r -= 1\n            if s[l].lower() != s[r].lower(): return False\n            l, r = l + 1, r - 1\n        return True\n    def alphaNum(self, c): return (ord('A') <= ord(c) <= ord('Z') or ord('a') <= ord(c) <= ord('z') or ord('0') <= ord(c) <= ord('9'))";
                    else sol = "public class Solution {\n    public bool IsPalindrome(string s) {\n        int l = 0, r = s.Length - 1;\n        while (l < r) {\n            while (l < r && !char.IsLetterOrDigit(s[l])) l++;\n            while (l < r && !char.IsLetterOrDigit(s[r])) r--;\n            if (char.ToLower(s[l]) != char.ToLower(s[r])) return false;\n            l++; r--;\n        }\n        return true;\n    }\n}";
                }
                else if (problemTitle == "LRU Cache") {
                    if (language == "python") sol = "class LRUCache:\n    def __init__(self, capacity: int):\n        self.cache = {}\n        self.capacity = capacity\n\n    def get(self, key: int) -> int:\n        if key not in self.cache:\n            return -1\n        self.cache[key] = self.cache.pop(key)\n        return self.cache[key]\n\n    def put(self, key: int, value: int) -> None:\n        if key in self.cache:\n            self.cache.pop(key)\n        self.cache[key] = value\n        if len(self.cache) > self.capacity:\n            self.cache.pop(next(iter(self.cache)))";
                    else if (language == "java") sol = "import java.util.LinkedHashMap;\nimport java.util.Map;\n\nclass LRUCache {\n    private LinkedHashMap<Integer, Integer> cache;\n    private int capacity;\n\n    public LRUCache(int capacity) {\n        this.capacity = capacity;\n        this.cache = new LinkedHashMap<Integer, Integer>(capacity, 0.75f, true) {\n            protected boolean removeEldestEntry(Map.Entry<Integer, Integer> eldest) {\n                return size() > capacity;\n            }\n        };\n    }\n    \n    public int get(int key) {\n        return cache.getOrDefault(key, -1);\n    }\n    \n    public void put(int key, int value) {\n        cache.put(key, value);\n    }\n}";
                    else if (language == "javascript") sol = "var LRUCache = function(capacity) {\n    this.cache = new Map();\n    this.capacity = capacity;\n};\n\nLRUCache.prototype.get = function(key) {\n    if (!this.cache.has(key)) return -1;\n    const val = this.cache.get(key);\n    this.cache.delete(key);\n    this.cache.set(key, val);\n    return val;\n};\n\nLRUCache.prototype.put = function(key, value) {\n    if (this.cache.has(key)) this.cache.delete(key);\n    this.cache.set(key, value);\n    if (this.cache.size > this.capacity) {\n        this.cache.delete(this.cache.keys().next().value);\n    }\n};";
                    else sol = "public class LRUCache {\n    class Node {\n        public int Key, Val;\n        public Node Prev, Next;\n        public Node(int k, int v) { Key = k; Val = v; }\n    }\n    private int _capacity;\n    private Dictionary<int, Node> _cache;\n    private Node _head, _tail;\n    \n    public LRUCache(int capacity) {\n        _capacity = capacity;\n        _cache = new Dictionary<int, Node>();\n        _head = new Node(-1, -1);\n        _tail = new Node(-1, -1);\n        _head.Next = _tail;\n        _tail.Prev = _head;\n    }\n    \n    private void Remove(Node node) {\n        node.Prev.Next = node.Next;\n        node.Next.Prev = node.Prev;\n    }\n    \n    private void Insert(Node node) {\n        node.Next = _head.Next;\n        node.Next.Prev = node;\n        _head.Next = node;\n        node.Prev = _head;\n    }\n    \n    public int Get(int key) {\n        if (!_cache.ContainsKey(key)) return -1;\n        var node = _cache[key];\n        Remove(node);\n        Insert(node);\n        return node.Val;\n    }\n    \n    public void Put(int key, int value) {\n        if (_cache.ContainsKey(key)) {\n            Remove(_cache[key]);\n        }\n        var newNode = new Node(key, value);\n        _cache[key] = newNode;\n        Insert(newNode);\n        \n        if (_cache.Count > _capacity) {\n            var lru = _tail.Prev;\n            Remove(lru);\n            _cache.Remove(lru.Key);\n        }\n    }\n}";
                }
                else {
                    sol = $"// Complete {language} solution for {problemTitle} is successfully unlocked! (Mocked for testing purposes)";
                }
                
                return $"**[MOCK FULL SOLUTION]**\nHere is the optimal solution in `{language}`:\n\n```{language}\n{sol}\n```";
            }
            else
            {
                if (isEmpty)
                {
                    return "**[SMART MOCK JUDGE]** (Simulated AI Inference)\n\n1. **Verdict**: Fail (Empty Submission)\n2. **Test Cases**: 0 / 3 Passed\n   - \u274C `Test Case 1` -> Failed (No Output)\n   - \u274C `Test Case 2` -> Failed (No Output)\n   - \u274C `Test Case 3` -> Failed (No Output)\n3. **Feedback**: You submitted the boilerplate code without implementing any logic!";
                }

                if (problemTitle == "Two Sum")
                {
                    if (!codeLower.Contains("if") || !codeLower.Contains("target"))
                    {
                        return "**[SMART MOCK JUDGE]** (Simulated AI Inference)\n\n1. **Verdict**: Fail (Wrong Answer)\n2. **Test Cases**: 0 / 3 Passed\n   - \u274C `nums=[2,7,11,15], target=9` -> Output: Incorrect, Expected: `[0,1]`\n   - \u274C `nums=[3,2,4], target=6` -> Output: Incorrect, Expected: `[1,2]`\n   - \u274C `nums=[3,3], target=6` -> Output: Incorrect, Expected: `[0,1]`\n3. **Feedback**: Your code failed because it lacks basic conditional checks. You must check if the complement (target - current_number) exists!";
                    }
                    else if (codeLower.Contains("sort"))
                    {
                        return "**[SMART MOCK JUDGE]** (Simulated AI Inference)\n\n1. **Verdict**: Pass (Accepted)\n2. **Test Cases**: 3 / 3 Passed\n   - \u2714 `nums=[2,7,11,15], target=9` -> Passed (4ms)\n   - \u2714 `nums=[3,2,4], target=6` -> Passed (5ms)\n   - \u2714 `nums=[3,3], target=6` -> Passed (4ms)\n3. **Time Complexity**: O(N log N)\n4. **Space Complexity**: O(1) or O(N) depending on sorting algorithm\n5. **Feedback**: Good job! Sorting the array and using a two-pointer approach (or binary search) achieves O(N log N) time complexity. However, an even faster O(N) solution is possible using a Hash Map!";
                    }
                    else if (!codeLower.Contains("dictionary") && !codeLower.Contains("map") && !codeLower.Contains("dict") && codeLower.Contains("for"))
                    {
                        return "**[SMART MOCK JUDGE]** (Simulated AI Inference)\n\n1. **Verdict**: Fail (Time Limit Exceeded)\n2. **Test Cases**: 2 / 3 Passed\n   - \u2714 `nums=[2,7,11,15], target=9` -> Passed (42ms)\n   - \u2714 `nums=[3,2,4], target=6` -> Passed (38ms)\n   - \u274C `nums=[large array...], target=19999` -> Failed (Time Limit Exceeded)\n3. **Time Complexity**: O(N^2)\n4. **Feedback**: Your solution passes basic tests but uses nested loops, resulting in O(N^2) complexity. This times out on large inputs. Try using a Hash Map!";
                    }
                    else if (codeLower.Contains("dictionary") || codeLower.Contains("map") || codeLower.Contains("dict"))
                    {
                        return "**[SMART MOCK JUDGE]** (Simulated AI Inference)\n\n1. **Verdict**: Pass (Accepted)\n2. **Test Cases**: 3 / 3 Passed\n   - \u2714 `nums=[2,7,11,15], target=9` -> Passed (1ms)\n   - \u2714 `nums=[3,2,4], target=6` -> Passed (2ms)\n   - \u2714 `nums=[3,3], target=6` -> Passed (1ms)\n3. **Time Complexity**: O(N)\n4. **Space Complexity**: O(N)\n5. **Feedback**: Excellent! Using auxiliary space for an O(N) lookup was the optimal choice.";
                    }
                    else 
                    {
                        return "**[SMART MOCK JUDGE]** (Simulated AI Inference)\n\n1. **Verdict**: Fail (Wrong Answer)\n2. **Test Cases**: 1 / 3 Passed\n   - \u2714 `nums=[2,7,11,15], target=9` -> Passed (1ms)\n   - \u274C `nums=[3,2,4], target=6` -> Output: Incorrect, Expected: `[1,2]`\n   - \u274C `nums=[3,3], target=6` -> Output: Incorrect, Expected: `[0,1]`\n3. **Feedback**: Your logic is incomplete and failed several test cases. Please review your algorithmic approach and try again.";
                    }
                }
                
                // Generic fallback for other problems
                if (!codeLower.Contains("if") && !codeLower.Contains("for") && !codeLower.Contains("while"))
                {
                    return "**[SMART MOCK JUDGE]** (Simulated AI Inference)\n\n1. **Verdict**: Fail (Wrong Answer)\n2. **Test Cases**: 0 / 3 Passed\n3. **Feedback**: Your code lacks the necessary logical constructs to solve this problem. Please try again.";
                }

                return "**[MOCK AI JUDGE]** (Simulated AI Inference)\n\n1. **Verdict**: Pass\n2. **Test Cases**: 3 / 3 Passed\n3. **Feedback**: Great approach! Your logic is sound, but consider explicitly handling empty input edge cases.";
            }
        }
    }
}


