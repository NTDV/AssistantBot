using System.Linq;
using System.Threading.Tasks;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace PersonalBot.Views.Events
{
    public class AllEventsForm : AutoCleanForm
    {
        public override async Task Action(MessageResult message)
        {
            var call = message.GetData<CallbackData>();
            await message.ConfirmAction();

            if (call == null)
                return;

            var query = call.Value.Split('-');

            switch (query.FirstOrDefault())
            {
                case "back":
                    await NavigateTo(new EventsMenuForm());
                    break;
                case "look":
                    await NavigateTo(new EventForm(PersonalBot.Database.GetEvent(long.Parse(query.Last()))));
                    break;
                default:
                    return;
            }
        }

        public override async Task Render(MessageResult message)
        {
            var events = PersonalBot.Database.GetAllEvents();
            foreach (var @event in events)
            {
                ButtonForm edit = new ButtonForm();
                edit.AddButtonRow(new ButtonBase("Подробнее", new CallbackData("a", $"look-{@event.Id}").Serialize()));

                await Device.Send(@event.ToString(), edit);
            }

            var form = new ButtonForm();
            form.AddButtonRow(new ButtonBase("Назад", new CallbackData("a", "back").Serialize()));

            await Device.Send($"Всего напоминаний: {events.Length}", form);
        }
    }
}