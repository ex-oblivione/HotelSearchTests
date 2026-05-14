using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HotelSearchTests.PageObjects
{
    internal class RegistrationPage : BasePage
    {
        // Переменные элементов на странице
        // Локаторы для элементов в блоке Регистрация
        private readonly By _nameInput = By.CssSelector(".registration input[placeholder='Имя']");
        private readonly By _surnameInput = By.CssSelector(".registration input[placeholder='Фамилия']");
        private readonly By _maleLabel = By.XPath("//label[contains(., 'Мужчина')]");
        private readonly By _femaleLabel = By.XPath("//label[contains(., 'Женщина')]");
        private readonly By _maleInput = By.XPath("//label[contains(., 'Мужчина')]/input");
        private readonly By _femaleInput = By.XPath("//label[contains(., 'Женщина')]/input");
        // Локаторы календаря
        private readonly By _dateInput = By.CssSelector(".date-dropdown_registration");
        private readonly By _navTitle = By.CssSelector(".datepicker--nav-title");
        private readonly By _prevButton = By.CssSelector(".datepicker--nav-action[data-action='prev']");
        private readonly By _applyButton = By.CssSelector(".datepicker--button[data-action='today']");
        private By GetYearCell(string year) => By.XPath($"//div[contains(@class, 'datepicker--cell-year') and @data-year='{year}']");
        private By GetMonthCell(string monthIndex) => By.XPath($"//div[contains(@class, 'datepicker--cell-month') and @data-month='{monthIndex}']");
        private By GetDayCell(string day, string monthIndex, string year) =>
            By.XPath($"//div[contains(@class, 'datepicker--cell-day') and @data-date='{day}' and @data-month='{monthIndex}' and @data-year='{year}']");
        private readonly By _clearButton = By.CssSelector(".datepicker--button[data-action='clear']");
        private readonly By _loginInput = By.CssSelector(".registration__name-form input[placeholder='Email']");
        private readonly By _passwordInput = By.CssSelector(".registration__name-form input[placeholder='Пароль']");
        private readonly By _specialOfferLabel = By.CssSelector(".registration__special-offer-toggle .toggle");
        private readonly By _specialOfferInput = By.CssSelector(".registration__special-offer-toggle .toggle__input");
        private readonly By _goToPaymentButton = By.CssSelector(".registration .registration__pay-btn");
        private readonly By _loginRegistrationButton = By.CssSelector(".registration .btn-gradient-border");

        public RegistrationPage(IWebDriver driver) : base(driver) { }

        // Поля регистрации
        public void EnterName(string name)
        {
            var input = _wait.Until(d => d.FindElement(_nameInput));
            input.Clear();
            input.SendKeys(name);
        }

        public string GetNameInputValue()
        {
            var input = _wait.Until(d => d.FindElement(_nameInput));
            return input.GetAttribute("value").Trim();
        }

        public void EnterSurname(string name)
        {
            var input = _wait.Until(d => d.FindElement(_surnameInput));
            input.Clear();
            input.SendKeys(name);
        }

        public string GetSurnameInputValue()
        {
            var input = _wait.Until(d => d.FindElement(_surnameInput));
            return input.GetAttribute("value").Trim();
        }

        public void SelectMale() => Click(_maleLabel);
        public void SelectFemale() => Click(_femaleLabel);

        public bool IsMaleSelected() => _driver.FindElement(_maleInput).Selected;
        public bool IsFemaleSelected() => _driver.FindElement(_femaleInput).Selected;

        // Методы календаря (имеют отличия от методов для других страниц, т.к. выбирается одна дата в прошлом)
        public void OpenCalendar() => Click(_dateInput);
        public void ClickNavTitle() => Click(_navTitle);
        public void ClickPrev() => Click(_prevButton);
        public void ClickApply() => Click(_applyButton);
        public string GetDateInputValue() => _driver.FindElement(_dateInput).GetAttribute("value");

        public void SelectBirthDateViaCalendar(string day, string monthIndex, string year, int clicksBack)
        {
            OpenCalendar();

            // Переходим в режим выбора месяцев, затем в режим выбора годов
            ClickNavTitle();
            ClickNavTitle();

            // Листаем назад до нужного десятилетия
            for (int i = 0; i < clicksBack; i++)
            {
                ClickPrev();
            }

            // Кликаем по конкретным элементам
            _driver.FindElement(GetYearCell(year)).Click();
            _driver.FindElement(GetMonthCell(monthIndex)).Click();
            _driver.FindElement(GetDayCell(day, monthIndex, year)).Click();

            ClickApply();
        }

        public void ClickClear() => _driver.FindElement(_clearButton).Click();

        public bool IsDateInputEmpty() => string.IsNullOrEmpty(GetDateInputValue());

        // Методы аутентификации
        public void EnterLogin(string login)
        {
            var input = _wait.Until(d => d.FindElement(_loginInput));
            input.Clear();
            input.SendKeys(login);
        }

        public string GetLoginInputValue()
        {
            var input = _wait.Until(d => d.FindElement(_loginInput));
            return input.GetAttribute("value").Trim();
        }

        public void EnterPassword(string password)
        {
            var input = _wait.Until(d => d.FindElement(_passwordInput));
            input.Clear();
            input.SendKeys(password);
        }

        // Временно такая проверка, пока не реализовано маскирование пароля на сайте
        public string GetPasswordInputValue()
        {
            var input = _wait.Until(d => d.FindElement(_passwordInput));
            return input.GetAttribute("value").Trim();
        }

        public void ToggleSpecialOffer() => _driver.FindElement(_specialOfferLabel).Click();
        public bool IsSpecialOfferSelected() => _driver.FindElement(_specialOfferInput).Selected;

        public void ClickGoToPaymentButton() => Click(_goToPaymentButton);

        public void ClickLoginRegistrationButton() => Click(_loginRegistrationButton);

    }
}
