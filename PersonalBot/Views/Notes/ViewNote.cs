using System.Threading.Tasks;
using PersonalBot.Data.Models;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace PersonalBot.Views.Notes
{
    public class ViewNote : AutoCleanForm
    {
        private readonly Note _note;
        
        public ViewNote(Note note)
        {
            _note = note;
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
                    await NavigateTo(new EditNote(_note));
                    break;
                case "delete":
                    await PersonalBot.Database.RemoveNote(_note);
                    await NavigateTo(new NotesMenu());
                    break;
                case "back":
                    await NavigateTo(new NotesMenu());
                    break;
                default:
                    return;
            }
        }


        public override async Task Render(MessageResult message)
        {
            ButtonForm form = new ButtonForm();

            form.AddButtonRow(
                new ButtonBase("◀️ Назад", new CallbackData("a", "back").Serialize()),
                new ButtonBase("🗑️ Удалить", new CallbackData("a", "delete").Serialize()),
                new ButtonBase("Изменить", new CallbackData("a", "edit").Serialize()));

            await Device.Send(_note.ToString(), form);
        }
    }
}