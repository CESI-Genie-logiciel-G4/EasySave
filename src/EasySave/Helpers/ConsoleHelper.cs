using EasySave.Services;

namespace EasySave.Helpers;

public class ConsoleHelper
{
    private readonly LocalizationService _localizationService;
    private readonly Func<string, string> _translate;
    
    public ConsoleHelper(LocalizationService localizationService)
    {
        _localizationService = localizationService;
        _translate = localizationService.GetString;
    }
    
    private const int MinDefaultValue = 0;
    private const int MaxDefaultValue = 100;
    private const string ExitWord = "exit";
    
    
    public void DisplaySeparator()
    {
        Console.WriteLine($"\n   +-  {new string('-',47)}  -+");
    }
    
    public void DisplayMotd(Version version)
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
                                    |___/         {_translate("Version")} {version.Major}.{version.Minor}
                                    
            """);
    }
    
    public void Pause()
    {
        Console.Write("\n\t"+ _translate("PressAnyKey") + "\n");
        if (Console.IsOutputRedirected) return;
        Console.ReadKey();
    }
    
    public void Clear()
    {
        if (Console.IsOutputRedirected) return;
        Console.Clear();
    }

    public void DisplayError(string error, bool enableCancel = true)
    {
        var cancel = string.Format(_translate("ExitToCancel"), ExitWord);
        var cancelMessage = enableCancel ? $"[{cancel}]" : string.Empty;
        
        Console.WriteLine($"\t\t(!) {error} {cancelMessage}\n");
    }
    
    /// <summary>
    /// Reads a line from the console and trims it.
    ///
    /// @throws OperationCanceledException if the user types "exit".
    /// </summary>
    private string ReadTrimmedConsole()
    {
        var input = Console.ReadLine()?.Trim() ?? string.Empty;
        
        if (input.ToLower().Equals(ExitWord))
        {
            throw new OperationCanceledException();
        }
        
        return input; 
    }
    
    private bool TryParseValidNumber(string input, int min, int max, out int number)
    {
        var isNumber = int.TryParse(input, out number);
        return isNumber && number >= min && number <= max;
    }

    public int AskForInt(string prompt, int min = MinDefaultValue, int max = MaxDefaultValue)
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
                DisplayError(_translate("InvalidNumber"));
            }
        } while (!isValid);

        return value;
    }

    public string AskForString(string prompt, int min = 0, int max = MaxDefaultValue)
    {
        string value;
        bool isValid;

        do
        {
            
            var range = string.Format(_translate("StringLengthRange"), min, max);
            Console.Write($"{prompt} {range}");
            
            value = ReadTrimmedConsole();

            isValid = value.Length >= min && value.Length <= max;
            if (!isValid)
            {
                DisplayError(_translate("InvalidStringLength"));
            }
        } while (!isValid);

        return value;
    }
    
    public string AskForPath(string prompt)
    {
        while (true)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            Console.Write($"\t({currentDirectory})\n{prompt} : ");
            
            var path = ReadTrimmedConsole();

            if (Directory.Exists(path)) return path;
            
            DisplayError(_translate("InvalidDirectory"));
        }
    }
    
    /// <summary>
    /// Asks the user for a list of integers.
    ///
    /// The user can enter a single number (x), a range (x-y), a list (x;y;z) or all numbers (*).
    /// </summary>
    public List<int> AskForMultipleValues(string prompt, int min = MinDefaultValue, int max = MaxDefaultValue)
    {
        while (true)
        {
            Console.Write($"{prompt} ({min}-{max}) : ");
            var input = ReadTrimmedConsole();

            if (string.IsNullOrEmpty(input))
            {
                DisplayError(_translate("InvalidInput"));
                continue;
            }

            var numbers = ParseIntRange(input, min, max);
            if (numbers != null) return numbers;
            
            DisplayError(_translate("InvalidFormat"));
        }
    }

    /// <summary>
    /// Parses an input string and returns a list of integers if valid, otherwise null.
    /// </summary>
    private List<int>? ParseIntRange(string input, int min, int max)
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
    private List<int>? ParseSingle(string input, int min, int max)
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
    private List<int>? ParseRange(string input, int min, int max)
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
    private List<int>? ParseList(string input, int min, int max)
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