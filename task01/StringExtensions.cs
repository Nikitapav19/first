namespace task01;

public static class StringExtensions
{
    public static bool IsPalindrome(this string input)
    {
        List<char> output = new List<char>();
        foreach (char el in input)
        {
            if (Char.IsWhiteSpace(el) || Char.IsPunctuation(el)) continue;
            output.Add(Char.ToLower(el));  // [а, б, а]
        }
        int length = output.Count();
        if (length == 0) return false;  // Случай с пустой строкой
        for (int i=0; i<length / 2; i++)
        {
            if (output[i] != output[length-i-1]) return false;
        }
        return true;
    }
}