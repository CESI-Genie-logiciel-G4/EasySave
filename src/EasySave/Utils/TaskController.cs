using CommunityToolkit.Mvvm.ComponentModel;

namespace EasySave.Utils;

public sealed partial class TaskController() : ObservableObject, IDisposable
{
    private CancellationTokenSource _cts = new();
    private readonly ManualResetEventSlim _pauseEvent = new(true);
    private Task _task = null!;
    
    [ObservableProperty]
    private bool _isPaused;
    
    [ObservableProperty]
    private bool _isCanceled;
    
    [ObservableProperty]
    private bool _isCompleted;
    
    public async Task Start(Func<CancellationToken, ManualResetEventSlim, Task> work)
    {
        _cts = new CancellationTokenSource();
        _pauseEvent.Set();
        IsPaused = false;
        
        _task = Task.Run(() => work(_cts.Token, _pauseEvent), _cts.Token);
        await _task;
        IsCompleted = true;
    }
    
    public void Pause()
    {
        if (IsPaused)
            return;
        
        _pauseEvent.Reset();
        IsPaused = true;
    }
    
    public void Resume()
    {
        if (!IsPaused)
            return;
        
        _pauseEvent.Set();
        IsPaused = false;
    }
    
    public void Cancel()
    {
        if (IsCanceled)
            return;
        
        _cts.Cancel();
        IsCanceled = true;
    }
    
    public void Dispose()
    {
        _cts.Dispose();
        _pauseEvent.Dispose();
        _task.Dispose();
    }
}