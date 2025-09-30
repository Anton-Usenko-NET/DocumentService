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
- Обробка команд (`/start`, кастомні).
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
