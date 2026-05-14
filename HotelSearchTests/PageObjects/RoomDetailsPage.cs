using HotelSearchTests.Components;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace HotelSearchTests.PageObjects
{
    public class RoomDetailsPage : BasePage
    {
        // Локатор для всех контейнеров комментариев
        private readonly By _commentItems = By.CssSelector(".room-describe__section .room-describe__comment-item");

        // Локаторы внутри конкретного комментария (ищем относительно элемента списка)
        private readonly By _likeInput = By.CssSelector("input.like__input");
        private readonly By _likeClickableArea = By.CssSelector("span.like__btn");
        private readonly By _likeCounter = By.CssSelector("span.like__counter");
        // Локаторы элементов прибытия, выезда, календаря, подобрать номер
        private readonly CalendarComponent _calendar;
        private readonly By _checkInInput = By.CssSelector(".date-dropdown__container #start_one");
        private readonly By _checkOutInput = By.CssSelector(".date-dropdown__container #end_one");
        private readonly By _bookRoomButton = By.XPath("//a//span[@class='btn-gradient__text text-style-litle' and text()='ЗАБРОНИРОВАТЬ']");


        public RoomDetailsPage(IWebDriver driver) : base(driver) {
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
        }

        // Получаем список всех комментариев
        private IWebElement GetCommentByIndex(int index)
        {
            var comments = _driver.FindElements(_commentItems);
            return comments[index];
        }

        public bool IsLikeSet(int commentIndex = 0)
        {
            var comment = GetCommentByIndex(commentIndex);
            return comment.FindElement(_likeInput).Selected;
        }

        public void ClickLike(int commentIndex = 0)
        {
            var comment = GetCommentByIndex(commentIndex);
            // Кликаем по области span, так как input часто скрыт визуально
            comment.FindElement(_likeClickableArea).Click();
        }

        public int GetLikeCount(int commentIndex = 0)
        {
            var comment = GetCommentByIndex(commentIndex);
            string countText = comment.FindElement(_likeCounter).Text.Trim();
            return int.Parse(countText);
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

        public void ClickBookRoomButton() => Click(_bookRoomButton);
    }
}
