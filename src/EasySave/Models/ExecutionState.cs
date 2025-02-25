namespace EasySave.Models;

public enum ExecutionState
{
    Pending,
    Running,
    Completed,
    Paused,
    Canceled,
    Failed,
    Blocked,
}