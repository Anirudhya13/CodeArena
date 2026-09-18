using AlgoJudge.Domain.Constants;
using AlgoJudge.Domain.Entities;
using AlgoJudge.Domain.ValueObjects;
using AlgoJudge.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AlgoJudge.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            // See https://jasontaylor.dev/ef-core-database-initialisation-strategies
            // await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole(Roles.Administrator);

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        // Default users
        var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Administrator1!");
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await _userManager.AddToRolesAsync(administrator, new [] { administratorRole.Name });
            }
        }

        // Default data
        // Seed, if necessary
                                if (!_context.AlgorithmProblems.Any())
        {
            _context.AlgorithmProblems.AddRange(new[]
            {
                new AlgorithmProblem
                {
                    Title = "Two Sum",
                    Description = "Given an array of integers `nums` and an integer `target`, return indices of the two numbers such that they add up to `target`.\n\nYou may assume that each input would have exactly one solution, and you may not use the same element twice.\n\n**Example 1:**\nInput: nums = [2,7,11,15], target = 9\nOutput: [0,1]",
                    Difficulty = "Easy",
                    StarterCode = "{\"csharp\":\"public class Solution {\\n    public int[] TwoSum(int[] nums, int target) {\\n        \\n    }\\n}\",\"python\":\"class Solution:\\n    def twoSum(self, nums: List[int], target: int) -> List[int]:\\n        pass\",\"java\":\"class Solution {\\n    public int[] twoSum(int[] nums, int target) {\\n        \\n    }\\n}\",\"javascript\":\"/**\\n * @param {number[]} nums\\n * @param {number} target\\n * @return {number[]}\\n */\\nvar twoSum = function(nums, target) {\\n    \\n};\"}",
                    Colour = Colour.Green
                },
                new AlgorithmProblem
                {
                    Title = "Valid Palindrome",
                    Description = "A phrase is a palindrome if, after converting all uppercase letters into lowercase letters and removing all non-alphanumeric characters, it reads the same forward and backward.\n\nGiven a string `s`, return `true` if it is a palindrome, or `false` otherwise.\n\n**Example:**\nInput: s = \"A man, a plan, a canal: Panama\"\nOutput: true",
                    Difficulty = "Easy",
                    StarterCode = "{\"csharp\":\"public class Solution {\\n    public bool IsPalindrome(string s) {\\n        \\n    }\\n}\",\"python\":\"class Solution:\\n    def isPalindrome(self, s: str) -> bool:\\n        pass\",\"java\":\"class Solution {\\n    public boolean isPalindrome(String s) {\\n        \\n    }\\n}\",\"javascript\":\"/**\\n * @param {string} s\\n * @return {boolean}\\n */\\nvar isPalindrome = function(s) {\\n    \\n};\"}",
                    Colour = Colour.Blue
                },
                new AlgorithmProblem
                {
                    Title = "LRU Cache",
                    Description = "Design a data structure that follows the constraints of a Least Recently Used (LRU) cache.\n\nImplement the `LRUCache` class:\n- `LRUCache(int capacity)` Initialize the LRU cache with positive size capacity.\n- `int get(int key)` Return the value of the key if the key exists, otherwise return -1.\n- `void put(int key, int value)` Update the value of the key if the key exists. Otherwise, add the key-value pair to the cache. If the number of keys exceeds the capacity from this operation, evict the least recently used key.",
                    Difficulty = "Medium",
                    StarterCode = "{\"csharp\":\"public class LRUCache {\\n    public LRUCache(int capacity) {\\n        \\n    }\\n    \\n    public int Get(int key) {\\n        \\n    }\\n    \\n    public void Put(int key, int value) {\\n        \\n    }\\n}\",\"python\":\"class LRUCache:\\n    def __init__(self, capacity: int):\\n        pass\\n\\n    def get(self, key: int) -> int:\\n        pass\\n\\n    def put(self, key: int, value: int) -> None:\\n        pass\",\"java\":\"class LRUCache {\\n    public LRUCache(int capacity) {\\n        \\n    }\\n    \\n    public int get(int key) {\\n        \\n    }\\n    \\n    public void put(int key, int value) {\\n        \\n    }\\n}\",\"javascript\":\"/**\\n * @param {number} capacity\\n */\\nvar LRUCache = function(capacity) {\\n    \\n};\\n\\nLRUCache.prototype.get = function(key) {\\n    \\n};\\n\\nLRUCache.prototype.put = function(key, value) {\\n    \\n};\"}",
                    Colour = Colour.Orange
                }
            });

            await _context.SaveChangesAsync();
        }
    }
}





