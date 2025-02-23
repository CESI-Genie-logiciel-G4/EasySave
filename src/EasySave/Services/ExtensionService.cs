using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using EasySave.Helpers;

namespace EasySave.Services;

public static class ExtensionService
{
    private static String Path = "./.easysave/encryptedExtensions.json";
    
    public static readonly ObservableCollection<string> EncryptedExtensions = new();

    public static void LoadEncryptedExtensions()
    {
        if (!File.Exists(Path)) return;
        var json = File.ReadAllText(Path);
        EncryptedExtensions.Clear();
        foreach (var ext in JsonSerializer.Deserialize<ObservableCollection<string>>(json)!)
        {
            EncryptedExtensions.Add(ext.ToLower());
        }
    }

    public static void StoreEncryptedExtensions()
    {
        FileHelper.CreateParentDirectory(Path);
        var json = JsonSerializer.Serialize(EncryptedExtensions);
        FileHelper.CreateAndWrite(Path, json);
    }
    
    /// <summary>
    ///   Add an extension to the list of encrypted extensions.
    /// </summary>
    /// <param name="extension"></param>
    /// <returns>
    ///     <c>true</c> if the extension was added to the list, <c>false</c> otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static bool AddEncryptedExtension(string extension)
    {
        var lowerExt = extension.ToLower();
        
        if (EncryptedExtensions.Contains(lowerExt)) return false;
        EncryptedExtensions.Add(lowerExt);
            
        StoreEncryptedExtensions();
        return true;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void RemoveEncryptedExtension(string extension)
    {
        EncryptedExtensions.Remove(extension.ToLower());
        StoreEncryptedExtensions();
    }
}