using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using HotelSearchTests.PageObjects;

namespace HotelSearchTests
{
    public class RoomDetailsPageTests : HeaderFooterTestsBase
    {

        private string? _roomUrl;

        [OneTimeSetUp]
        public void RoomSetup()
        {
            // Идем искать номер ОДИН раз
            Driver.Navigate().GoToUrl(SearchRoomUrl);
            var searchPage = new SearchRoomPage(Driver);
            var targetCard = searchPage.GetFirstCardWithComments();

            Assert.That(targetCard, Is.Not.Null, "Номера с отзывами не найдены.");

            searchPage.ClickRoomDescription(targetCard!);
            _roomUrl = Driver.Url; // Запомнили URL
        }

        protected override string CurrentPageUrl => _roomUrl!;

        protected override BasePage CreatePageObject() => new RoomDetailsPage(Driver);

        protected override void NavigateToInitialPage()
        {
            if (!string.IsNullOrEmpty(_roomUrl))
                Driver.Navigate().GoToUrl(_roomUrl);
            else
                base.NavigateToInitialPage(); // на случай, если OneTimeSetUp ещё не выполнился
        }

        [Test]
        [Description("Проверка лайка на странице номера, выбранного по наличию отзывов")]
        public void Test_LikeFunctionality_OnRandomRoomWithComments()
        {

            var detailsPage = new RoomDetailsPage(Driver);
            int commentIndex = 0; // Работаем с первым комментарием на странице

            // Сброс лайка (если уже стоит)
            if (detailsPage.IsLikeSet(commentIndex))
            {
                detailsPage.ClickLike(commentIndex);
                Assert.That(detailsPage.IsLikeSet(commentIndex), Is.False, "Не удалось сбросить лайк.");
            }

            int initialLikes = detailsPage.GetLikeCount(commentIndex);

            // Проверка установки лайка
            detailsPage.ClickLike(commentIndex);
            Assert.Multiple(() =>
            {
                Assert.That(detailsPage.IsLikeSet(commentIndex), Is.True, "Лайк не зафиксирован.");
                Assert.That(detailsPage.GetLikeCount(commentIndex), Is.EqualTo(initialLikes + 1), "Счетчик не вырос.");
            });

            // Проверка снятия лайка
            detailsPage.ClickLike(commentIndex);
            Assert.Multiple(() =>
            {
                Assert.That(detailsPage.IsLikeSet(commentIndex), Is.False, "Лайк не снялся.");
                Assert.That(detailsPage.GetLikeCount(commentIndex), Is.EqualTo(initialLikes), "Счетчик не вернулся в норму.");
            });
        }

        [Test]
        [Description("Проверка выбора текущей даты и даты в следующем месяце")]
        public void Test_SelectCurrentAndNextMonthDates()
        {

            var detailsPage = new RoomDetailsPage(Driver);

            DateTime today = DateTime.Today;
            string expectedCheckIn = today.ToString("dd.MM.yyyy");
            DateTime expectedCheckOutDate = new DateTime(today.Year, today.Month, 1).AddMonths(1);
            string expectedCheckOut = expectedCheckOutDate.ToString("dd.MM.yyyy");
            detailsPage.OpenCalendar();
            detailsPage.SelectCurrentDay();
            DateTime selectedDate = detailsPage.SelectFirstDayOfNextMonth();
            detailsPage.ClickApply();

            string actualCheckIn = detailsPage.GetCheckInValue();
            string actualCheckOut = detailsPage.GetCheckOutValue();

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
            var detailsPage = new RoomDetailsPage(Driver);

            string expectedValue = string.Empty;
            string expectedMask = "ДД.ММ.ГГГГ";

            detailsPage.OpenCalendar();
            detailsPage.SelectCurrentDay();
            DateTime selectedDate = detailsPage.SelectFirstDayOfNextMonth();
            detailsPage.ClickClear();

            string actualCheckInVal = detailsPage.GetCheckInValue();
            string actualCheckOutVal = detailsPage.GetCheckOutValue();

            string actualCheckInPlaceholder = detailsPage.GetCheckInPlaceholder();
            string actualCheckOutPlaceholder = detailsPage.GetCheckOutPlaceholder();

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
        public void ClickOnBookRoomButton()
        {
            var detailsPage = new RoomDetailsPage(Driver);

            detailsPage.OpenCalendar();
            detailsPage.SelectCurrentDay();
            DateTime selectedDate = detailsPage.SelectFirstDayOfNextMonth();

            detailsPage.ClickApply();
            detailsPage.ClickBookRoomButton();

            string actualUrl = NormalizeUrl(Driver.Url);
            Assert.That(actualUrl, Does.Contain(_roomUrl));
        }
    }
}


