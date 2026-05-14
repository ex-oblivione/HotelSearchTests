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
    public class SignInPage : BasePage
    {
        // Переменные элементов на странице
        // Локаторы для элементов в блоке Войти
        private readonly By _loginInput = By.CssSelector(".login input[placeholder='Email']");
        private readonly By _passwordInput = By.CssSelector(".login input[placeholder='Пароль']");
        private readonly By _loginWithCreditsButton = By.CssSelector(".login__btn .btn-gradient");
        private readonly By _createAccountButton = By.CssSelector(".login__registration .btn-gradient-border__text");

        public SignInPage(IWebDriver driver) : base(driver) { }


        // Поля аутентификации
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

        public void ClickLoginWithCreditsButton() => Click(_loginWithCreditsButton);

        public void ClickCreateAccountButton() => Click(_createAccountButton);

    }
}