namespace EasySave.Models;

public class Execution
{
    public event Action<string, bool>? Notifier;
    public event Action<int, int>? ProgressUpdated; 
    
    private int _currentProgress = 0;
    private int TotalSteps { get; set; }
    
    public void SetTotalSteps(int steps)
    {
        TotalSteps = steps;
    }
    
    public void SetError(string message)
    {
        Notify(message,true);
    }
    
    public void SetMessage(string message){
        Notify(message, false);
    }
    
    public void SetSuccess(string message)
    {
        Notify(message, false);
    }
    
    public void Notify(string message, bool isError)
    {
        Notifier?.Invoke(message, isError);
    }
    public void UpdateProgress(int step)
    {
        _currentProgress += step;
        ProgressUpdated?.Invoke(_currentProgress, TotalSteps);
    }
}