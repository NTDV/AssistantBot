using System.Linq;
using System.Threading.Tasks;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace PersonalBot.Views.Notes
{
    public class AllNotesForm : AutoCleanForm
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
                    await NavigateTo(new NotesMenuForm());
                    break;
                case "look":
                    await NavigateTo(new NoteForm(PersonalBot.Database.GetNote(long.Parse(query.Last()))));
                    break;
                default:
                    return;
            }
        }

        public override async Task Render(MessageResult message)
        {
            var notes = PersonalBot.Database.GetAllNotes();
            foreach (var note in notes)
            {
                ButtonForm edit = new ButtonForm();
                edit.AddButtonRow(new ButtonBase("Подробнее", new CallbackData("a", $"look-{note.Id}").Serialize()));
                
                await Device.Send(note.Title, edit);
            }

            var form = new ButtonForm();
            form.AddButtonRow(new ButtonBase("Назад", new CallbackData("a", "back").Serialize()));

            await Device.Send($"Всего заметок: {notes.Length}", form);
        }
    }
}