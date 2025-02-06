namespace EasySave.Models;

public class Execution
{
    public event Action<int>? ProgressUpdated;
    private int _currentProgress = 0;
    private int _totalSteps;

    public Execution(int totalSteps)
    {
        _totalSteps = totalSteps > 0 ? totalSteps : 100;
    }

    public void UpdateProgress(int step)
    {
        _currentProgress += step;
        int progressPercentage = (_currentProgress * 100) / _totalSteps;
        ProgressUpdated?.Invoke(progressPercentage);
    }
}