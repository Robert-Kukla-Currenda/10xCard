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
            Path = $"./Screenshots/{name}_{DateTime.Now:yyyyMMdd_HHmmss}.png",
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
    private ILocator ErrorMessage => Page.Locator(".error-message");

    public override async Task<bool> IsPageLoadedAsync()
    {
        return await EmailInput.IsVisibleAsync() && await PasswordInput.IsVisibleAsync();
    }

    public async Task LoginAsync(string email, string password)
    {
        await EmailInput.FillAsync(email);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    public async Task<string> GetErrorMessageAsync()
    {
        if (await ErrorMessage.IsVisibleAsync())
        {
            return await ErrorMessage.TextContentAsync();
        }
        
        return string.Empty;
    }
}

/// <summary>
/// Dashboard page object
/// </summary>
public class DashboardPage : BasePage
{
    public DashboardPage(IPage page) : base(page)
    {
    }

    // Locators
    private ILocator WelcomeMessage => Page.Locator("h1.welcome-message");
    private ILocator CardsList => Page.Locator(".card-list");
    
    public override async Task<bool> IsPageLoadedAsync()
    {
        return await WelcomeMessage.IsVisibleAsync() && await CardsList.IsVisibleAsync();
    }

    public async Task<int> GetCardsCountAsync()
    {
        return await Page.Locator(".card-item").CountAsync();
    }
}
