using System;
using PersonalBot.Resources.Providers.Declarations;
using PersonalBot.Resources.Providers.Implementations;
using TelegramBot.Declarations;

namespace Assistant
{
    internal static class Assistant
    {
        public static void Main()
        {
            ISettingsProvider settings = new StaticSettingsProvider("Resources/settings.json");
            
            IBot bot = PersonalBot.PersonalBot.CreateInstance(settings);
            bot.Start();
            
            Console.ReadLine();
            bot.Stop();
        }
    }
}