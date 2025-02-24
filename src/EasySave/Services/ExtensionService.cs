using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using EasySave.Exceptions;
using EasySave.Helpers;

namespace EasySave.Services;

public static class ExtensionService
{
    private const string Path = "./.easysave/encryptedExtensions.json";

    public static readonly ObservableCollection<string> EncryptedExtensions = [];

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

    private static void StoreEncryptedExtensions()
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
    ///     <c>extension</c> if the extension was added to the list, <c>null</c> otherwise.
    /// </returns>
    /// <remarks>
    ///   If the extension is not prefixed with a dot, it will be added with a dot.
    /// </remarks>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static string? AddEncryptedExtension(string extension)
    {
        var lowerExt = extension.ToLower();

        if (!lowerExt.StartsWith('.'))
        {
            lowerExt = lowerExt.Insert(0, ".");
        }
        
        if (EncryptedExtensions.Contains(lowerExt)) 
            throw new AlreadyExistException("Extension already exists in the list : ", lowerExt);
        EncryptedExtensions.Add(lowerExt);
            
        StoreEncryptedExtensions();
        return lowerExt;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void RemoveEncryptedExtension(string extension)
    {
        EncryptedExtensions.Remove(extension.ToLower());
        StoreEncryptedExtensions();
    }
}