namespace EasySave.Services;

public static class PriorityService
{
    public static string[]? GetSortedFile(string rootFolder)
    {
        var files = Directory.GetFiles(rootFolder, "*", SearchOption.AllDirectories);
        return SortFilesByPriority(files);
    }
    
    /// <summary>
    ///     Sort file list with the calculate priority
    /// </summary>
    /// <param name="files"></param>
    /// <returns>
    ///     Sorted list of file path
    /// </returns>
    public static string[]? SortFilesByPriority(string[]? files)
    {
        if (files == null) return null;
        return files.OrderByDescending(GetPriority).ToArray();
    }
    
    /// <summary>
    ///     Calculate a level of priority based on the file extension
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>
    ///     For the moment return 0 or 1 but could return 0 to n
    /// </returns>
    private static int GetPriority(string fileName)
    {
        string extension = Path.GetExtension(fileName);
        if(!ExtensionService.PriorityExtensions.Contains(extension)) return 0;
        return 1;
    }
}