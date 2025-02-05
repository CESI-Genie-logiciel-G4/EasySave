namespace EasySave.Helpers;

public static class ConsoleHelper
{
    private const int MinDefaultValue = 0;
    private const int MaxDefaultValue = 100;
    
    public const string Motd = 
        """
             ______                     _____                    
            |  ____|                   / ____|                   
            | |__    __ _  ___  _   _ | (___    __ _ __   __ ___ 
            |  __|  / _` |/ __|| | | | \___ \  / _` |\ \ / // _ \
            | |____| (_| |\__ \| |_| | ____) || (_| | \ V /|  __/
            |______|\__,_||___/ \__, ||_____/  \__,_|  \_/  \___|
                                 __/ |                           
                                |___/   
        """;
    
    public const string Separator = 
        "\n   +-  --------------------------------------------  -*\n";

    public static void Pause()
    {
        Console.Write("\nPress any key to continue...");
        Console.Read();
    }
    
    private static bool TryParseValidNumber(string input, int min, int max, out int number)
    {
        var isNumber = int.TryParse(input, out number);
        return isNumber && number >= min && number <= max;
    }

    public static int AskForInt(string prompt, int min = MinDefaultValue, int max = MaxDefaultValue)
    {
        int value;
        bool isValid;

        do
        {
            Console.Write($"{prompt} ({min}-{max}) : ");
            var entry = Console.ReadLine()?.Trim() ?? string.Empty;
            isValid = TryParseValidNumber(entry, min, max, out value);

            if (!isValid)
            {
                Console.WriteLine("\t❌ Please enter a valid number.");
                continue;
            }
            
            isValid = true;
        } while (!isValid);

        return value;
    }

    public static string AskForString(string prompt, int min = 0, int max = MaxDefaultValue)
    {
        string value;
        bool isValid;

        do
        {
            Console.Write($"{prompt} ({min}-{max} characters) : ");
            value = Console.ReadLine() ?? string.Empty;

            isValid = value.Length >= min && value.Length <= max;
            if (!isValid)
            {
                Console.WriteLine($"\t❌ Please enter a string between {min} and {max} characters.");
            }
        } while (!isValid);

        return value;
    }
    
    /// <summary>
    /// Asks the user for a list of integers.
    ///
    /// The user can enter a single number (x), a range (x-y), a list (x;y;z) or all numbers (*).
    /// </summary>
    public static List<int> AskForMultipleValues(string prompt, int min = MinDefaultValue, int max = MaxDefaultValue)
    {
        while (true)
        {
            Console.Write($"{prompt} ({min}-{max}) : ");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("\t❌ Invalid input. Please try again.");
                continue;
            }

            var numbers = ParseIntRange(input, min, max);
            if (numbers != null) return numbers;

            Console.WriteLine("\t❌ Invalid format. Try: `5`, `2-5`, `1;3;7`, `*`.");
        }
    }

    /// <summary>
    /// Parses an input string and returns a list of integers if valid, otherwise null.
    /// </summary>
    private static List<int>? ParseIntRange(string input, int min, int max)
    {
        if (input == "*")
        {
            return Enumerable.Range(min, max - min + 1).ToList();
        }
        if (input.Contains('-'))
        {
            return ParseRange(input, min, max);
        }

        if (input.Contains(';'))
        {
            return ParseList(input, min, max);
        }

        return ParseSingle(input, min, max);
    }

    /// <summary>
    /// Handles a single number input ("5") and returns a list containing only that number.
    /// </summary>
    private static List<int>? ParseSingle(string input, int min, int max)
    {
        if (int.TryParse(input, out var number) && number >= min && number <= max)
        {
            return [number];
        }

        return null;
    }

    /// <summary>
    /// Handles a range input "x-y" and returns a list of integers if valid.
    /// </summary>
    private static List<int>? ParseRange(string input, int min, int max)
    {
        var parts = input.Split('-');

        if (parts.Length != 2)
        {
            return null;
        }

        if (
            !int.TryParse(parts[0], out var start) ||
            !int.TryParse(parts[1], out var end)
        )
            return null;

        if (start < min || start > max ||
            end < min || end > max ||
            start > end
           )
        {
            return null;
        }

        return Enumerable.Range(start, end - start + 1).ToList();
    }

    /// <summary>
    /// Handles a list input "x;y;z" and returns a list of integers if valid.
    /// </summary>
    private static List<int>? ParseList(string input, int min, int max)
    {
        var parts = input.Split(';');
        List<int> numbers = [];

        foreach (var part in parts)
        {
            if (int.TryParse(part, out var number) && number >= min && number <= max)
            {
                numbers.Add(number);
            }
            else
            {
                return null; // Cancel the entire list if any item is invalid
            }
        }

        return numbers;
    }
}