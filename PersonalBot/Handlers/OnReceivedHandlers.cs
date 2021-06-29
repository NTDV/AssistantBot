using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PersonalBot.Handlers
{
    public static class OnReceivedHandlers
    {
        public static Task GetHandler(ITelegramBotClient botClient, Update update)
        {
            return update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(botClient, update.Message),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery),
                _ => UnknownUpdateHandlerAsync(update)
            };
        }

        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            Console.WriteLine($"Receive message type: {message.Type}");
            
            if (message.Type != MessageType.Text)
                return;

            var sentMessage = await Actions.ProcessAction(botClient, message);
            
            Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");
        }
        
        private static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id,
                $"Received {callbackQuery.Data}");

            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id,
                $"Received {callbackQuery.Data}");
        }
        
        private static Task UnknownUpdateHandlerAsync(Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }
    }
}