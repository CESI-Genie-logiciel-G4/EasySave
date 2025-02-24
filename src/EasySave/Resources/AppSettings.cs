using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using EasySave.Utils;
using Logger.Transporters;

namespace EasySave.Resources;

public partial class AppSettings : ObservableObject
{
    public ObservableCollection<String> PriorityExtensions { get; set; } = [];
    public String Language { get; set; } = "en";
    [ObservableProperty]
    public ObservableCollection<TransporterItem> _logTransporters  =
    [
        new("Console", new ConsoleTransporter(), false),
        new("XML", new FileXmlTransporter("./.easysave/logs/")),
        new("JSON", new FileJsonTransporter("./.easysave/logs/"),false)
    ];
}