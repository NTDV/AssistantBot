using System.Linq;
using System.Threading.Tasks;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace PersonalBot.Views.Events
{
    public class AllUserEvents : AutoCleanForm
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
                    await NavigateTo(new EventsMenu());
                    break;
                case "look":
                    var @event = await PersonalBot.Database.GetEvent(long.Parse(query.Last()));
                    await NavigateTo(new ViewEvent(@event));
                    break;
                default:
                    return;
            }
        }

        public override async Task Render(MessageResult message)
        {
            var events = PersonalBot.Database.GetAllEvents(Device.DeviceId);
            foreach (var @event in events)
            {
                ButtonForm edit = new ButtonForm();
                edit.AddButtonRow(new ButtonBase("⬆️ Подробнее", new CallbackData("a", $"look-{@event.Id}").Serialize()));

                await Device.Send(@event.ToString(), edit);
            }

            var form = new ButtonForm();
            form.AddButtonRow(new ButtonBase("◀️ Назад", new CallbackData("a", "back").Serialize()));

            await Device.Send($"🕜 Всего напоминаний: {events.Length}", form);
        }
    }
}