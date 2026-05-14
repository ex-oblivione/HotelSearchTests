using HotelSearchTests.Components;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelSearchTests.PageObjects
{
    public class SearchRoomPage : BasePage
    {
        // Элементы для фильтра дат
        private readonly CalendarComponent _calendar;
        private readonly By _filterDateInput = By.CssSelector(".filter-date-dropdown__input");
        // Панель "Гости"
        private readonly GuestsComponent _guests;

        // Локаторы для ползунка цены
        private readonly By _priceFromText = By.CssSelector(".search-room-page__range-slider .range-slider__text-price-from");
        private readonly By _priceToText = By.CssSelector(".search-room-page__range-slider .range-slider__text-price-to");
        private readonly By _sliderHandleFrom = By.CssSelector(".search-room-page__range-slider .irs-handle.from");
        private readonly By _sliderHandleTo = By.CssSelector(".search-room-page__range-slider .irs-handle.to");
        private readonly By _sliderLine = By.CssSelector(".search-room-page__range-slider .irs-line");
        // Локаторы для чекбоксов ПРАВИЛА ДОМА
        private readonly By _rulesCheckboxes = By.CssSelector(".search-room-page__rules .checkbox");
        private readonly By _rulesInputs = By.CssSelector(".search-room-page__rules .checkbox__input");
        // Локаторы для УДОБСТВА НОМЕРА
        private readonly By _roomsDropdownBtn = By.CssSelector(".dropdown-room .dropdown-room__btn");
        private readonly By _roomsPlaceholder = By.CssSelector(".dropdown-room .dropdown-room__placeholder");
        private readonly By _roomsContent = By.CssSelector(".dropdown-room .dropdown-room__content");
        private readonly By _roomsRows = By.CssSelector(".dropdown-room .dropdown-room__content-item");
        // Локаторы для Дополнительных удобств
        private readonly By _expandableAmenitiesBtn = By.CssSelector(".expand-checkbox-list .expand-checkbox-list__btn");
        private readonly By _expandableAmenitiesContent = By.CssSelector(".expand-checkbox-list .expand-checkbox-list__content");
        private readonly By _additionalCheckboxes = By.CssSelector(".expand-checkbox-list .expand-checkbox-list__content .checkbox");
        private readonly By _additionalInputs = By.CssSelector(".expand-checkbox-list .expand-checkbox-list__content .checkbox__input");
        // Локаторы для карточек номеров
        private readonly By _roomCards = By.CssSelector(".search-room-page__rooms-list .room-slider");
        private readonly By _roomDescriptionLink = By.CssSelector(".search-room-page__rooms-list .room-slider__description");
        private readonly By _commentsCount = By.CssSelector(".search-room-page__rooms-list .room-slider__comments-num");

        public SearchRoomPage(IWebDriver driver) : base(driver) 
        {
            _calendar = new CalendarComponent(
             driver,
             calendarContainerLocator: By.CssSelector(".datepicker.-bottom-left-.-from-bottom-.active"),
             applyButtonLocator: By.XPath("//span[@class='datepicker--button' and text()='Применить']"),
             clearButtonLocator: By.XPath("//span[@class='datepicker--button' and text()='Очистить']"),
             nextMonthButtonLocator: By.CssSelector(".datepicker--nav-action[data-action='next']"),
             currentDayLocator: By.CssSelector(".datepicker--cell-day.-current-"),
             availableDaysLocator: By.CssSelector(".datepicker--cell-day:not(.-other-month-):not(.-disabled-)"),
             firstDayOfMonthLocator: By.XPath("//div[contains(@class, 'datepicker--cell-day') and @data-date='1' and not(contains(@class, '-other-month-'))]")
            );
            _guests = new GuestsComponent(driver);

        }

        public void OpenCalendar() => _calendar.Open(_filterDateInput);
        public void SelectCurrentDay() => _calendar.SelectCurrentDay();
        public DateTime SelectFirstDayOfNextMonth() => _calendar.SelectFirstDayOfNextMonth();
        public void ClickApply() => _calendar.ClickApply();
        public void ClickClear() => _calendar.ClickClear();


        // Методы получения значений полей
        public string GetFilterDateInputValue()
        {
            var element = _wait.Until(d => d.FindElement(_filterDateInput));
            string val = element.GetAttribute("value");
            return val != null ? val.Trim() : string.Empty;
        }

        public string GetFilterDateInputPlaceholder()
        {
            var element = _wait.Until(d => d.FindElement(_filterDateInput));
            return element.GetAttribute("placeholder")?.Trim() ?? string.Empty;
        }

        public string GetFilterDateValue()
        {
            var element = _wait.Until(d => d.FindElement(_filterDateInput));
            return element.GetAttribute("value").Trim();
        }

        public string GetShortMonth(DateTime date)
        {
            string month = date.ToString("MMM", new System.Globalization.CultureInfo("ru-RU")).Replace(".", "");

            if (month.Length > 3)
            {
                month = month.Substring(0, 3);
            }

            return char.ToUpper(month[0]) + month.Substring(1).ToLower();
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


        // Методы для ползунка цены
        public string GetPriceFromText() => _wait.Until(d => d.FindElement(_priceFromText)).Text.Trim();
        public string GetPriceToText() => _wait.Until(d => d.FindElement(_priceToText)).Text.Trim();
        private int GetSliderWidth() => _wait.Until(d => d.FindElement(_sliderLine)).Size.Width;

        public void DragSliderToEdge(bool toMax)
        {
            int width = GetSliderWidth();
            // Двигаем либо левый ползунок (From) в 0, либо правый (To) на полную ширину
            var handleLocator = toMax ? _sliderHandleTo : _sliderHandleFrom;
            var handle = _wait.Until(d => d.FindElement(handleLocator));

            int offset = toMax ? width : -width;

            new Actions(_driver)
                .ClickAndHold(handle)
                .MoveByOffset(offset, 0)
                .Release()
                .Perform();
        }

        // Методы для чекбоксов ПРАВИЛА ДОМА
        public void SetAllRules(bool state)
        {
            var checkboxes = _driver.FindElements(_rulesCheckboxes);
            var inputs = _driver.FindElements(_rulesInputs);

            for (int i = 0; i < checkboxes.Count; i++)
            {
                if (inputs[i].Selected != state)
                {
                    checkboxes[i].Click();
                }
            }
        }

        public List<bool> GetRulesStates()
        {
            return _driver.FindElements(_rulesInputs)
                          .Select(i => i.Selected)
                          .ToList();
        }

        // Методы для дропдауна Удобства номера
        public void OpenRoomsDropdown()
        {
            if (!_driver.FindElement(_roomsContent).Displayed)
                _driver.FindElement(_roomsDropdownBtn).Click();
        }

        public string GetRoomsPlaceholderText() => _driver.FindElement(_roomsPlaceholder).Text.Trim();

        public string GetRoomCounterValue(int rowIndex)
        {
            return _driver.FindElements(_roomsRows)[rowIndex]
                .FindElement(By.CssSelector(".dropdown-room__content-counter-value")).Text;
        }

        public void ChangeRoomValue(int rowIndex, bool increment, int count)
        {
            var row = _driver.FindElements(_roomsRows)[rowIndex];
            var buttonSelector = increment ? "span[data-direction='plus']" : "span[data-direction='minus']";
            var button = row.FindElement(By.CssSelector(buttonSelector));

            for (int i = 0; i < count; i++)
            {
                button.Click();
            }
        }

        public void ResetRoomsToZero()
        {
            var rows = _driver.FindElements(_roomsRows);
            foreach (var row in rows)
            {
                var value = int.Parse(row.FindElement(By.CssSelector(".dropdown-room__content-counter-value")).Text);
                var minusBtn = row.FindElement(By.CssSelector("span[data-direction='minus']"));
                for (int i = 0; i < value; i++)
                {
                    minusBtn.Click();
                }
            }
        }

        // Метод для раскрытия списка Дополнительных удобств
        public void OpenAdditionalAmenities()
        {
            var content = _wait.Until(d => d.FindElement(_expandableAmenitiesContent));
            // Открываем только если список еще не виден
            if (!content.Displayed)
            {
                _driver.FindElement(_expandableAmenitiesBtn).Click();
                // Ждем, пока список станет видимым
                _wait.Until(d => content.Displayed);
            }
        }

        // Метод выбора всех чекбоксов
        public void SetAllAdditionalAmenities(bool state)
        {
            var checkboxes = _driver.FindElements(_additionalCheckboxes);
            var inputs = _driver.FindElements(_additionalInputs);

            for (int i = 0; i < checkboxes.Count; i++)
            {
                if (inputs[i].Selected != state)
                {
                    checkboxes[i].Click();
                }
            }
        }

        // Получение состояний для проверок
        public List<bool> GetAdditionalAmenitiesStates()
        {
            return _driver.FindElements(_additionalInputs)
                          .Select(i => i.Selected)
                          .ToList();
        }

        // Методы для карточек номеров
        public int GetActiveSlideIndex(IWebElement card)
        {
            var indicators = card.FindElements(By.CssSelector(".slider__indicators li"));
            for (int i = 0; i < indicators.Count; i++)
            {
                if (indicators[i].GetAttribute("class").Contains("active"))
                    return i;
            }
            return -1;
        }

        public void ClickNextPhoto(IWebElement card)
        {
            card.FindElement(By.CssSelector(".slider__control_next")).Click();
        }


        public void ClickPrevPhoto(IWebElement card)
        {
            card.FindElement(By.CssSelector(".slider__control_prev")).Click();
        }

        public IList<IWebElement> GetRoomCards() => _driver.FindElements(_roomCards);

        public int GetTotalSlidesCount(IWebElement card)
        {
            return card.FindElements(By.CssSelector(".slider__indicators li")).Count;
        }

        public IList<IWebElement> GetIndicators(IWebElement card)
        {
            return card.FindElements(By.CssSelector(".slider__indicators li"));
        }

        private IWebElement GetActiveImage(IWebElement card)
        {
            int activeIndex = GetActiveSlideIndex(card);
            return card.FindElements(By.CssSelector(".slider__item img"))[activeIndex];
        }

        public string GetActiveImageSrc(IWebElement card) => GetActiveImage(card).GetAttribute("src");

        public void ClickIndicator(IWebElement indicator)
        {
            _wait.Until(d => indicator.Displayed && indicator.Enabled);
            indicator.Click();
        }

        public void WaitUntilSrcChanges(IWebElement card, string oldSrc)
        {
            IWebElement activeImg = GetActiveImage(card);
            WaitForAttributeChanged(activeImg, "src", oldSrc);
        }

        public void ClickRoomDescription(IWebElement card)
        {
            // Находим ссылку внутри конкретной карточки и кликаем
            var link = card.FindElement(_roomDescriptionLink);
            _wait.Until(d => link.Displayed && link.Enabled);
            link.Click();
        }

        // Метод находит все карточки и выбирает первую, где число отзывов > 0
        public IWebElement? GetFirstCardWithComments()
        {
            var cards = _driver.FindElements(_roomCards);

            foreach (var card in cards)
            {
                try
                {
                    var countText = card.FindElement(_commentsCount).Text.Trim();
                    if (int.TryParse(countText, out int count) && count > 0)
                    {
                        return card;
                    }
                }
                catch (NoSuchElementException)
                {
                    continue;
                }
            }
            return null;
        }
    }
}