using System.Threading.Tasks;
using PersonalBot.Views.About;
using PersonalBot.Views.Covid;
using PersonalBot.Views.Events;
using PersonalBot.Views.Notes;
using PersonalBot.Views.Weather;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace PersonalBot.Views
{
    public class Start : AutoCleanForm
    {
        public Start()
        {
            Closed += async (_,_) => await Device.HideReplyKeyboard();
        }
        
        public override async Task Action(MessageResult message)
        { 
            var call = message.GetData<CallbackData>();
            await message.ConfirmAction();

            if (call == null)
                return;

            switch (call.Value)
            {
                case "corona":
                    await NavigateTo(new Covid.Covid());
                    return;
                case "notes":
                    await NavigateTo(new NotesMenu());
                    break;
                case "about":
                    await NavigateTo(new About.About());
                    break;
                case "weather":
                    await NavigateTo(new Weather.Weather());
                    return;
                case "events":
                    await NavigateTo(new EventsMenu());
                    break;
                default:
                    return;
            }
        }


        public override async Task Render(MessageResult message)
        {
            ButtonForm form = new ButtonForm();

            form.AddButtonRow(
                new ButtonBase("☣️ Коронавирус", new CallbackData("a", "corona").Serialize()),
                new ButtonBase("🌤 Погода", new CallbackData("a", "weather").Serialize()));
            
            form.AddButtonRow(
                new ButtonBase("🤖 О Помогаторе", new CallbackData("a", "about").Serialize()),
                new ButtonBase("📝 Заметки", new CallbackData("a", "notes").Serialize()),
                new ButtonBase("⏳ События", new CallbackData("a", "events").Serialize()));

            await Device.Send("👋 Я бот, который делает жизнь чуточку проще. Чем я могу помочь?", form);
        }
    }
}