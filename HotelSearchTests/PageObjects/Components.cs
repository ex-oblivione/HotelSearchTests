using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelSearchTests.Components
{
    public class CalendarComponent
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Локаторы, специфичные для конкретного календаря
        private readonly By _calendarContainerLocator;
        private readonly By _applyButtonLocator;
        private readonly By _clearButtonLocator;
        private readonly By _nextMonthButtonLocator;
        private readonly By _currentDayLocator;
        private readonly By _availableDaysLocator;        // все кликабельные дни
        private readonly By _firstDayOfMonthLocator;      // первое число месяца (без учёта соседних месяцев)

        public CalendarComponent(
            IWebDriver driver,
            By calendarContainerLocator,
            By applyButtonLocator,
            By clearButtonLocator,
            By nextMonthButtonLocator,
            By currentDayLocator,
            By availableDaysLocator,
            By firstDayOfMonthLocator)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            _calendarContainerLocator = calendarContainerLocator;
            _applyButtonLocator = applyButtonLocator;
            _clearButtonLocator = clearButtonLocator;
            _nextMonthButtonLocator = nextMonthButtonLocator;
            _currentDayLocator = currentDayLocator;
            _availableDaysLocator = availableDaysLocator;
            _firstDayOfMonthLocator = firstDayOfMonthLocator;
        }

        // Открыть календарь, кликнув по указанному полю ввода
        public void Open(By inputFieldLocator)
        {
            Click(inputFieldLocator);
            _wait.Until(d => d.FindElement(_calendarContainerLocator).Displayed);
        }

        public void SelectCurrentDay() => Click(_currentDayLocator);
        public void GoToNextMonth()
        {
            Click(_nextMonthButtonLocator);
            // Ждём, пока календарь перерисуется и появятся дни
            _wait.Until(d => d.FindElements(_availableDaysLocator).Any(el => el.Displayed));
        }

        public DateTime SelectFirstDayOfNextMonth()
        {
            // Переход на следующий месяц
            GoToNextMonth();

            // Получаем элемент первого числа
            var firstDayElement = _wait.Until(d =>
            {
                var element = d.FindElement(_firstDayOfMonthLocator);
                return element.Displayed && element.Enabled ? element : null;
            });

            // Извлекаем атрибуты для возврата даты (опционально)
            int day = int.Parse(firstDayElement.GetAttribute("data-date"));
            int month = int.Parse(firstDayElement.GetAttribute("data-month")) + 1;
            int year = int.Parse(firstDayElement.GetAttribute("data-year"));

            firstDayElement.Click();
            return new DateTime(year, month, day);
        }

        // Нажать кнопку «Применить» и дождаться закрытия календаря
        public void ClickApply()
        {
            Click(_applyButtonLocator);
            _wait.Until(d => d.FindElements(_calendarContainerLocator).Count == 0 ||
                             !d.FindElements(_calendarContainerLocator).Any(c => c.Displayed));
        }

        // Нажать кнопку «Очистить» (календарь обычно остаётся открытым)
        public void ClickClear() => Click(_clearButtonLocator);

        // Получить значение поля ввода даты
        public string GetInputValue(By inputLocator)
        {
            var element = _wait.Until(d => d.FindElement(inputLocator));
            return element.GetAttribute("value")?.Trim() ?? string.Empty;
        }

        // Получить плейсхолдер поля ввода даты
        public string GetPlaceholder(By inputLocator)
        {
            var element = _wait.Until(d => d.FindElement(inputLocator));
            return element.GetAttribute("placeholder")?.Trim() ?? string.Empty;
        }

        private void Click(By locator) => _wait.Until(d => d.FindElement(locator)).Click();
    }

    public class GuestsComponent
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;
    // Локаторы
    private readonly By _dropdownButton = By.XPath("//span[contains(@class, 'dropdown__placeholder') and text()='Сколько гостей']");
    private readonly By _dropdownValue = By.CssSelector(".dropdown__placeholder");
    private readonly By _applyButton = By.CssSelector(".dropdown__bottom-item-apply");
    private readonly By _clearButton = By.CssSelector(".dropdown__bottom-item-clear");
    private readonly By _plusAdults = By.XPath("//div//span[text()='ВЗРОСЛЫЕ']/following-sibling::div//span[@data-direction='plus']");
    private readonly By _plusChildren = By.XPath("//div//span[text()='ДЕТИ']/following-sibling::div//span[@data-direction='plus']");
    private readonly By _plusInfants = By.XPath("//div//span[text()='МЛАДЕНЦЫ']/following-sibling::div//span[@data-direction='plus']");
    private readonly By _minusAdults = By.XPath("//div//span[text()='ВЗРОСЛЫЕ']/following-sibling::div//span[@data-direction='minus']");
    private readonly By _minusChildren = By.XPath("//div//span[text()='ДЕТИ']/following-sibling::div//span[@data-direction='minus']");
    private readonly By _minusInfants = By.XPath("//div//span[text()='МЛАДЕНЦЫ']/following-sibling::div//span[@data-direction='minus']");
    private readonly By _valueAdults = By.XPath("//div//span[text()='ВЗРОСЛЫЕ']/following-sibling::div//span[contains(@class, 'dropdown__content-counter-value')]");
    private readonly By _valueChildren = By.XPath("//div//span[text()='ДЕТИ']/following-sibling::div//span[contains(@class, 'dropdown__content-counter-value')]");
    private readonly By _valueInfants = By.XPath("//div//span[text()='МЛАДЕНЦЫ']/following-sibling::div//span[contains(@class, 'dropdown__content-counter-value')]");

    public GuestsComponent(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
    }

    public void Open() => Click(_dropdownButton);
    public void AddAdult() => Click(_plusAdults);
    public void AddChild() => Click(_plusChildren);
    public void AddInfant() => Click(_plusInfants);
    public void RemoveAdult() => Click(_minusAdults);
    public void RemoveChild() => Click(_minusChildren);
    public void RemoveInfant() => Click(_minusInfants);
    public void ClickApply() => Click(_applyButton);
    public void ClickClear() => Click(_clearButton);

    public string GetText()
    {
        var element = _wait.Until(d => d.FindElement(_dropdownValue));
        return element.Text.Trim();
    }

    public string GetAdultsCount() => _wait.Until(d => d.FindElement(_valueAdults)).Text.Trim();
    public string GetChildrenCount() => _wait.Until(d => d.FindElement(_valueChildren)).Text.Trim();
    public string GetInfantsCount() => _wait.Until(d => d.FindElement(_valueInfants)).Text.Trim();

    public void ResetToZero()
    {
        while (GetAdultsCount() != "0") RemoveAdult();
        while (GetChildrenCount() != "0") RemoveChild();
        while (GetInfantsCount() != "0") RemoveInfant();
    }

    public bool IsOpen()
    {
        try { return _wait.Until(d => d.FindElement(_applyButton)).Displayed; }
        catch { return false; }
    }

    private void Click(By locator) => _wait.Until(d => d.FindElement(locator)).Click();
}
}
