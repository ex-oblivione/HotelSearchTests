using HotelSearchTests.Components;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HotelSearchTests.PageObjects
{
    public class MainPage : BasePage
    {

        // Переменные элементов на странице
        // Элементы прибытия, выезда, календаря, подобрать номер
        private readonly CalendarComponent _calendar;
        private readonly By _checkInInput = By.CssSelector(".date-dropdown__container #start_one");
        private readonly By _checkOutInput = By.CssSelector(".date-dropdown__container #end_one");
        private readonly By _findRoomButton = By.XPath("//a//span[@class='btn-gradient__text text-style-litle' and text()='ПОДОБРАТЬ НОМЕР']");
        // Панель "Гости"
        private readonly GuestsComponent _guests;

        public MainPage(IWebDriver driver) : base(driver)
        {
            _calendar = new CalendarComponent(
            driver,
            calendarContainerLocator: By.XPath("//div[@class='datepicker -bottom-left- -from-bottom- active']"),
            applyButtonLocator: By.XPath("//span[@class='datepicker--button' and text()='Применить']"),
            clearButtonLocator: By.XPath("//span[@class='datepicker--button' and text()='Очистить']"),
            nextMonthButtonLocator: By.CssSelector(".datepicker--nav-action[data-action='next']"),
            currentDayLocator: By.CssSelector(".datepicker--cell-day.-current-"),
            availableDaysLocator: By.CssSelector(".datepicker--cell-day:not(.-other-month-):not(.-disabled-)"),
            firstDayOfMonthLocator: By.XPath("//div[contains(@class, 'datepicker--cell-day') and @data-date='1' and not(contains(@class, '-other-month-'))]")
            );
            _guests = new GuestsComponent(driver);
        }

        // Работа с календарём
        public void OpenCalendar() => _calendar.Open(_checkInInput);
        public void SelectCurrentDay() => _calendar.SelectCurrentDay();
        public DateTime SelectFirstDayOfNextMonth() => _calendar.SelectFirstDayOfNextMonth();
        public void ClickApply() => _calendar.ClickApply();
        public void ClickClear() => _calendar.ClickClear();
        public string GetCheckInValue() => _calendar.GetInputValue(_checkInInput);
        public string GetCheckOutValue() => _calendar.GetInputValue(_checkOutInput);

        public string GetCheckInPlaceholder()
        {
            var element = _wait.Until(d => d.FindElement(_checkInInput));
            return element.GetAttribute("placeholder")?.Trim() ?? string.Empty;
        }

        public string GetCheckOutPlaceholder()
        {
            var element = _wait.Until(d => d.FindElement(_checkOutInput));
            return element.GetAttribute("placeholder")?.Trim() ?? string.Empty;
        }

        // Блок "Гости"
        public void OpenGuestsDropdown() => _guests.Open();
        public void AddAdult() => _guests.AddAdult();
        public void AddChild() => _guests.AddChild();
        public void AddInfant() => _guests.AddInfant();
        public void RemoveAdult() => _guests.RemoveAdult();
        public void RemoveChild() => _guests.RemoveChild();
        public void RemoveInfant() => _guests.RemoveInfant();
        public void ClickApplyGuests() => _guests.ClickApply();
        public void ClickClearGuests() => _guests.ClickClear();

        public string GetGuestsDropdownText() => _guests.GetText();
        public string GetAdultsCount() => _guests.GetAdultsCount();
        public string GetChildrenCount() => _guests.GetChildrenCount();
        public string GetInfantsCount() => _guests.GetInfantsCount();
        public void ResetGuestsToZero() => _guests.ResetToZero();
        public bool IsDropdownOpen() => _guests.IsOpen();
        public void ClickFindRoomButton() => Click(_findRoomButton);
    }
}