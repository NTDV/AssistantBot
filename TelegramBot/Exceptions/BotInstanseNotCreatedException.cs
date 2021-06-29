using System;
using TelegramBot.Declarations;

namespace TelegramBot.Exceptions
{
    public class BotInstanseNotCreatedException<TBot> : Exception where TBot : IBot
    {
        public BotInstanseNotCreatedException() : 
            base($"{typeof(TBot)} instanse not created yet, try to call {nameof(IBot.CreateInstanse)} method.") 
        { }
    }
}