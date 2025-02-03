namespace EasySave.Tests;
using EasySave;

public class CalculatorTest
{
    [Fact]
    public void AdditionTest()
    {
        Assert.Equal(4, Calculator.Add(2, 2));
    }
    
    [Fact]
    public void SubtractionTest()
    {
        Assert.Equal(0, Calculator.Subtract(2, 2));
    }
}