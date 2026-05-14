# HotelSearchTests
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Selenium](https://img.shields.io/badge/Selenium-43B02A?style=for-the-badge&logo=selenium&logoColor=white)
![NUnit](https://img.shields.io/badge/NUnit-2C8EBB?style=for-the-badge&logo=nunit&logoColor=white)

Автоматизированные UI-тесты на C# для учебного сайта бронирования отелей.  
Проект выполнен в рамках курса FSD — [2nd-task](https://github.com/ex-oblivione/2nd-task).

## Стек технологий

- **C#**
- **Selenium WebDriver**
- **NUnit**
- **Page Object Model**

## Что покрыто тестами

- Фильтрация номеров (даты, цена, удобства)
- Работа с календарём и ползунком цены
- Формы регистрации и входа
- Слайдеры фотографий в карточках номеров
- Хедер, футер, навигация

## Как запустить

1. Клонировать репозиторий
2. Открыть `.sln` в Visual Studio
3. В меню **Тест** → **Обозреватель тестов** → **Запустить все тесты**

## Примечание

Сайт-тестенд статический, поэтому часть кнопок (логин, бронирование) не ведёт на реальные страницы — тесты это учитывают.

# HotelSearchTests

Automated UI tests in C# for a training hotel booking website.  
This project was completed as part of the FSD (Frontend Self-Development) course — [2nd-task](https://github.com/ex-oblivione/2nd-task).

## Tech Stack

- **C#**
- **Selenium WebDriver**
- **NUnit**
- **Page Object Model**

## Covered Functionality

- Room filtering (dates, price range, amenities)
- Calendar and price slider interactions
- Registration and login forms
- Photo sliders in room cards
- Header, footer, and navigation

## How to Run

1. Clone the repository
2. Open the `.sln` file in Visual Studio
3. Go to **Test** → **Test Explorer** → **Run All Tests**

## Note

The test site is static, so some buttons (login, booking) do not navigate to real pages – the tests take this into account.