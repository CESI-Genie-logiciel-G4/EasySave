using EasySave.Services;

namespace EasySave.Helpers;

public static class ConsoleHelper
{
    private const int MinDefaultValue = 0;
    private const int MaxDefaultValue = 100;
    private const string ExitWord = "exit";
    
    private static string T(string key) => LocalizationService.GetString(key);
    
    public static void DisplaySeparator()
    {
        Console.WriteLine($"\n   +-  {new string('-',47)}  -+");
    }
    
    public static void DisplayMotd(Version version)
    {
        Console.WriteLine(
            $"""
                 ______                     _____                    
                |  ____|                   / ____|                   
                | |__    __ _  ___  _   _ | (___    __ _ __   __ ___ 
                |  __|  / _` |/ __|| | | | \___ \  / _` |\ \ / // _ \
                | |____| (_| |\__ \| |_| | ____) || (_| | \ V /|  __/
                |______|\__,_||___/ \__, ||_____/  \__,_|  \_/  \___|
                                     __/ |                           
                                    |___/         {T("Version")} {version.Major}.{version.Minor}
                                    
            """);
    }
    
    public static void Pause()
    {
        Console.Write("\n\t"+ T("PressAnyKey") + "\n");
        Console.ReadKey();
    }
    
    public static void Clear()
    {
        if (Console.IsOutputRedirected)
        {
            return;
        }
        Console.Clear();
    }

    private static void DisplayError(string key)
    {
        var cancel = string.Format(T("ExitToCancel"), ExitWord);
        Console.WriteLine($"\t- (!) {T(key)} [{cancel}]");
    }
    
    /// <summary>
    /// Reads a line from the console and trims it.
    ///
    /// @throws OperationCanceledException if the user types "exit".
    /// </summary>
    private static string ReadTrimmedConsole()
    {
        var input = Console.ReadLine()?.Trim() ?? string.Empty;
        
        if (input.ToLower().Equals(ExitWord))
        {
            throw new OperationCanceledException();
        }
        
        return input; 
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
            var entry = ReadTrimmedConsole();
            isValid = TryParseValidNumber(entry, min, max, out value);

            if (!isValid)
            {
                DisplayError("InvalidNumber");
            }
        } while (!isValid);

        return value;
    }

    public static string AskForString(string prompt, int min = 0, int max = MaxDefaultValue)
    {
        string value;
        bool isValid;

        do
        {
            
            var range = string.Format(T("StringLengthRange"), min, max);
            Console.Write($"{prompt} {range}");
            
            value = ReadTrimmedConsole();

            isValid = value.Length >= min && value.Length <= max;
            if (!isValid)
            {
                DisplayError("InvalidStringLength");
            }
        } while (!isValid);

        return value;
    }
    
    public static string AskForPath(string prompt)
    {
        while (true)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            Console.Write($"\t({currentDirectory})\n{prompt} : ");
            
            var path = ReadTrimmedConsole();

            if (Directory.Exists(path)) return path;
            
            DisplayError("InvalidDirectory");
        }
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
            var input = ReadTrimmedConsole();

            if (string.IsNullOrEmpty(input))
            {
                DisplayError("InvalidInput");
                continue;
            }

            var numbers = ParseIntRange(input, min, max);
            if (numbers != null) return numbers;
            
            DisplayError("InvalidFormat");
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
        if (TryParseValidNumber(input, min, max, out var number))
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
        if (parts.Length != 2) return null;

        if (!int.TryParse(parts[0], out var start) || !int.TryParse(parts[1], out var end))
            return null;
        
        if (start < min || start > max || end < min || end > max || start > end)
        {
            return null;
        }
        
        return start == end ? [start] : Enumerable.Range(start, end - start + 1).ToList();
    }

    /// <summary>
    /// Handles a list input "x;y;z" and returns a list of integers if valid.
    /// </summary>
    private static List<int>? ParseList(string input, int min, int max)
    {
        var parts = input.Split(';');
        HashSet<int> uniqueNumbers = [];

        foreach (var part in parts)
        {
            if (TryParseValidNumber(part, min, max, out var number))
            {
                uniqueNumbers.Add(number);
            }
            else
            {
                return null; // Cancel the entire list if any item is invalid
            }
        }

        return uniqueNumbers.ToList();
    }
}