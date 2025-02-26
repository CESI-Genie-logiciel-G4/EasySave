using CommunityToolkit.Mvvm.ComponentModel;
using Logger.Transporters;

namespace EasySave.Utils;

public partial class TransporterItem(string title, Transporter transporter, bool isEnabled = true): ObservableObject
{
    public string Title { get; } = title;
    public Transporter Transporter { get; } = transporter;
    
    [ObservableProperty]
    private bool _isEnabled = isEnabled;
}