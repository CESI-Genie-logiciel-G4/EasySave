using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using EasySave.Exceptions;
using EasySave.Helpers;

namespace EasySave.Services;

public static class ExtensionService
{
    public static readonly ObservableCollection<string> EncryptedExtensions 
        = SettingsService.Settings.EncryptExtensions;
    
    /// <summary>
    ///   Add an extension to the list of encrypted extensions.
    /// </summary>
    /// <param name="extension"></param>
    /// <returns>
    ///     <c>extension</c> if the extension was added to the list, <c>null</c> otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static string? AddEncryptedExtension(string extension)
    {
        var formatedExtension = FormatExtension(extension);
        SettingsService.AddEncryptExtensions(formatedExtension);
        return formatedExtension;
    }

    /// <summary>
    ///     If the extension is not prefixed with a dot, it will be added with a dot.
    /// </summary>
    /// <param name="extension"></param>
    /// <returns>
    ///     <c>extension</c> extension well formated
    /// </returns>
    public static string FormatExtension(string extension)
    {
        var lowerExt = extension.ToLower();

        if (!lowerExt.StartsWith('.')) 
            lowerExt = lowerExt.Insert(0, ".");
        
        return lowerExt;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void RemoveEncryptedExtension(string extension)
    {
        SettingsService.RemoveEncryptExtensions(extension);
    }
}