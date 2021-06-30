using System.Threading.Tasks;
using PersonalBot.Data.Models;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace PersonalBot.Views
{
    public class NoteForm : AutoCleanForm
    {
        private Note _note;
        
        public NoteForm(Note note)
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
                case "delete":
                    await PersonalBot.Database.RemoveNote(_note);
                    await NavigateTo(new NotesMenuForm());
                    break;
                case "back":
                    await NavigateTo(new NotesMenuForm());
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

            await Device.Send(_note.ToString(), form);
        }
    }
}