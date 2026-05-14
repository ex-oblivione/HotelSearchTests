using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace HotelSearchTests.PageObjects
{
    public abstract class BasePage
    {
        // Общие переменные элементов на странице
        // Локаторы для кнопок в хедере
        protected readonly By _headerToxinLogoButton = By.CssSelector(".header .toxin-logo");
        protected readonly By _headerAboutUsButton = By.XPath("//a[@class='header-menu__link' and text()='О нас']");
        protected readonly By _headerServicesButton = By.XPath("//div[@class='header-menu__link' and text()='Услуги']");
        protected readonly By _headerServicesDropDownList = By.XPath("//div[text()='Услуги']/following-sibling::ul//li");
        protected readonly By _headerVacanciesButton = By.XPath("//a[@class='header-menu__link' and text()='Вакансии']");
        protected readonly By _headerNewsButton = By.XPath("//a[@class='header-menu__link' and text()='Новости']");
        protected readonly By _headerAgreementsButton = By.XPath("//div[@class='header-menu__link' and text()='Соглашения']");
        protected readonly By _headerAgreementsDropDownList = By.XPath("//div[text()='Соглашения']/following-sibling::ul//li");
        protected readonly By _loginButton = By.XPath("//a[@class='btn-gradient-border__text text-style-litle' and text()='ВОЙТИ']");
        protected readonly By _registerButton = By.XPath("//a[@class='btn-gradient__text text-style-litle' and text()='ЗАРЕГИСТРИРОВАТЬСЯ']");
        // Локаторы для элементов в футере
        protected readonly By _footerToxinLogoButton = By.CssSelector(".footer .toxin-logo");
        protected readonly By _footerAboutUsButton = By.XPath("//footer//a[@class='sitemap__sublist-link' and text()='О нас']");
        protected readonly By _footerNewsButton = By.XPath("//footer//a[@class='sitemap__sublist-link' and text()='Новости']");
        protected readonly By _footerSupportButton = By.XPath("//footer//a[@class='sitemap__sublist-link' and text()='Служба поддержки']");
        protected readonly By _footerServicesButton = By.XPath("//footer//a[@class='sitemap__sublist-link' and text()='Услуги']");
        protected readonly By _footerAboutServiceButton = By.XPath("//footer//a[@class='sitemap__sublist-link' and text()='О сервисе']");
        protected readonly By _footerOurTeamButton = By.XPath("//footer//a[@class='sitemap__sublist-link' and text()='Наша команда']");
        protected readonly By _footerVacanciesButton = By.XPath("//footer//a[@class='sitemap__sublist-link' and text()='Вакансии']");
        protected readonly By _footerInvestorsButton = By.XPath("//footer//a[@class='sitemap__sublist-link' and text()='Инвесторы']");
        protected readonly By _footerAgreementsButton = By.XPath("//footer//a[@class='sitemap__sublist-link' and text()='Соглашения']");
        protected readonly By _footerCommumitiesButton = By.XPath("//footer//a[@class='sitemap__sublist-link' and text()='Сообщества']");
        protected readonly By _footerContactUsButton = By.XPath("//footer//a[@class='sitemap__sublist-link' and text()='Связь с нами']");
        protected readonly By _footerSubscribeInput = By.CssSelector(".subscription-field__placeholder");

        protected IWebDriver _driver;
        protected WebDriverWait _wait;

        public BasePage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
        }

        // Ожидание кликабельности элемента
        protected IWebElement WaitForClickable(By locator)
        {
            return _wait.Until(d =>
            {
                var element = d.FindElement(locator);
                return (element.Displayed && element.Enabled) ? element : null;
            });
        }

        // Безопасный клик с ожиданием кликабельности
        protected void Click(By locator)
        {
            WaitForClickable(locator).Click();
        }

        // Ждем, пока атрибут элемента изменится и не будет равен старому значению
        protected void WaitForAttributeChanged(IWebElement element, string attribute, string oldValue)
        {
            _wait.Until(d => element.GetAttribute(attribute) != oldValue);
        }

        // Метод для проверки, что мы перешли на нужную страницу
        public bool IsRedirectedToDetails(string expectedPartUrl)
        {
            // Ожидание изменения URL внутри страницы
            return _wait.Until(d => d.Url.Contains(expectedPartUrl));
        }


        // Клики в хедере, общем для всех страниц
        public void ClickHeaderLogo() => Click(_headerToxinLogoButton);
        public void ClickHeaderAboutUsButton() => Click(_headerAboutUsButton);
        public void ClickHeaderVacanciesButton() => Click(_headerVacanciesButton);
        public void ClickHeaderNewsButton() => Click(_headerNewsButton);
        public void ClickLoginButton() => Click(_loginButton);
        public void ClickRegisterButton() => Click(_registerButton);

        public List<string> HoverHeaderServicesButton()
        {
            // Ждём саму кнопку "Услуги"
            IWebElement hoverable = WaitForClickable(_headerServicesButton);

            // Выполняем ховер без JS-скролла
            new Actions(_driver)
                .MoveToElement(hoverable)
                .Perform();

            // Ждём, когда элементы списка станут видимыми
            _wait.Until(d => d.FindElement(_headerServicesDropDownList).Displayed);

            var dropdownElements = _driver.FindElements(_headerServicesDropDownList);
            return dropdownElements
                .Select(el => el.Text.Trim())
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .ToList();
        }

        public List<string> HoverHeaderAgreementsButton()
        {
            // Ждём саму кнопку "Соглашения"
            IWebElement hoverable = WaitForClickable(_headerAgreementsButton);

            // Выполняем ховер
            new Actions(_driver)
                .MoveToElement(hoverable)
                .Perform();

            // Ждём, когда элементы списка станут видимыми
            _wait.Until(d => d.FindElement(_headerAgreementsDropDownList).Displayed);

            var dropdownElements = _driver.FindElements(_headerAgreementsDropDownList);
            return dropdownElements
                .Select(el => el.Text.Trim())
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .ToList();
        }

        // Клики в футере, общем для всех страниц
        public void ClickFooterLogo() => Click(_footerToxinLogoButton);
        public void ClickFooterAboutUsButton() => Click(_footerAboutUsButton);
        public void ClickFooterNewsButton() => Click(_footerNewsButton);
        public void ClickFooterSupportButton() => Click(_footerSupportButton);
        public void ClickFooterServicesButton() => Click(_footerServicesButton);
        public void ClickFooterAboutServiceButton() => Click(_footerAboutServiceButton);
        public void ClickFooterOurTeamButton() => Click(_footerOurTeamButton);
        public void ClickFooterVacanciesButton() => Click(_footerVacanciesButton);
        public void ClickFooterInvestorsButton() => Click(_footerInvestorsButton);
        public void ClickFooterAgreementsButton() => Click(_footerAgreementsButton);
        public void ClickFooterCommunitiesButton() => Click(_footerCommumitiesButton);
        public void ClickFooterContactUsButton() => Click(_footerContactUsButton);

        // Поле подписки
        public void EnterSubscriptionEmail(string email)
        {
            var input = _wait.Until(d => d.FindElement(_footerSubscribeInput));
            input.Clear();
            input.SendKeys(email);
        }

        public string GetSubscriptionInputValue()
        {
            var input = _wait.Until(d => d.FindElement(_footerSubscribeInput));
            return input.GetAttribute("value").Trim();
        }
    }
}