using System.Threading.Tasks;
using PersonalBot.Views.Covid;
using PersonalBot.Views.Events;
using PersonalBot.Views.Notes;
using PersonalBot.Views.Weather;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace PersonalBot.Views
{
    public class StartForm : AutoCleanForm
    {
        public StartForm()
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
                    await NavigateTo(new CovidForm());
                    return;
                case "notes":
                    await NavigateTo(new NotesMenuForm());
                    break;
                case "about":
                case "weather":
                    await NavigateTo(new WeatherForm());
                    return;
                case "events":
                    await NavigateTo(new EventsMenuForm());
                    break;
                default:
                    return;
            }
        }


        public override async Task Render(MessageResult message)
        {
            ButtonForm form = new ButtonForm();

            form.AddButtonRow(
                new ButtonBase("Коронавирус", new CallbackData("a", "corona").Serialize()),
                new ButtonBase("Погода", new CallbackData("a", "weather").Serialize()));
            
            form.AddButtonRow(
                new ButtonBase("О Помогаторе", new CallbackData("a", "about").Serialize()),
                new ButtonBase("Заметки", new CallbackData("a", "notes").Serialize()),
                new ButtonBase("События", new CallbackData("a", "events").Serialize()));

            await Device.Send("Выберите один из пунктов ниже", form);
        }
    }
}