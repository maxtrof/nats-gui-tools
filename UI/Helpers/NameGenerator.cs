using System;

namespace UI.Helpers;

/// <summary>
/// Provides helper functions to generate random names
/// </summary>
internal static class NameGenerator
{
    private static readonly Random Rnd = new();
    private static readonly string[] Consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
    private static readonly string[] Vowels = { "a", "e", "i", "o", "u", "ae", "y" };

    /// <summary>
    /// Generates random spellable names
    /// </summary>
    /// <param name="advanced">If true - generates a name with two words and digits</param>
    public static string GetRandomName(bool advanced = false)
    {
        string name = "";
        name += Consonants[Rnd.Next(Consonants.Length)].ToUpper();
        name += Vowels[Rnd.Next(Vowels.Length)];
        int b = 2;
        var lenght = advanced
            ? Rnd.Next(4, 8)
            : Rnd.Next(2, 4);
        while (b < lenght)
        {
            name += Consonants[Rnd.Next(Consonants.Length)];
            b++;
            name += Vowels[Rnd.Next(Vowels.Length)];
            b++;
        }

        if (!advanced) return name;
        
        if (Rnd.Next(0, 100) > 50) name += GetRandomName();
        if (Rnd.Next(0, 30) > 20) name += Rnd.Next(1, 999);

        return name;
    }
}