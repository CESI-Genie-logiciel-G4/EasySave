using System;
using System.Globalization;
using System.Reflection;
using Xunit.v3;

/// <summary>
/// An attribute class for setting the culture and UI culture for a test method.
/// This class is based on the official xUnit documentation:
/// https://github.com/xunit/samples.xunit/tree/main/v3/UseCultureExample
/// </summary>
/// <param name="culture">The culture to set.</param>
/// <param name="uiCulture">The UI culture to set.</param>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class UseCultureAttribute(string culture, string uiCulture) : BeforeAfterTestAttribute
{
    readonly Lazy<CultureInfo> culture = new(() => new CultureInfo(culture, false));
    readonly Lazy<CultureInfo> uiCulture = new(() => new CultureInfo(uiCulture, false));

    CultureInfo? originalCulture;
    CultureInfo? originalUICulture;

    /// <summary>
    /// Replaces the culture and UI culture of the current thread with
    /// <paramref name="culture" />
    /// </summary>
    /// <param name="culture">The name of the culture.</param>
    /// <remarks>
    /// This constructor overload uses <paramref name="culture" /> for both
    /// <see cref="Culture" /> and <see cref="UICulture" />.
    /// </remarks>
    public UseCultureAttribute(string culture)
        : this(culture, culture) { }

    /// <summary>
    /// Gets the culture.
    /// </summary>
    public CultureInfo Culture => culture.Value;

    /// <summary>
    /// Gets the UI culture.
    /// </summary>
    public CultureInfo UICulture => uiCulture.Value;

    /// <summary>
    /// Stores the current <see cref="Thread.CurrentPrincipal" />
    /// <see cref="CultureInfo.CurrentCulture" /> and <see cref="CultureInfo.CurrentUICulture" />
    /// and replaces them with the new cultures defined in the constructor.
    /// </summary>
    /// <param name="methodUnderTest">The method under test</param>
    public override void Before(MethodInfo methodUnderTest, IXunitTest test)
    {
        originalCulture = Thread.CurrentThread.CurrentCulture;
        originalUICulture = Thread.CurrentThread.CurrentUICulture;

        Thread.CurrentThread.CurrentCulture = Culture;
        Thread.CurrentThread.CurrentUICulture = UICulture;

        CultureInfo.CurrentCulture.ClearCachedData();
        CultureInfo.CurrentUICulture.ClearCachedData();
    }

    /// <summary>
    /// Restores the original <see cref="CultureInfo.CurrentCulture" /> and
    /// <see cref="CultureInfo.CurrentUICulture" /> to <see cref="Thread.CurrentPrincipal" />
    /// </summary>
    /// <param name="methodUnderTest">The method under test</param>
    public override void After(MethodInfo methodUnderTest, IXunitTest test)
    {
        if (originalCulture is not null)
            Thread.CurrentThread.CurrentCulture = originalCulture;
        if (originalUICulture is not null)
            Thread.CurrentThread.CurrentUICulture = originalUICulture;

        CultureInfo.CurrentCulture.ClearCachedData();
        CultureInfo.CurrentUICulture.ClearCachedData();
    }
}