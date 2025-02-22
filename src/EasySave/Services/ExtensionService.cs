namespace EasySave.Services
{
    public static class ExtensionService
    {
        public static List<string> EncryptedExtensions { get; private set; } = [];

        public static void SetEncryptedExtensions(List<string> extensions)
        {
            EncryptedExtensions = new List<string>(extensions);
        }

        public static void AddEncryptedExtension(string extension)
        {
            if (!EncryptedExtensions.Contains(extension))
            {
                EncryptedExtensions.Add(extension);
            }
        }

        public static void RemoveEncryptedExtension(string extension)
        {
            EncryptedExtensions.Remove(extension);
        }
    }
}
