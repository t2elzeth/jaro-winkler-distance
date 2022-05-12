namespace JaroWinkler;

public class Program
{
    public static void Main()
    {
        var user = new User
        {
            Name = "Oleg",
            Surname = "Meshechkov",
            MiddleName = null
        };

        const string compareWith = "олег мешечков";

        var possibilities = GetPossibilities(user);

        Console.WriteLine($"Found {possibilities.Count} possibilities");

        foreach (var possibility in possibilities)
        {
            var similarity = JaroWinklerDistance.Similarity(possibility, compareWith.ToLower());
            if (similarity >= 80)
                Console.WriteLine($"Similarity of word '{possibility}' and {compareWith} is {Math.Round(similarity, 2)} %");
        }
    }

    private static HashSet<string> GetPossibilities(User user)
    {
        var name = user.Name.ToLower();
        var surname = user.Surname.ToLower();
        var middleName = user.MiddleName?.ToLower() ?? "";

        var possibilities = new List<string>();

        var cyrillicName = Transliterator.ToCyrillic(name);
        var cyrillicSurname = Transliterator.ToCyrillic(surname);
        var cyrillicMiddleName = Transliterator.ToCyrillic(middleName);
        possibilities.AddRange(BuildFullName(cyrillicSurname, cyrillicName, cyrillicMiddleName));
        possibilities.AddRange(BuildFullName(cyrillicName, cyrillicSurname, cyrillicMiddleName));
        possibilities.AddRange(BuildFullName(cyrillicMiddleName, cyrillicName, cyrillicSurname));
        possibilities.AddRange(BuildFullName(cyrillicSurname, cyrillicMiddleName, cyrillicName));
        possibilities.AddRange(BuildFullName(cyrillicMiddleName, cyrillicName, cyrillicSurname));
        possibilities.AddRange(BuildFullName(cyrillicName, cyrillicMiddleName, cyrillicSurname));

        var latinName = Transliterator.ToLatin(name);
        var latinSurname = Transliterator.ToLatin(surname);
        var latinMiddleName = Transliterator.ToLatin(middleName);
        possibilities.AddRange(BuildFullName(latinSurname, latinName, latinMiddleName));
        possibilities.AddRange(BuildFullName(latinName, latinSurname, latinMiddleName));
        possibilities.AddRange(BuildFullName(latinMiddleName, latinName, latinSurname));
        possibilities.AddRange(BuildFullName(latinSurname, latinMiddleName, latinName));
        possibilities.AddRange(BuildFullName(latinMiddleName, latinName, latinSurname));
        possibilities.AddRange(BuildFullName(latinName, latinMiddleName, latinSurname));

        // to remove duplicated entries
        return possibilities.ToHashSet();
    }

    private static List<string> BuildFullName(string first, string second, string third)
    {
        return new List<string>
        {
            RemoveAllWhiteSpaces($"{first} {second} {third}"),
            RemoveAllWhiteSpaces($"{GetFirstLetter(first)} {second} {third}"),
            RemoveAllWhiteSpaces($"{first} {GetFirstLetter(second)} {third}"),
            RemoveAllWhiteSpaces($"{first} {second} {GetFirstLetter(third)}"),
        };
    }

    private static string GetFirstLetter(string word)
    {
        return word.Length == 0 ? "" : word[0].ToString();
    }

    private static string RemoveAllWhiteSpaces(string word)
    {
        return string.Join(" ", word.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}

public class User
{
    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string? MiddleName { get; set; }
}