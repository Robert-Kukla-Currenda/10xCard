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

    public async Task<string> GetSnackbarMessageAsync()
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
    private ILocator LogoutMenuItem => Page.Locator("text=Wyloguj się");
    private ILocator SignedInMenuButton => Page.Locator("div.signedin");
    
    public override async Task<bool> IsPageLoadedAsync()
    {
        var isSignedInMenu = await SignedInMenu.IsVisibleAsync();
        return isSignedInMenu;
    }
    
    public async Task LogoutAsync()
    {
        await SignedInMenuButton.ClickAsync();
        await LogoutMenuItem.ClickAsync();
    }
}


/// <summary>
/// Signed out home page object
/// </summary>
public class SignedOutHomePage : BasePage
{
    public SignedOutHomePage(IPage page) : base(page)
    {
    }

    // Locators
    private ILocator WelcomeText => Page.Locator("text=Witaj w TenXCards!");
    private ILocator SnackbarMessage => Page.Locator(".mud-snackbar");

    public override async Task<bool> IsPageLoadedAsync()
    {
        var isSignedOutWelcomeText = await WelcomeText.IsVisibleAsync();
        return isSignedOutWelcomeText;
    }

    public async Task<string> GetSnackbarMessageAsync()
    {
        // After logout, the user should be redirected to the home page
        // and a snackbar message should be displayed
        var snackbarVisibile = await SnackbarMessage.IsVisibleAsync();
        if (snackbarVisibile)
        {
            var snackbarText = await SnackbarMessage.TextContentAsync();
            return snackbarText ?? string.Empty;
        }

        return string.Empty;
    }
}