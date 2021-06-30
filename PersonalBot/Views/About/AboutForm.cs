using System.Threading.Tasks;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace PersonalBot.Views.About
{
    public class AboutForm : AutoCleanForm
    {
        public override async Task Action(MessageResult message)
        {
            var call = message.GetData<CallbackData>();
            await message.ConfirmAction();

            if (call == null)
                return;

            switch (call.Value)
            {
                case "back":
                    await NavigateTo(new StartForm());
                    break;
                
                default:
                    return;
            }
        }

        public override async Task Render(MessageResult message)
        {
            ButtonForm form = new ButtonForm();
            form.AddButtonRow(new ButtonBase("◀️ Назад", new CallbackData("a", "back").Serialize()));

            await Device.Send("⭐️ Что умеет бот?\n- Создавать, удалять, просматривать заметки\n- Создавать, просматривать и получать напоминания\n- Предоставлять сводку о погоде в разных уголках света\n- Выводить актуальную информацию о Covid-19 в России\n- Делать вашу жизнь проще ;)\n\nGitHub проекта - https://github.com/NTDV/AssistantBot", form);
        }
    }
}