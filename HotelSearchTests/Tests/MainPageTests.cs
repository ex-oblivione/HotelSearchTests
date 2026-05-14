using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using HotelSearchTests.PageObjects;

namespace HotelSearchTests
{
    public class MainPageTests : HeaderFooterTestsBase
    {
        protected override string CurrentPageUrl => BaseUrl;   // для главной страницы

        protected override BasePage CreatePageObject() => new MainPage(Driver);
        [Test]
        [Description("Проверка выбора текущей даты и даты в следующем месяце")]
        public void Test_SelectCurrentAndNextMonthDates()
        {
            var mainPage = new MainPage(Driver);

            DateTime today = DateTime.Today;
            string expectedCheckIn = today.ToString("dd.MM.yyyy");
            DateTime expectedCheckOutDate = new DateTime(today.Year, today.Month, 1).AddMonths(1);
            string expectedCheckOut = expectedCheckOutDate.ToString("dd.MM.yyyy"); 
            
            mainPage.OpenCalendar();
            mainPage.SelectCurrentDay();
            DateTime selectedDate = mainPage.SelectFirstDayOfNextMonth();
            mainPage.ClickApply();

            string actualCheckIn = mainPage.GetCheckInValue();
            string actualCheckOut = mainPage.GetCheckOutValue();

            Assert.Multiple(() =>
            {
                Assert.That(actualCheckIn, Is.EqualTo(expectedCheckIn),
                    "Дата прибытия не совпадает с текущей датой.");

                Assert.That(actualCheckOut, Is.EqualTo(expectedCheckOut),
                    "Дата выезда не совпадает с выбранным числом.");
            });
        }

        [Test]
        [Description("Проверка очистки выбранных дат и сброса значений в маску ДД.ММ.ГГГГ")]
        public void Test_ClickClear_ResetsDatesToMask()
        {
            var mainPage = new MainPage(Driver);

            string expectedValue = string.Empty;
            string expectedMask = "ДД.ММ.ГГГГ";

            mainPage.OpenCalendar();
            mainPage.SelectCurrentDay();
            DateTime selectedDate = mainPage.SelectFirstDayOfNextMonth();
            mainPage.ClickClear();

            string actualCheckInVal = mainPage.GetCheckInValue();
            string actualCheckOutVal = mainPage.GetCheckOutValue();

            string actualCheckInPlaceholder = mainPage.GetCheckInPlaceholder();
            string actualCheckOutPlaceholder = mainPage.GetCheckOutPlaceholder();

            Assert.Multiple(() =>
            {
                Assert.That(actualCheckInVal, Is.EqualTo(expectedValue),
                    "Поле 'Прибытие' не пустое после нажатия 'Очистить'.");
                Assert.That(actualCheckOutVal, Is.EqualTo(expectedValue),
                    "Поле 'Выезд' не пустое после нажатия 'Очистить'.");

                Assert.That(actualCheckInPlaceholder, Is.EqualTo(expectedMask),
                    "У поля 'Прибытие' отсутствует или неверная маска placeholder.");
                Assert.That(actualCheckOutPlaceholder, Is.EqualTo(expectedMask),
                    "У поля 'Выезд' отсутствует или неверная маска placeholder.");
            });
        }

        [Test]
        [Description("Проверка добавления гостей, обновления текста в поле выбора и корректности склонения слов гость и младенец от 1 до 21")]
        public void Test_GuestsDeclension_From1To21()
        {
            var expectedTexts = new Dictionary<int, string>
    {
        { 1, "1 гость, 1 младенец" },
        { 2, "2 гостя, 2 младенца" },
        { 3, "3 гостя, 3 младенца" },
        { 4, "4 гостя, 4 младенца" },
        { 5, "5 гостей, 5 младенцев" },
        { 20, "20 гостей, 20 младенцев" },
        { 21, "21 гость, 21 младенец" }
    };

            var mainPage = new MainPage(Driver);

            mainPage.OpenGuestsDropdown();
            mainPage.ResetGuestsToZero();

            // Проверяем начальное состояние (0 гостей)
            Assert.That(mainPage.GetGuestsDropdownText(), Is.EqualTo("Сколько гостей"),
                "При нуле гостей должен показываться плейсхолдер.");

            // Один проход от 1 до 21: на каждом шаге добавляем взрослого и младенца
            for (int i = 1; i <= 21; i++)
            {
                mainPage.AddAdult();
                mainPage.AddInfant();

                // Если текущее количество входит в список проверяемых — ассертим
                if (expectedTexts.ContainsKey(i))
                {
                    string actualText = mainPage.GetGuestsDropdownText();
                    Assert.That(actualText, Is.EqualTo(expectedTexts[i]),
                        $"Некорректное склонение для значения: {i}");
                }
            }

            mainPage.ClickApplyGuests();
        }

        [Test]
        [Description("Проверка уменьшения числа гостей при нажатии на кнопку минус")]
        public void Test_RemoveGuests_UpdatesDropdownTextAndValues()
        {
            var mainPage = new MainPage(Driver);

            mainPage.OpenGuestsDropdown();
            mainPage.ResetGuestsToZero();

            mainPage.AddAdult();
            mainPage.AddChild();
            mainPage.AddInfant();

            Assert.That(mainPage.GetGuestsDropdownText(), Is.EqualTo("2 гостя, 1 младенец"));

            mainPage.RemoveInfant();
            Assert.Multiple(() =>
            {
                Assert.That(mainPage.GetInfantsCount(), Is.EqualTo("0"), "Количество младенцев не сбросилось в 0");
                Assert.That(mainPage.GetGuestsDropdownText(), Is.EqualTo("2 гостя"), "Текст не обновился на '2 гостя'");
            });

            mainPage.RemoveChild();
            Assert.Multiple(() =>
            {
                Assert.That(mainPage.GetChildrenCount(), Is.EqualTo("0"), "Количество детей не сбросилось в 0");
                Assert.That(mainPage.GetGuestsDropdownText(), Is.EqualTo("1 гость"), "Текст не обновился на '1 гость'");
            });

            mainPage.RemoveAdult();
            Assert.Multiple(() =>
            {
                Assert.That(mainPage.GetAdultsCount(), Is.EqualTo("0"), "Количество взрослых не сбросилось в 0");
                Assert.That(mainPage.GetGuestsDropdownText(), Is.EqualTo("Сколько гостей"), "Текст не вернулся к дефолтному плейсхолдеру");
            });

            mainPage.ClickApplyGuests();
            Assert.That(mainPage.GetGuestsDropdownText(), Is.EqualTo("Сколько гостей"));
        }

        [Test]
        [Description("Проверка невозможности уменьшить количество гостей ниже нуля")]
        public void Test_RemoveGuestsBelowZero_DoesNotChangeValues()
        {
            var mainPage = new MainPage(Driver);

            mainPage.OpenGuestsDropdown();
            mainPage.ResetGuestsToZero();

            mainPage.RemoveAdult();
            Assert.Multiple(() =>
            {
                Assert.That(mainPage.GetAdultsCount(), Is.EqualTo("0"), "Счетчик взрослых ушел в минус или изменился");
                Assert.That(mainPage.GetGuestsDropdownText(), Is.EqualTo("Сколько гостей"), "Текст плейсхолдера некорректно изменился");
            });

            mainPage.RemoveChild();
            Assert.Multiple(() =>
            {
                Assert.That(mainPage.GetChildrenCount(), Is.EqualTo("0"), "Счетчик детей ушел в минус или изменился");
                Assert.That(mainPage.GetGuestsDropdownText(), Is.EqualTo("Сколько гостей"), "Текст плейсхолдера некорректно изменился");
            });

            mainPage.RemoveInfant();
            Assert.Multiple(() =>
            {
                Assert.That(mainPage.GetInfantsCount(), Is.EqualTo("0"), "Счетчик младенцев ушел в минус или изменился");
                Assert.That(mainPage.GetGuestsDropdownText(), Is.EqualTo("Сколько гостей"), "Текст плейсхолдера некорректно изменился");
            });

            mainPage.ClickApplyGuests();
            Assert.That(mainPage.GetGuestsDropdownText(), Is.EqualTo("Сколько гостей"));
        }

        [Test]
        [Description("Проверка сброса выбранных гостей при нажатии на кнопку ОЧИСТИТЬ")]
        public void Test_ClearGuests_ResetsValuesAndKeepsDropdownOpen()
        {
            var mainPage = new MainPage(Driver);

            mainPage.OpenGuestsDropdown();
            mainPage.ResetGuestsToZero();

            mainPage.AddAdult();
            mainPage.AddChild();
            mainPage.AddInfant();

            Assert.That(mainPage.GetGuestsDropdownText(), Is.EqualTo("2 гостя, 1 младенец"),
                "Текст кнопки 'Сколько гостей' не соответствует ожидаемому после добавления");

            mainPage.ClickClearGuests();

            Assert.Multiple(() =>
            {
                Assert.That(mainPage.GetAdultsCount(), Is.EqualTo("0"), "Счетчик взрослых не сбросился на 0");
                Assert.That(mainPage.GetChildrenCount(), Is.EqualTo("0"), "Счетчик детей не сбросился на 0");
                Assert.That(mainPage.GetInfantsCount(), Is.EqualTo("0"), "Счетчик младенцев не сбросился на 0");

                Assert.That(mainPage.GetGuestsDropdownText(), Is.EqualTo("Сколько гостей"),
                    "Текст кнопки не изменился на дефолтный плейсхолдер после нажатия Очистить");

                Assert.That(mainPage.IsDropdownOpen(), Is.True,
                    "Выпадающий список закрылся после нажатия Очистить, а должен оставаться открытым");
            });
        }

        [Test]
        public void ClickOnFindRoomButton_ShouldNavigateToSearchRoomPage()
        {
            var mainPage = new MainPage(Driver);

            mainPage.ClickFindRoomButton();
            string actualUrl = NormalizeUrl(Driver.Url);
            Assert.That(actualUrl, Is.EqualTo(SearchRoomUrl));
        }
    }
}