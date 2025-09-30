# 🤖 Telegram Bot на C# (Worker Service)

Telegram-бот, написаний на C# з використанням [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) SDK.  
Бот реалізований як **Worker Service** (фоновий сервіс), що дозволяє запускати його як:
- Windows Service
- systemd unit у Linux
- контейнер у Docker

---
Бот працює з **Google Drive API**: створює, змінює, копіює документи з Google Drive.  
Також бот зчитує дані з файлу **Google Docs** (таблиці) та вставляє ці дані у кілька шаблонів документів.

---

## 📌 Можливості
- Обробка команд (`/start`, `/help` кастомні).
- Відповіді текстовими повідомленнями.
- Створення Google документів за шаблонами.
- Логування всіх апдейтів.
- Запуск у режимі фонового сервісу.

---

Працюючого бота можна переглянути тут: **[@kab2bot](https://t.me/kab2bot)**

---

## 🛠 Технології
- .NET 8
- Telegram.Bot
- Microsoft.Extensions.Hosting (Worker Service)
- NUnit, xUnit, Moq
- Google APIs

## 📂 Структура проєкту

```text
DocumentService/                      # Рішення (Solution)
├── Solution Items/
│   ├── .gitignore
│   └── README.md
│
├── DocumentService.Documents/        # Бібліотека для роботи з Google Drive
│   ├── Tests/                        # Тести
│   │   ├── GoogleDriveIntegrationTests.cs   # Інтеграційні тести з Google API
│   │   ├── GoogleDriveMockTests.cs          # Unit-тести з Moq
│   │   └── credentials.json                 # Облікові дані для Google API
│   │
│   ├── Dates.cs                        # Допоміжний клас для роботи з датами
│   ├── DriveFileManager.cs             # Менеджер для керування файлами в Google Drive
│   ├── IGoogleDriveService.cs          # Інтерфейс сервісу Google Drive
│   └── GoogleDriveService.cs           # Реалізація сервісу Google Drive
│
└── DocumentService.TelegramBotWS/      # Telegram Bot (Worker Service)
    ├── Properties/
    │   └── launchSettings.json         # Налаштування запуску
    │
    ├── publish/                        # Папка для публікації
    ├── .env                            # Змінні середовища
    ├── appsettings.json                # Конфігурація застосунку
    ├── DotEnv.cs                       # Завантаження .env
    ├── Program.cs                      # Точка входу
    └── Worker.cs                       # Основний сервіс (BackgroundService)
