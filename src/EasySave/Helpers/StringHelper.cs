namespace EasySave.Helpers;

public static class StringHelper
{
    public static string GetEllipsisSuffix(string path, int maxLength = 27)
    {
        if (string.IsNullOrEmpty(path)) return path;
        if (path.Length <= maxLength) return path;

        return "..." + path[^maxLength..];
    }
}