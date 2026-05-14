using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using System;

namespace HotelSearchTests
{
    public abstract class BaseTest
    {
        protected IWebDriver Driver { get; private set; }

        protected const string BaseUrl = "https://ex-oblivione.github.io/2nd-task/dist/index.html";
        protected const string SignInUrl = "https://ex-oblivione.github.io/2nd-task/dist/sign-in.html";
        protected const string RegistrationUrl = "https://ex-oblivione.github.io/2nd-task/dist/registration.html";
        protected const string SearchRoomUrl = "https://ex-oblivione.github.io/2nd-task/dist/search-room.html";

        protected virtual IWebDriver CreateDriver() => new ChromeDriver();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Driver = CreateDriver();
            Driver.Manage().Window.Maximize();
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (Driver != null)
            {
                Driver.Quit();
                Driver.Dispose();
            }
        }

        [SetUp]
        public void TestSetUp()
        {
            // Закрываем возможные алерты перед переходом
            try { Driver.SwitchTo().Alert().Accept(); } catch { }
            try { Driver.SwitchTo().Alert().Dismiss(); } catch { }

            NavigateToInitialPage();
            
            Driver.Manage().Cookies.DeleteAllCookies();

            try { ((IJavaScriptExecutor)Driver).ExecuteScript("window.localStorage.clear();"); } catch { }
            try { ((IJavaScriptExecutor)Driver).ExecuteScript("window.sessionStorage.clear();"); } catch { }
        }

        protected virtual void NavigateToInitialPage()
        {
            Driver.Navigate().GoToUrl(BaseUrl);
        }

        // Нормализация URL
        protected string NormalizeUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            var uriBuilder = new UriBuilder(url);
            // Убираем фрагмент (#)
            uriBuilder.Fragment = string.Empty;
            // При необходимости можно убрать и параметры запроса: uriBuilder.Query = string.Empty;
            return uriBuilder.Uri.ToString();
        }
    }
}