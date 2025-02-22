namespace EasySave.Services;

public static class ExtensionService
{
    private static List<string> _encryptedExtensions = [];
    private static readonly Lock Lock = new();

    public static List<string> EncryptedExtensions
    {
        get
        {
            lock (Lock)
            {
                return [.._encryptedExtensions];
            }
        }
    }

    public static void SetEncryptedExtensions(List<string> extensions)
    {
        lock (Lock)
        {
            _encryptedExtensions = new List<string>(extensions.Select(e => e.ToLower()));
        }
    }

    public static void AddEncryptedExtension(string extension)
    {
        lock (Lock)
        {
            var lowerExt = extension.ToLower();
            if (!_encryptedExtensions.Contains(lowerExt))
            {
                _encryptedExtensions.Add(lowerExt);
            }
        }
    }

    public static void RemoveEncryptedExtension(string extension)
    {
        lock (Lock)
        {
            _encryptedExtensions.Remove(extension.ToLower());
        }
    }
}