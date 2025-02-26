using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using EasySave.Utils;
using Logger.Transporters;

namespace EasySave.Resources;

public partial class AppSettings : ObservableObject
{
    public ObservableCollection<String> PriorityExtensions { get; set; } = [];
    
    public ObservableCollection<String> EncryptExtensions { get; set; } = [];
    
    public ObservableCollection<String> PriorityProcessNames { get; set; } = [];
    
    public String Language { get; set; } = "en";
    
    public long MaxFileSizeKb { get; set; } = 2_000_000; // 2 GB en kB
    
    [ObservableProperty]
    private ObservableCollection<TransporterItem> _logTransporters  =
    [
        new("Console", new ConsoleTransporter(), false),
        new("XML", new FileXmlTransporter("./.easysave/logs/")),
        new("JSON", new FileJsonTransporter("./.easysave/logs/"),false)
    ];
}