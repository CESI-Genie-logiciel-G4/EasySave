using Logger.Transporters;

namespace EasySave.Utils;

public class TransporterItem(string title, Transporter transporter, bool isEnabled = true)
{
    public string Title { get; } = title;
    public Transporter Transporter { get; } = transporter;
    public bool IsEnabled { get; set; } = isEnabled;
}