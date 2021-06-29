using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PersonalBot.Handlers
{
    public static class Actions
    {
        public static Task<Message> ProcessAction(ITelegramBotClient botClient, Message message)
        {
            return message.Text.Split(' ').First() switch
            {
                "/keyboard" => SendInlineKeyboard(botClient, message),
                _ => Usage(botClient, message)
            };
        }
        
        private static async Task<Message> SendInlineKeyboard(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            InlineKeyboardMarkup inlineKeyboard = new (new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("1", "1"),
                    InlineKeyboardButton.WithCallbackData("2", "2"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("3", "3"),
                    InlineKeyboardButton.WithCallbackData("4", "4"),
                },
            });
            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id, 
                text: "Choose", 
                replyMarkup: inlineKeyboard);
        }
        
        private static async Task<Message> Usage(ITelegramBotClient botClient, Message message)
        {
            const string usage = "Usage:\n" +
                                 "/keyboard   - send inline keyboard\n";
            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                text: usage,
                replyMarkup: new ReplyKeyboardRemove());
        }
    }
}