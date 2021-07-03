using System.Threading.Tasks;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace PersonalBot.Views.Notes
{
    public class NotesMenu : AutoCleanForm
    {
        public override async Task Action(MessageResult message)
        {
            var call = message.GetData<CallbackData>();
            await message.ConfirmAction();

            if (call == null)
                return;

            switch (call.Value)
            {
                case "add":
                    await NavigateTo(new NewNote());
                    break;
                case "back":
                    await NavigateTo(new Start());
                    break;
                case "all":
                    await NavigateTo(new AllUserNotes());
                    break;
                
                default:
                    return;
            }
        }

        public override async Task Render(MessageResult message)
        {
            ButtonForm form = new ButtonForm();

            form.AddButtonRow(new ButtonBase("🆕 Добавить", new CallbackData("a", "add").Serialize()));
            form.AddButtonRow(
                new ButtonBase("◀️ Назад", new CallbackData("a", "back").Serialize()),
                new ButtonBase("⏳ Все", new CallbackData("a", "all").Serialize()));

            await Device.Send("⚙️ Управление заметками", form);
        }
    }
}