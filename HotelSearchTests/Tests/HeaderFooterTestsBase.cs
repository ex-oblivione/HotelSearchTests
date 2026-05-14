using HotelSearchTests.PageObjects;
using NUnit.Framework;

namespace HotelSearchTests
{
    public abstract class HeaderFooterTestsBase : BaseTest
    {
        protected abstract string CurrentPageUrl { get; }

        // Общие данные для выпадающих списков (вынесены из тестов)
        protected static readonly List<string> ExpectedServicesList = new() { "пашем", "сеем", "жнём", "куём" };
        protected static readonly List<string> ExpectedAgreementsList = new() { "согласен", "согласен", "не согласен" };

        // ТЕСТЫ ХЕДЕРА 

        [Test]
        public void ClickOnLogoButton_ShouldNavigateToHomePage()
        {
            var page = CreatePageObject();
            page.ClickHeaderLogo();
            string actualUrl = NormalizeUrl(Driver.Url);
            Assert.That(actualUrl, Is.EqualTo(BaseUrl));
        }

        [Test]
        public void ClickOnHeaderAboutUsButton_ShouldStayOnCurrentPage()
        {
            var page = CreatePageObject();
            page.ClickHeaderAboutUsButton();
            string actualUrl = NormalizeUrl(Driver.Url);
            Assert.That(actualUrl, Is.EqualTo(CurrentPageUrl));
        }

        [Test]
        public void HoverOnHeaderServicesButton_ShouldShowExpectedList()
        {
            var page = CreatePageObject();
            var actualTexts = page.HoverHeaderServicesButton();
            Assert.That(actualTexts, Is.EqualTo(ExpectedServicesList));
        }

        [Test]
        public void ClickOnHeaderVacanciesButton_ShouldStayOnCurrentPage()
        {
            var page = CreatePageObject();
            page.ClickHeaderVacanciesButton();
            Assert.That(Driver.Url, Is.EqualTo(CurrentPageUrl));
        }

        [Test]
        public void ClickOnHeaderNewsButton_ShouldStayOnCurrentPage()
        {
            var page = CreatePageObject();
            page.ClickHeaderNewsButton();
            Assert.That(Driver.Url, Is.EqualTo(CurrentPageUrl));
        }

        [Test]
        public void HoverOnHeaderAgreementsButton_ShouldShowExpectedList()
        {
            var page = CreatePageObject();
            var actualTexts = page.HoverHeaderAgreementsButton();
            Assert.That(actualTexts, Is.EqualTo(ExpectedAgreementsList));
        }

        [Test]
        public void ClickOnLoginButton_ShouldNavigateToSignInPage()
        {
            var page = CreatePageObject();
            page.ClickLoginButton();
            string actualUrl = NormalizeUrl(Driver.Url);
            Assert.That(actualUrl, Is.EqualTo(SignInUrl));
        }

        [Test]
        public void ClickOnRegisterButton_ShouldNavigateToRegistrationPage()
        {
            var page = CreatePageObject();
            page.ClickRegisterButton();
            string actualUrl = NormalizeUrl(Driver.Url);
            Assert.That(actualUrl, Is.EqualTo(RegistrationUrl));
        }

        //  ТЕСТЫ ФУТЕРА 

        [Test]
        public void ClickOnFooterLogoButton_ShouldNavigateToHomePage()
        {
            var page = CreatePageObject();
            page.ClickFooterLogo();
            string actualUrl = NormalizeUrl(Driver.Url);
            Assert.That(actualUrl, Is.EqualTo(BaseUrl));
        }

        [Test]
        public void ClickOnFooterAboutUsButton_ShouldStayOnCurrentPage()
        {
            var page = CreatePageObject();
            page.ClickFooterAboutUsButton();
            string actualUrl = NormalizeUrl(Driver.Url);
            Assert.That(actualUrl, Is.EqualTo(CurrentPageUrl));
        }

        [Test]
        public void ClickOnFooterNewsButton_ShouldStayOnCurrentPage()
        {
            var page = CreatePageObject();
            page.ClickFooterNewsButton();
            string actualUrl = NormalizeUrl(Driver.Url);
            Assert.That(actualUrl, Is.EqualTo(CurrentPageUrl));
        }

        [Test]
        public void ClickOnFooterSupportButton_ShouldStayOnCurrentPage()
        {
            var page = CreatePageObject();
            page.ClickFooterSupportButton();
            string actualUrl = NormalizeUrl(Driver.Url);
            Assert.That(actualUrl, Is.EqualTo(CurrentPageUrl));
        }

        [Test]
        public void ClickOnFooterServicesButton_ShouldStayOnCurrentPage()
        {
            var page = CreatePageObject();
            page.ClickFooterServicesButton();
            string actualUrl = NormalizeUrl(Driver.Url);
            Assert.That(actualUrl, Is.EqualTo(CurrentPageUrl));
        }

        [Test]
        public void ClickOnFooterAboutServiceButton_ShouldStayOnCurrentPage()
        {
            var page = CreatePageObject();
            page.ClickFooterAboutServiceButton();
            string actualUrl = NormalizeUrl(Driver.Url);
            Assert.That(actualUrl, Is.EqualTo(CurrentPageUrl));
        }

        [Test]
        public void ClickOnFooterOurTeamButton_ShouldStayOnCurrentPage()
        {
            var page = CreatePageObject();
            page.ClickFooterOurTeamButton();
            string actualUrl = NormalizeUrl(Driver.Url);
            Assert.That(actualUrl, Is.EqualTo(CurrentPageUrl));
        }

        [Test]
        public void ClickOnFooterVacanciesButton_ShouldStayOnCurrentPage()
        {
            var page = CreatePageObject();
            page.ClickFooterVacanciesButton();
            string actualUrl = NormalizeUrl(Driver.Url);
            Assert.That(actualUrl, Is.EqualTo(CurrentPageUrl));
        }

        [Test]
        public void ClickOnFooterInvestorsButton_ShouldStayOnCurrentPage()
        {
            var page = CreatePageObject();
            page.ClickFooterInvestorsButton();
            string actualUrl = NormalizeUrl(Driver.Url);
            Assert.That(actualUrl, Is.EqualTo(CurrentPageUrl));
        }

        [Test]
        public void ClickOnFooterAgreementsButton_ShouldStayOnCurrentPage()
        {
            var page = CreatePageObject();
            page.ClickFooterAgreementsButton();
            string actualUrl = NormalizeUrl(Driver.Url);
            Assert.That(actualUrl, Is.EqualTo(CurrentPageUrl));
        }

        [Test]
        public void ClickOnFooterCommunitiesButton_ShouldStayOnCurrentPage()
        {
            var page = CreatePageObject();
            page.ClickFooterCommunitiesButton();
            string actualUrl = NormalizeUrl(Driver.Url);
            Assert.That(actualUrl, Is.EqualTo(CurrentPageUrl));
        }

        [Test]
        public void ClickOnFooterContactUsButton_ShouldStayOnCurrentPage()
        {
            var page = CreatePageObject();
            page.ClickFooterContactUsButton();
            string actualUrl = NormalizeUrl(Driver.Url);
            Assert.That(actualUrl, Is.EqualTo(CurrentPageUrl));
        }

        [Test]
        public void FooterSubscribeInput_ShouldDisplayEnteredValue()
        {
            var page = CreatePageObject();
            string testEmail = "testuser@example.com";
            page.EnterSubscriptionEmail(testEmail);
            string actualValue = page.GetSubscriptionInputValue();
            Assert.That(actualValue, Is.EqualTo(testEmail));
        }

        // Вспомогательный метод, который создаёт конкретный PageObject.
        // Его переопределяют наследники.
        protected abstract BasePage CreatePageObject();
    }
}