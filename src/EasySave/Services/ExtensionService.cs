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
    
    public static readonly ObservableCollection<string> PriorityExtensions
        = SettingsService.Settings.PriorityExtensions;

    public enum ExtensionType
    {
        Encrypted,
        Priority,
    }

    /// <summary>
    ///   Add an extension to the list of extensions type in settings.
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="type"> Refer to an enum with each extensions list type </param>
    /// <returns>
    ///     <c>extension</c> If the extension was added to the list, <c>null</c> otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static string? AddExtension(string extension, ExtensionType type)
    {
        var formatedExtension = FormatExtension(extension);
        
        Action<string> addMethod = type switch
        {
            ExtensionType.Encrypted => SettingsService.AddEncryptExtensions,
            ExtensionType.Priority => SettingsService.AddPriorityExtensions,
            _ => throw new ArgumentException($"Extension type method add undefined: {type}")
        };
        addMethod(formatedExtension);
        
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

    /// <summary>
    ///   Remove an extension to the list of extensions type in settings.
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="type"> Refer to an enum with each extensions list type </param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void RemoveExtension(string extension, ExtensionType type)
    {
        var formatedExtension = FormatExtension(extension);
        
        Action<string> removeMethod = type switch
        {
            ExtensionType.Encrypted => SettingsService.RemoveEncryptExtensions,
            ExtensionType.Priority => SettingsService.RemovePriorityExtensions,
            _ => throw new ArgumentException($"Extension type method remove undefined: {type}")
        };
        
        removeMethod(formatedExtension);
    }

    public static string GetExtension(ExtensionType type, int index)
    {
        return type switch
        {
            ExtensionType.Encrypted => EncryptedExtensions[index],
            ExtensionType.Priority => PriorityExtensions[index],
            _ => throw new ArgumentException($"Extension type undefined: {type}")
        };
    }
}