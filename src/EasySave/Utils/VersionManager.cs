using System.Reflection;

namespace EasySave.Utils;

public static class VersionManager
{
    public static Version Version => Assembly.GetExecutingAssembly().GetName().Version ?? new Version();
}