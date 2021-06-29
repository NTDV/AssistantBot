using System;
using TelegramBot.Declarations;

namespace TelegramBot.Exceptions
{
    public class BotAlreadyStartedException : Exception
    {
        public BotAlreadyStartedException() { }

        public BotAlreadyStartedException(IBot bot) : 
            base($"Bot {bot} already started, try to call {nameof(IBot.Stop)} method.")
        { }
    }
}