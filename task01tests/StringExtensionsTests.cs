using Xunit;
using task01;
namespace task01tests;

public class StringExtensionsTests
{
    [Fact]
    public void IsPalindrome_ValidPalindrome_ReturnsTrue()
    {
        string input = "А роза упала на лапу Азора";
        Assert.True(input.IsPalindrome());
    }

    [Fact]
    public void IsPalindrome_NotPalindrome_ReturnsFalse()
    {
        string input = "Hello, world!";
        Assert.False(input.IsPalindrome());
    }

    [Fact]
    public void IsPalindrome_EmptyString_ReturnsFalse()
    {
        string input = "";
        Assert.False(input.IsPalindrome());
    }

    [Fact]
    public void IsPalindrome_WithPunctuation_IgnoresPunctuation()
    {
        string input = "Was it a car or a cat I saw?";
        Assert.True(input.IsPalindrome());
    }
}