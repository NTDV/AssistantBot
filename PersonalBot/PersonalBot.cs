using System;
using PersonalBot.Controllers;
using PersonalBot.Data;
using PersonalBot.Resources.Providers.Declarations;
using PersonalBot.Views;
using TelegramBot.Declarations;
using TelegramBot.Exceptions;
using TelegramBotBase;

namespace PersonalBot
{
    public class PersonalBot : IBot, IDisposable
    {
        public static DbProvider Database { get; private set; }
        public static ISettingsProvider Settings { get; private set; }
        
        private static PersonalBot _instanse;
        private static NotificationsSender _notificationsSender;

        private BotBase<Start> _bot;


        private PersonalBot(ISettingsProvider settingsProvider)
        {
            Settings = settingsProvider;
        }

        public static PersonalBot GetInstance()
        {
            if (_instanse == null)
                throw new BotInstanseNotCreatedException<PersonalBot>();

            return _instanse;
        }
        
        public static PersonalBot CreateInstance(ISettingsProvider settingsProvider)
        {
            Database = new DbProvider(settingsProvider);
            _instanse = new PersonalBot(settingsProvider);

            return GetInstance();
        }

        public void Start()
        {
            _notificationsSender?.Stop();
            _bot?.Stop();
            
            _bot = new BotBase<Start>(Settings["api_token"]);
            
            _notificationsSender = new NotificationsSender(_bot);
            _notificationsSender.Start();
            
            _bot.Start();
        }

        public void Stop()
        {
            _notificationsSender?.Stop();
            _notificationsSender = null;
            
            _bot?.Stop();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}