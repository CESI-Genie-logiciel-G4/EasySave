using EasySave.Utils;

namespace EasySave.Tests.Utils;

public class MenuItemTests
{
    [Fact] 
    public void MenuItem_WhenCreatedWithAction_ShouldHaveAction()
    {
        var action = new Action(() => { });
        const string title = "Title";
        
        var menuItem = new MenuItem(title, action);
        
        Assert.Equal(action, menuItem.Action);
    }
}