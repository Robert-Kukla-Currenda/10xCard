using System.Threading.Tasks;
using Microsoft.Playwright;

namespace TenXCards.E2E.Tests.PageObjects;

/// <summary>
/// Base page object with common functionality for all pages
/// </summary>
public abstract class BasePage
{
    protected readonly IPage Page;

    protected BasePage(IPage page)
    {
        Page = page;
    }

    public abstract Task<bool> IsPageLoadedAsync();
    
    /// <summary>
    /// Takes a screenshot of the current page for debugging or visual regression testing
    /// </summary>
    public async Task TakeScreenshotAsync(string name)
    {
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = $"./Screenshots/{name}.png",
            FullPage = true
        });
    }
}

/// <summary>
/// Login page object
/// </summary>
public class LoginPage : BasePage
{
    public LoginPage(IPage page) : base(page)
    {
    }

    // Locators
    private ILocator EmailInput => Page.Locator("input[type='email']");
    private ILocator PasswordInput => Page.Locator("input[type='password']");
    private ILocator LoginButton => Page.Locator("button[type='submit']");
    private ILocator ErrorMessage => Page.Locator(".mud-snackbar");

    public override async Task<bool> IsPageLoadedAsync()
    {
        var emailVisible = await EmailInput.IsVisibleAsync();
        var passwordVisible = await PasswordInput.IsVisibleAsync();
        var loginButtonVisible = await LoginButton.IsVisibleAsync();
        return emailVisible && passwordVisible && loginButtonVisible;
    }

    public async Task LoginAsync(string email, string password)
    {
        await EmailInput.FillAsync(email);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    public async Task<string> GetErrorMessageAsync()
    {
        var errorMessageVisible = await ErrorMessage.IsVisibleAsync();
        if (errorMessageVisible)
        {
            var snackbarText = await ErrorMessage.TextContentAsync();
            return snackbarText ?? string.Empty;
        }
        
        return string.Empty;
    }
}

/// <summary>
/// Signed in home page object
/// </summary>
public class SignedInHomePage : BasePage
{
    public SignedInHomePage(IPage page) : base(page)
    {
    }

    // Locators
    private ILocator SignedInMenu => Page.Locator("div.signedin");    
    
    public override async Task<bool> IsPageLoadedAsync()
    {
        var isSignedInMenu = await SignedInMenu.IsVisibleAsync();
        return isSignedInMenu;
    }
}
