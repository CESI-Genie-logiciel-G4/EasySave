namespace EasySave.Helpers;

public static class FileHelper
{
    public static void Copy(string sourceFile, string destinationFile)
    {
        CreateParentDirectory(destinationFile);
        File.Copy(sourceFile, destinationFile, true);
    }
    public static void CreateAndWrite(string path, string content)
    {
        CreateParentDirectory(path);
        File.WriteAllText(path, content);   
    }
    
    public static void CreateFile(string path)
    {
        CreateParentDirectory(path);
        File.Create(path).Close();
    }
    
    public static void CreateParentDirectory(string path)
    {
        var parentDir = Path.GetDirectoryName(path);
        if (parentDir is null)
        {
            throw new Exception("Parent directory is null");
        }
        Directory.CreateDirectory(parentDir);
    }

    public static string GetMirrorFilePath(string sourceFolder, string sourceFile, string mirrorFolder)
    {
        var relativePath = Path.GetRelativePath(sourceFolder, sourceFile);
        return Path.Combine(mirrorFolder, relativePath);
    }

    public static long GetFileSize(string file)
    {
        var fileSize = new FileInfo(file).Length;
        return fileSize;
    }
}