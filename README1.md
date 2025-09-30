# 🤖 Telegram Bot на C# (Worker Service)

Telegram-бот, написанный на C# с использованием [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) SDK.  
Бот реализован как **Worker Service** (фоновый сервис), что позволяет запускать его как:
- Windows Service
- systemd unit в Linux
- контейнер в Docker
---
Бот работает с Google Drive API, создает, меняет, копирует документы из Google Drive
Бот считывает данные из Google doc файла (из таблички) и вставляет эти данные в несколько шаблонов документов
---

## 📌 Возможности
- Обработка команд (`/start`, кастомные).
- Ответы текстовыми сообщениями.
- Создание Google документов по шаблонам
- Логирование всех апдейтов.
- Запуск в режиме фоновой службы.

---
Работающий бот можно посмотреть @kab2bot
___

Технологии

.NET 8

Telegram.Bot

Microsoft.Extensions.Hosting (Worker Service)

NUnit, xunit, Moq

Google Apis
