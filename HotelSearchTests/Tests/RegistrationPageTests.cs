using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using HotelSearchTests.PageObjects;

namespace HotelSearchTests
{
    public class RegistrationPageTests : HeaderFooterTestsBase
    {
        protected override string CurrentPageUrl => RegistrationUrl;

        protected override BasePage CreatePageObject() => new RegistrationPage(Driver);

        // Переопределяем адрес для тестов этого класса
        protected override void NavigateToInitialPage()
        {
            Driver.Navigate().GoToUrl(RegistrationUrl);
        }

        [Test]
        [Description("Проверка ввода данных в поле имя")]
        public void Test_NameRegistrationInput_DisplaysEnteredValue()
        {
            var registrationPage = new RegistrationPage(Driver);
            string testName = "Иван";

            registrationPage.EnterName(testName);

            string actualValue = registrationPage.GetNameInputValue();
            Assert.That(actualValue, Is.EqualTo(testName),
                "Значение в поле ввода имени не совпадает с введенным.");
        }

        [Test]
        [Description("Проверка ввода данных в поле фамилия")]
        public void Test_SurnameRegistrationInput_DisplaysEnteredValue()
        {
            var registrationPage = new RegistrationPage(Driver);
            string testSurname = "Иванов";

            registrationPage.EnterSurname(testSurname);

            string actualValue = registrationPage.GetSurnameInputValue();
            Assert.That(actualValue, Is.EqualTo(testSurname),
                "Значение в поле ввода фамилии не совпадает с введенным.");
        }

        [Test]
        public void SexRadioButtons_ShouldToggleCorrectly()
        {
            var registrationPage = new RegistrationPage(Driver);

            // 1. Выбираем "Мужчина"
            registrationPage.SelectMale();

            Assert.Multiple(() =>
            {
                Assert.That(registrationPage.IsMaleSelected(), Is.True, "Радиокнопка 'Мужчина' должна быть выбрана");
                Assert.That(registrationPage.IsFemaleSelected(), Is.False, "Радиокнопка 'Женщина' не должна быть выбрана");
            });

            // 2. Выбираем "Женщина"
            registrationPage.SelectFemale();

            Assert.Multiple(() =>
            {
                Assert.That(registrationPage.IsFemaleSelected(), Is.True, "Радиокнопка 'Женщина' должна быть выбрана");
                Assert.That(registrationPage.IsMaleSelected(), Is.False, "Радиокнопка 'Мужчина' должна стать неактивной");
            });
        }

        [Test]
        public void ShouldSetBirthDateViaCalendarNavigation()
        {
            var registrationPage = new RegistrationPage(Driver);

            // Выбираем 31 декабря 1988 года (4 клика назад по десятилетиям)
            registrationPage.SelectBirthDateViaCalendar("31", "11", "1988", 4);

            Assert.That(registrationPage.GetDateInputValue(), Is.EqualTo("31.12.1988"),
                "Дата неверно отобразилась после навигации по календарю");
        }

        [Test]
        public void ShouldSetBirthDateViaDirectInput()
        {
            var registrationPage = new RegistrationPage(Driver);
            string date = "01.01.2000";

            var input = Driver.FindElement(By.CssSelector(".date-dropdown_registration"));
            input.Click();
            input.SendKeys(date);

            // Жмем Enter или Применить, чтобы закрыть виджет
            registrationPage.ClickApply();

            Assert.That(registrationPage.GetDateInputValue(), Is.EqualTo(date),
                "Дата неверно отобразилась при ручном вводе");
        }

        [Test]
        public void ShouldClearDate_AfterSelectingViaCalendar()
        {
            var registrationPage = new RegistrationPage(Driver);

            // 1. Выбираем дату через навигацию (31.12.1988)
            registrationPage.SelectBirthDateViaCalendar("31", "11", "1988", 4);

            registrationPage.OpenCalendar();
            registrationPage.ClickClear();

            // 3. Проверка
            Assert.That(registrationPage.IsDateInputEmpty(), Is.True,
                "Поле даты должно быть пустым после очистки выбранной в календаре даты");
        }

        [Test]
        public void ShouldClearDate_AfterManualInput()
        {
            var registrationPage = new RegistrationPage(Driver);
            string date = "01.01.1990";

            // 1. Вводим дату вручную
            var input = Driver.FindElement(By.CssSelector(".date-dropdown_registration"));
            input.Click();
            input.SendKeys(date);

            // 2. Жмем Очистить (календарь обычно открыт при вводе или клике)
            registrationPage.ClickClear();

            // 3. Проверка
            Assert.That(registrationPage.IsDateInputEmpty(), Is.True,
                "Поле даты должно быть пустым после очистки введенного вручную значения");
        }

        [Test]
        [Description("Проверка отображения введенного значения в поле логина")]
        public void LoginInput_ShouldDisplayEnteredValue()
        {
            var registrationPage = new RegistrationPage(Driver);
            string testLogin = "testuser@example.com";

            registrationPage.EnterLogin(testLogin);

            string actualLoginValue = registrationPage.GetLoginInputValue();
            Assert.That(actualLoginValue, Is.EqualTo(testLogin),
                "Значение в поле ввода логина не совпадает с введенным.");
        }

        [Test]
        [Description("Проверка отображения значения в поле пароля")]
        public void PasswordInput_ShouldDisplayEnteredValue()
        {
            var registrationPage = new RegistrationPage(Driver);
            string testPassword = "********";

            registrationPage.EnterPassword(testPassword);

            string actualPasswordValue = registrationPage.GetPasswordInputValue();
            Assert.That(actualPasswordValue, Is.EqualTo(testPassword),
                "Значение в поле ввода пароля не совпадает с введенным.");
        }

        [Test]
        [Description("Проверка переключения тумблера спецпредложений (включение и выключение)")]
        public void SpecialOfferToggle_ShouldSwitchOnAndOff()
        {
            var registrationPage = new RegistrationPage(Driver);

            // 1. Изначально проверяем состояние (обычно выключен)
            bool initialState = registrationPage.IsSpecialOfferSelected();

            // 2. Включаем
            registrationPage.ToggleSpecialOffer();
            Assert.That(registrationPage.IsSpecialOfferSelected(), Is.Not.EqualTo(initialState),
                "Тумблер не включился после первого клика");

            // 3. Выключаем (возвращаем в исходное состояние)
            registrationPage.ToggleSpecialOffer();
            Assert.That(registrationPage.IsSpecialOfferSelected(), Is.EqualTo(initialState),
                "Тумблер не вернулся в исходное состояние после второго клика");
        }

        [Test]
        public void ClickOnGoToPaymentButton()
        {
            var registrationPage = new RegistrationPage(Driver);

            registrationPage.ClickGoToPaymentButton();

            Assert.That(Driver.Url, Does.Contain(RegistrationUrl));
        }

        [Test]
        public void ClickOnLoginRegistrationButton_ShouldNavigateToSignInPage()
        {
            var registrationPage = new RegistrationPage(Driver);

            registrationPage.ClickLoginRegistrationButton();
            string actualUrl = NormalizeUrl(Driver.Url);
            Assert.That(actualUrl, Does.Contain(SignInUrl));
        }
    }
}