using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using HotelSearchTests.PageObjects;

namespace HotelSearchTests
{
    public class SearchRoomTests : HeaderFooterTestsBase
    {
        protected override string CurrentPageUrl => SearchRoomUrl;

        protected override BasePage CreatePageObject() => new SearchRoomPage(Driver);
        protected override void NavigateToInitialPage()
        {
            Driver.Navigate().GoToUrl(SearchRoomUrl);
        }

        [Test]
        [Description("Проверка выбора диапазона дат в фильтре 'С какого по какое?'")]
        public void Test_SelectDateRange_UpdatesInputText()
        {
            var searchPage = new SearchRoomPage(Driver);

            DateTime today = DateTime.Today;
            DateTime firstOfNextMonth = new DateTime(today.Year, today.Month, 1).AddMonths(1);

            string expectedText = $"{today.Day} {searchPage.GetShortMonth(today)} - {firstOfNextMonth.Day} {searchPage.GetShortMonth(firstOfNextMonth)}";

            searchPage.OpenCalendar();
            searchPage.SelectCurrentDay();
            DateTime selectedDate = searchPage.SelectFirstDayOfNextMonth();

            searchPage.ClickApply();

            string actualText = searchPage.GetFilterDateValue();
            Assert.That(actualText, Is.EqualTo(expectedText), "Текст в инпуте фильтра дат не соответствует ожидаемому формату диапазона.");
        }

        [Test]
        [Description("Проверка очистки выбранных дат и сброса значений в плейсхолдер С какого по какое?")]
        public void Test_ClickClear_ResetsDatesToMask()
        {
            var searchPage = new SearchRoomPage(Driver);

            string expectedValue = string.Empty;
            string expectedMask = "С какого по какое?";

            searchPage.OpenCalendar();
            searchPage.SelectCurrentDay();
            DateTime selectedDate = searchPage.SelectFirstDayOfNextMonth();

            searchPage.ClickClear();

            string actualFilterDateInputVal = searchPage.GetFilterDateInputValue();

            string actualFilterDateInputPlaceholder = searchPage.GetFilterDateInputPlaceholder();

            Assert.Multiple(() =>
            {
                Assert.That(actualFilterDateInputVal, Is.EqualTo(expectedValue),
                    "Поле 'Даты пребывания в отеле' не пустое после нажатия 'Очистить'.");

                Assert.That(actualFilterDateInputPlaceholder, Is.EqualTo(expectedMask),
                    "У поля 'Даты пребывания в отеле' отсутствует или неверная маска плейсхолдер.");
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

            var searchPage = new SearchRoomPage(Driver);

            searchPage.OpenGuestsDropdown();
            searchPage.ResetGuestsToZero();

            // Проверяем начальное состояние (0 гостей)
            Assert.That(searchPage.GetGuestsDropdownText(), Is.EqualTo("Сколько гостей"),
                "При нуле гостей должен показываться плейсхолдер.");

            // Один проход от 1 до 21: на каждом шаге добавляем взрослого и младенца
            for (int i = 1; i <= 21; i++)
            {
                searchPage.AddAdult();
                searchPage.AddInfant();

                // Если текущее количество входит в список проверяемых — ассертим
                if (expectedTexts.ContainsKey(i))
                {
                    string actualText = searchPage.GetGuestsDropdownText();
                    Assert.That(actualText, Is.EqualTo(expectedTexts[i]),
                        $"Некорректное склонение для значения: {i}");
                }
            }

            searchPage.ClickApplyGuests();
        }


        [Test]
        [Description("Проверка уменьшения числа гостей при нажатии на кнопку минус")]
        public void Test_RemoveGuests_UpdatesDropdownTextAndValues()
        {
            var searchPage = new SearchRoomPage(Driver);

            searchPage.OpenGuestsDropdown();
            searchPage.ResetGuestsToZero();

            searchPage.AddAdult();
            searchPage.AddChild();
            searchPage.AddInfant();

            Assert.That(searchPage.GetGuestsDropdownText(), Is.EqualTo("2 гостя, 1 младенец"));

            searchPage.RemoveInfant();
            Assert.Multiple(() =>
            {
                Assert.That(searchPage.GetInfantsCount(), Is.EqualTo("0"), "Количество младенцев не сбросилось в 0");
                Assert.That(searchPage.GetGuestsDropdownText(), Is.EqualTo("2 гостя"), "Текст не обновился на '2 гостя'");
            });

            searchPage.RemoveChild();
            Assert.Multiple(() =>
            {
                Assert.That(searchPage.GetChildrenCount(), Is.EqualTo("0"), "Количество детей не сбросилось в 0");
                Assert.That(searchPage.GetGuestsDropdownText(), Is.EqualTo("1 гость"), "Текст не обновился на '1 гость'");
            });

            searchPage.RemoveAdult();
            Assert.Multiple(() =>
            {
                Assert.That(searchPage.GetAdultsCount(), Is.EqualTo("0"), "Количество взрослых не сбросилось в 0");
                Assert.That(searchPage.GetGuestsDropdownText(), Is.EqualTo("Сколько гостей"), "Текст не вернулся к дефолтному плейсхолдеру");
            });

            searchPage.ClickApplyGuests();
            Assert.That(searchPage.GetGuestsDropdownText(), Is.EqualTo("Сколько гостей"));
        }

        [Test]
        [Description("Негативный тест: проверка невозможности уменьшить количество гостей ниже нуля")]
        public void Test_RemoveGuestsBelowZero_DoesNotChangeValues()
        {
            var searchPage = new SearchRoomPage(Driver);

            searchPage.OpenGuestsDropdown();
            searchPage.ResetGuestsToZero();

            searchPage.RemoveAdult();
            Assert.Multiple(() =>
            {
                Assert.That(searchPage.GetAdultsCount(), Is.EqualTo("0"), "Счетчик взрослых ушел в минус или изменился");
                Assert.That(searchPage.GetGuestsDropdownText(), Is.EqualTo("Сколько гостей"), "Текст плейсхолдера некорректно изменился");
            });

            searchPage.RemoveChild();
            Assert.Multiple(() =>
            {
                Assert.That(searchPage.GetChildrenCount(), Is.EqualTo("0"), "Счетчик детей ушел в минус или изменился");
                Assert.That(searchPage.GetGuestsDropdownText(), Is.EqualTo("Сколько гостей"), "Текст плейсхолдера некорректно изменился");
            });

            searchPage.RemoveInfant();
            Assert.Multiple(() =>
            {
                Assert.That(searchPage.GetInfantsCount(), Is.EqualTo("0"), "Счетчик младенцев ушел в минус или изменился");
                Assert.That(searchPage.GetGuestsDropdownText(), Is.EqualTo("Сколько гостей"), "Текст плейсхолдера некорректно изменился");
            });

            searchPage.ClickApplyGuests();
            Assert.That(searchPage.GetGuestsDropdownText(), Is.EqualTo("Сколько гостей"));
        }

        [Test]
        [Description("Проверка сброса выбранных гостей при нажатии на кнопку ОЧИСТИТЬ")]
        public void Test_ClearGuests_ResetsValuesAndKeepsDropdownOpen()
        {
            var searchPage = new SearchRoomPage(Driver);

            searchPage.OpenGuestsDropdown();
            searchPage.ResetGuestsToZero();

            searchPage.AddAdult();
            searchPage.AddChild();
            searchPage.AddInfant();

            Assert.That(searchPage.GetGuestsDropdownText(), Is.EqualTo("2 гостя, 1 младенец"),
                "Текст кнопки 'Сколько гостей' не соответствует ожидаемому после добавления");

            searchPage.ClickClearGuests();

            Assert.Multiple(() =>
            {
                Assert.That(searchPage.GetAdultsCount(), Is.EqualTo("0"), "Счетчик взрослых не сбросился на 0");
                Assert.That(searchPage.GetChildrenCount(), Is.EqualTo("0"), "Счетчик детей не сбросился на 0");
                Assert.That(searchPage.GetInfantsCount(), Is.EqualTo("0"), "Счетчик младенцев не сбросился на 0");

                Assert.That(searchPage.GetGuestsDropdownText(), Is.EqualTo("Сколько гостей"),
                    "Текст кнопки не изменился на дефолтный плейсхолдер после нажатия Очистить");

                Assert.That(searchPage.IsDropdownOpen(), Is.True,
                    "Выпадающий список закрылся после нажатия Очистить, а должен оставаться открытым");
            });
        }

        [Test]
        [Description("Проверка установки ползунком экстремальных значений цены")]
        public void Test_PriceSlider_SetMinAndMaxBoundaries()
        {
            var searchPage = new SearchRoomPage(Driver);

            searchPage.DragSliderToEdge(toMax: false);

            searchPage.DragSliderToEdge(toMax: true);

            string actualMinPrice = searchPage.GetPriceFromText();
            string actualMaxPrice = searchPage.GetPriceToText();

            Assert.Multiple(() =>
            {
                Assert.That(actualMinPrice, Does.Contain("0"),
                    $"Минимальная цена не сбросилась в 0. Получено: {actualMinPrice}");

                Assert.That(actualMaxPrice.Replace(" ", ""), Does.Contain("15000"),
                    $"Максимальная цена не установилась в 15 000. Получено: {actualMaxPrice}");
            });
        }

        [Test]
        [Description("Комплексная проверка: установка и снятие всех чекбоксов 'Правила дома'")]
        public void Test_HouseRules_ToggleCycle()
        {
            var searchPage = new SearchRoomPage(Driver);

            searchPage.SetAllRules(true);
            var statesAfterSet = searchPage.GetRulesStates();

            Assert.Multiple(() =>
            {
                for (int i = 0; i < statesAfterSet.Count; i++)
                {
                    Assert.That(statesAfterSet[i], Is.True, $"Чекбокс №{i + 1} не установился в состояние 'Выбран'.");
                }
            });

            searchPage.SetAllRules(false);
            var statesAfterUnset = searchPage.GetRulesStates();

            Assert.Multiple(() =>
            {
                for (int i = 0; i < statesAfterUnset.Count; i++)
                {
                    Assert.That(statesAfterUnset[i], Is.False, $"Чекбокс №{i + 1} не снялся (остался в состоянии 'Выбран').");
                }
            });
        }

        [Test]
        [Description("Проверка корректности склонения всех типов удобств от 1 до 21")]
        public void Test_RoomsDeclension_From1To21()
        {
            var expectedTextBedrooms = new Dictionary<int, string>
            {
                { 1, "1 спальня" },
                { 2, "2 спальни" },
                { 3, "3 спальни" },
                { 4, "4 спальни" },
                { 5, "5 спален" },
                { 20, "20 спален" },
                { 21, "21 спальня" }
            };

            var expectedTextBeds = new Dictionary<int, string>
            {
                { 1, "1 кровать" },
                { 2, "2 кровати" },
                { 3, "3 кровати" },
                { 4, "4 кровати"},
                { 5, "5 кроватей" },
                { 20, "20 кроватей" },
                { 21, "21 кровать" }
            };

            var expectedTextBath = new Dictionary<int, string>
            {
                { 1, "1 ванная" },
                { 2, "2 ванные" },
                { 3, "3 ванные" },
                { 4, "4 ванные"},
                { 5, "5 ванных" },
                { 20, "20 ванных" },
                { 21, "21 ванная" }
            };


            var searchPage = new SearchRoomPage(Driver);
            searchPage.OpenRoomsDropdown();
            searchPage.ResetRoomsToZero();

            Assert.That(searchPage.GetRoomsPlaceholderText(), Is.EqualTo("Выберете из списка"),
                "При нулевом значении выбора удобств должен показываться плейсхолдер.");

            for (int i = 1; i <= 21; i++)
            {
                searchPage.ChangeRoomValue(0, true, 1);
                if (expectedTextBedrooms.ContainsKey(i))
                {
                    Assert.That(searchPage.GetRoomsPlaceholderText(), Does.StartWith(expectedTextBedrooms[i]),
                        $"Некорректное склонение для спален при значении {i}");
                }
            }

            searchPage.ResetRoomsToZero();
            for (int i = 1; i <= 21; i++)
            {
                searchPage.ChangeRoomValue(1, true, 1);
                if (expectedTextBeds.ContainsKey(i))
                {
                    Assert.That(searchPage.GetRoomsPlaceholderText(), Does.StartWith(expectedTextBeds[i]),
                        $"Некорректное склонение для кроватей при значении {i}");
                }
            }

            searchPage.ResetRoomsToZero();
            for (int i = 1; i <= 21; i++)
            {
                searchPage.ChangeRoomValue(2, true, 1);
                if (expectedTextBath.ContainsKey(i))
                {
                    Assert.That(searchPage.GetRoomsPlaceholderText(), Does.StartWith(expectedTextBath[i]),
                        $"Некорректное склонение для ванных при значении {i}");
                }
            }
        }

        [Test]
        [Description("Проверка отображения комбинаций удобств и обрезки длинного текста")]
        public void Test_RoomsDropdown_Combinations()
        {
            var searchPage = new SearchRoomPage(Driver);
            searchPage.OpenRoomsDropdown();

            // Ситуация 1: "1 спальня, 1 кровать…"
            searchPage.ResetRoomsToZero();
            searchPage.ChangeRoomValue(0, true, 1); // спальня
            searchPage.ChangeRoomValue(1, true, 1); // кровать
            searchPage.ChangeRoomValue(2, true, 1); // ванная

            Assert.That(searchPage.GetRoomsPlaceholderText(), Is.EqualTo("1 спальня, 1 кровать…"),
                "Текст должен содержать спецсимвол многоточия (U+2026) при обрезке");

            // Ситуация 2: "1 кровать, 1 ванная"
            searchPage.ResetRoomsToZero();
            searchPage.ChangeRoomValue(1, true, 1); // кровать
            searchPage.ChangeRoomValue(2, true, 1); // ванная

            Assert.That(searchPage.GetRoomsPlaceholderText(), Is.EqualTo("1 кровать, 1 ванная"),
                "При выборе двух последних элементов многоточие не ожидается");
        }

        [Test]
        [Description("Проверка того, что счетчики удобств не уменьшаются ниже нуля при клике на 'минус'")]
        public void Test_RoomsDropdown_CountersStayAtZero()
        {
            var searchPage = new SearchRoomPage(Driver);
            searchPage.OpenRoomsDropdown();

            searchPage.ResetRoomsToZero();

            for (int i = 0; i < 3; i++)
            {
                searchPage.ChangeRoomValue(i, increment: false, count: 2);
            }

            Assert.Multiple(() =>
            {
                Assert.That(searchPage.GetRoomCounterValue(0), Is.EqualTo("0"),
                    "Счетчик спален стал меньше нуля!");

                Assert.That(searchPage.GetRoomCounterValue(1), Is.EqualTo("0"),
                    "Счетчик кроватей стал меньше нуля!");

                Assert.That(searchPage.GetRoomCounterValue(2), Is.EqualTo("0"),
                    "Счетчик ванных комнат стал меньше нуля!");

                // Проверяем, что плейсхолдер не отображает отрицательных чисел
                Assert.That(searchPage.GetRoomsPlaceholderText(), Does.Not.Contain("-"),
                    "В плейсхолдере отображается отрицательное значение или символ минуса");
            });
        }


        [Test]
        [Description("Проверка блока 'Дополнительные удобства': раскрытие списка, установка и снятие всех чекбоксов")]
        public void Test_AdditionalAmenities_ExpandAndToggleCycle()
        {
            var searchPage = new SearchRoomPage(Driver);

            searchPage.OpenAdditionalAmenities();

            searchPage.SetAllAdditionalAmenities(true);
            var statesAfterSet = searchPage.GetAdditionalAmenitiesStates();

            Assert.Multiple(() =>
            {
                for (int i = 0; i < statesAfterSet.Count; i++)
                {
                    Assert.That(statesAfterSet[i], Is.True, $"Доп. удобство №{i + 1} не было выбрано.");
                }
            });

            searchPage.SetAllAdditionalAmenities(false);
            var statesAfterUnset = searchPage.GetAdditionalAmenitiesStates();

            Assert.Multiple(() =>
            {
                for (int i = 0; i < statesAfterUnset.Count; i++)
                {
                    Assert.That(statesAfterUnset[i], Is.False, $"Доп. удобство №{i + 1} осталось выбранным.");
                }
            });
        }

        [Test]
        [Description("Проверка циклического перелистывания фотографий в карточке номера")]
        public void Test_RoomCardSlider_CyclicNavigation()
        {
            var searchPage = new SearchRoomPage(Driver);
            var firstCard = searchPage.GetRoomCards().First();

            int totalPhotos = searchPage.GetTotalSlidesCount(firstCard);

            Assert.That(totalPhotos, Is.GreaterThan(0), "В карточке отсутствуют фото для слайдера.");

            Assert.Multiple(() =>
            {
                // 1. Проверка пролистывания ВПЕРЕД
                for (int i = 0; i < totalPhotos; i++)
                {
                    int currentIndex = searchPage.GetActiveSlideIndex(firstCard);
                    Assert.That(currentIndex, Is.EqualTo(i), $"Ожидался слайд {i} при движении вперед");
                    searchPage.ClickNextPhoto(firstCard);
                }

                // После 4-го клика индекс должен вернуться на 0
                Assert.That(searchPage.GetActiveSlideIndex(firstCard), Is.EqualTo(0),
                    "Слайдер не вернулся на первое фото после завершения круга вперед");

                // 2. Проверка пролистывания НАЗАД (из положения 0)
                searchPage.ClickPrevPhoto(firstCard);
                int lastIndex = totalPhotos - 1;
                Assert.That(searchPage.GetActiveSlideIndex(firstCard), Is.EqualTo(lastIndex), "При клике 'назад' с первого фото слайдер не перешел на последнее");

                // Возвращаемся назад на первое фото
                for (int i = lastIndex; i > 0; i--)
                {
                    searchPage.ClickPrevPhoto(firstCard);
                }
                Assert.That(searchPage.GetActiveSlideIndex(firstCard), Is.EqualTo(0),
                    "Слайдер не вернулся на первое фото при движении назад");
            });
        }

        [Test]
        [Description("Переключение всех индикаторов с ожиданием смены изображений в карточке номера")]
        public void Test_RoomCardSlider_Indicators_FullCycle()
        {
            var searchPage = new SearchRoomPage(Driver);
            // Берем первую карточку из списка
            var firstCard = searchPage.GetRoomCards().First();
            var indicators = searchPage.GetIndicators(firstCard);

            // Фиксируем начальный src
            string lastImageSrc = searchPage.GetActiveImageSrc(firstCard);

            Assert.Multiple(() =>
            {
                // Проходим по всем индикаторам
                for (int i = 0; i < indicators.Count; i++)
                {
                    // Кликаем по индикатору (используя безопасный клик)
                    searchPage.ClickIndicator(indicators[i]);

                    // Если это не первый слайд (который уже открыт), ждем смены src
                    if (searchPage.GetActiveSlideIndex(firstCard) != i || i > 0)
                    {
                        searchPage.WaitUntilSrcChanges(firstCard, lastImageSrc);
                    }

                    string currentImageSrc = searchPage.GetActiveImageSrc(firstCard);

                    Assert.That(currentImageSrc, Is.Not.Null.And.Not.Empty,
                        $"Слайд {i}: Изображение не загрузилось (пустой src)");

                    if (i > 0)
                    {
                        Assert.That(currentImageSrc, Is.Not.EqualTo(lastImageSrc),
                            $"Слайд {i}: Изображение не обновилось после клика на индикатор");
                    }

                    lastImageSrc = currentImageSrc;
                }
            });
        }

        [Test]
        [Description("Проверка перехода на страницу описания номера")]
        public void Test_RoomCard_ClickDescription_RedirectsToDetailsPage()
        {
            var searchPage = new SearchRoomPage(Driver);

            // 1. Берем карточку
            var firstCard = searchPage.GetRoomCards().First();

            // 2. Кликаем (ожидание кликабельности внутри метода страницы)
            searchPage.ClickRoomDescription(firstCard);

            // 3. Проверяем результат (ожидание перехода тоже внутри метода страницы)
            bool isRedirected = searchPage.IsRedirectedToDetails("room-describe.html");

            Assert.That(isRedirected, Is.True, "Переход на страницу описания номера не произошел.");
        }
    }
}