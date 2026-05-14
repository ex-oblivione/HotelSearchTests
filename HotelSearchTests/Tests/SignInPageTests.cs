using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using HotelSearchTests.PageObjects;

namespace HotelSearchTests
{
    public class SignInPageTests : HeaderFooterTestsBase
    {
        protected override string CurrentPageUrl => SignInUrl;
        protected override BasePage CreatePageObject() => new SignInPage(Driver);
        protected override void NavigateToInitialPage()
        {
            Driver.Navigate().GoToUrl(SignInUrl);
        }
        [Test]
        [Description("Проверка ввода данных в поле логина и пароля")]
        public void Test_LoginPasswordInput_DisplaysEnteredValue()
        {
            var signInPage = new SignInPage(Driver);
            string testLogin = "testuser@example.com";
            string testPassword = "********";

            signInPage.EnterLogin(testLogin);
            signInPage.EnterPassword(testPassword);

            string actualLoginValue = signInPage.GetLoginInputValue();
            string actualPasswordValue = signInPage.GetPasswordInputValue();
            Assert.Multiple(() =>
            {
                Assert.That(actualLoginValue, Is.EqualTo(testLogin),
                    "Значение в поле ввода логина некорректно.");

                Assert.That(actualPasswordValue, Is.EqualTo(testPassword),
                    "Значение в поле ввода пароля некорректно.");
            });
        }

        // Временный тест, пока на сайте не реализована полная авторизация
        [Test]
        public void LoginWithCreditsButton()
        {
            var signInPage = new SignInPage(Driver);
            string testLogin = "testuser@example.com";
            string testPassword = "********";

            signInPage.EnterLogin(testLogin);
            signInPage.EnterPassword(testPassword);

            signInPage.ClickLoginWithCreditsButton();

            Assert.That(Driver.Url, Does.Contain(SignInUrl));
        }

        [Test]
        public void ClickOnCreateAccountButton_ShouldNavigateToRegistrationPage()
        {
            var signInPage = new SignInPage(Driver);

            signInPage.ClickCreateAccountButton();

            string actualUrl = NormalizeUrl(Driver.Url);
            Assert.That(actualUrl, Does.Contain(RegistrationUrl));
        }
    }
}