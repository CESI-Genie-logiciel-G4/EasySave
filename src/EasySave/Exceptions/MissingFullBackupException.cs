namespace EasySave.Exceptions;

public class MissingFullBackupException() : Exception("Last full backup folder is not available")
{
}