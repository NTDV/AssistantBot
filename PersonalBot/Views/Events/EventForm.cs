using System.Threading.Tasks;
using PersonalBot.Data.Models;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace PersonalBot.Views
{
    public class EventForm : AutoCleanForm
    {
        private Event _event;
        
        public EventForm(Event @event)
        {
            _event = @event;
        }
        
        public override async Task Action(MessageResult message)
        { 
            var call = message.GetData<CallbackData>();
            await message.ConfirmAction();

            if (call == null)
                return;

            switch (call.Value)
            {
                case "delete":
                    await PersonalBot.Database.RemoveEvent(_event);
                    await NavigateTo(new EventsMenuForm());
                    break;
                case "back":
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
                new ButtonBase("Вернуться", new CallbackData("a", "back").Serialize()),
                new ButtonBase("Удалить", new CallbackData("a", "delete").Serialize()));

            await Device.Send(_event.ToString(), form);
        }
    }
}