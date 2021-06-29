using System;
using System.Threading;
using PersonalBot.Handlers;
using PersonalBot.Resources.Providers.Declarations;
using Telegram.Bot;
using TelegramBot.Declarations;
using TelegramBot.Exceptions;

namespace PersonalBot
{
    public class PersonalBot : IBot
    {
        private static PersonalBot _instanse;

        private ITelegramBotClient _bot;
        private readonly ISettingsProvider _settings;
        private CancellationTokenSource _cancellationToken;

        private PersonalBot(ISettingsProvider settingsProvider)
        {
            _settings = settingsProvider;
        }

        public static PersonalBot GetInstance()
        {
            if (_instanse == null)
                throw new BotInstanseNotCreatedException<PersonalBot>();

            return _instanse;
        }
        
        public static PersonalBot CreateInstance(ISettingsProvider settingsProvider)
        {
            _instanse = new PersonalBot(settingsProvider);

            return GetInstance();
        }

        public async void StartAsync()
        {
            if (_cancellationToken is {IsCancellationRequested: false})
            {
                throw new BotAlreadyStartedException(this);
            }
            
            _bot = new TelegramBotClient(_settings["api_token"]);

            var me = await _bot.GetMeAsync();
            Console.Title = me.Username;

            _cancellationToken = new CancellationTokenSource();
            
            _bot.StartReceiving(new UpdatingHandler(), _cancellationToken.Token);
        }

        public void Stop()
        {
            _cancellationToken.Cancel();
        }
    }
}