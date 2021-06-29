using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PersonalBot.Handlers
{
    public class UpdatingHandler : DefaultUpdateHandler
    {
        private UpdatingHandler(Func<ITelegramBotClient, Update, CancellationToken, Task> updateHandler,
            Func<ITelegramBotClient, Exception, CancellationToken, Task> errorHandler,
            UpdateType[] allowedUpdates = null) : base(updateHandler, errorHandler, allowedUpdates) { }
        
        public UpdatingHandler() : this(HandleUpdateAsync, HandleErrorAsync) { }

        private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            try
            {
                await OnReceivedHandlers.GetHandler(botClient, update);
            }
            catch (Exception e)
            {
                await HandleErrorAsync(botClient, e, cancellationToken);
            }
        }

        private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            string errorMessage;
            if (exception is ApiRequestException apiException)
            {
                errorMessage = "Telegram API Error:" +
                               $"\n[{apiException.ErrorCode}]" +
                               $"\n{apiException.Message}";
            }
            else
            {
                errorMessage = exception.ToString();
            }


            Console.WriteLine(errorMessage);
            
            return Task.CompletedTask;
        }
    }
}