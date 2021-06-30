using System;
using System.Threading;
using PersonalBot.Views;
using TelegramBotBase;

namespace PersonalBot.Controllers
{
    public class NotificationsSender
    {
        private Timer _notificationService;
        private BotBase<StartForm> _bot;
        
        public NotificationsSender(BotBase<StartForm> bot)
        {
            _bot = bot;
        }

        public async void Notify(object state)
        {
            var bot = state as BotBase<StartForm>;

            foreach (var @event in PersonalBot.Database.GetNotifyingEvents())
            {
                try
                {
                    await bot.Client.TelegramClient.SendTextMessageAsync(@event.ChatId, "❗️Напоминание\n" + @event);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public void Start()
        {
            _notificationService = new Timer(Notify, _bot, 0, 30_000);
        }

        public void Stop()
        {
            _notificationService.Dispose();
        }
    }
}