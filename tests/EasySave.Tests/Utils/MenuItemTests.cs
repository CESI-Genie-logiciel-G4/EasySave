using EasySave.Utils;

namespace EasySave.Tests.Utils;

public class MenuItemTests
{
    [Fact]
    public void Constructor_WithTitleAndFuncAction_ShouldSetProperties()
    {
        // Arrange
        string title = "Test Title";
        Func<Task> action = () => Task.CompletedTask;

        // Act
        var menuItem = new MenuItem(title, action);

        // Assert
        Assert.Equal(title, menuItem.Title);
        Assert.Equal(action, menuItem.Action);
    }

    [Fact]
    public void Constructor_WithTitleAndAction_ShouldSetProperties()
    {
        // Arrange
        string title = "Test Title";
        Action action = () => { };

        // Act
        var menuItem = new MenuItem(title, action);

        // Assert
        Assert.Equal(title, menuItem.Title);
        Assert.NotNull(menuItem.Action);
    }

    [Fact]
    public async Task Action_ShouldExecuteProvidedAction()
    {
        // Arrange
        var actionExecuted = false;
        var action = () => { actionExecuted = true; };
        var menuItem = new MenuItem("Test Title", action);

        // Act
        await menuItem.Action();

        // Assert
        Assert.True(actionExecuted);
    }
}