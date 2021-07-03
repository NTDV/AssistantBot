using System.Threading.Tasks;
using PersonalBot.Data.Models;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace PersonalBot.Views.Events
{
    public class ViewEvent : AutoCleanForm
    {
        private readonly Event _event;
        
        public ViewEvent(Event @event)
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
                case "edit":
                    await NavigateTo(new EditEvent(_event));
                    break;
                case "delete":
                    await PersonalBot.Database.RemoveEvent(_event);
                    await NavigateTo(new EventsMenu());
                    break;
                case "back":
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
                new ButtonBase("◀️Назад", new CallbackData("a", "back").Serialize()),
                new ButtonBase("🗑️ Удалить", new CallbackData("a", "delete").Serialize()),
                new ButtonBase("Изменить", new CallbackData("a", "edit").Serialize()));

            await Device.Send(_event.ToString(), form);
        }
    }
}