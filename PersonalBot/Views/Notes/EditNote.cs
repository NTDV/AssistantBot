using System.Threading.Tasks;
using PersonalBot.Data.Models;
using TelegramBotBase.Base;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;

namespace PersonalBot.Views.Notes
{
    public class EditNote : AutoCleanForm
    {
        private readonly Note _edited = new();
        private readonly Note _old;

        public EditNote(Note old)
        {
            DeleteMode = eDeleteMode.OnLeavingForm;

            _old = old;

            Init += async (_, _) => await Device.Send(
                $"Редактирование заметки\nЗаголовок: {_old.Title}\nСодержимое: {_old.Description}");
        }
        
        public override Task Load(MessageResult message)
        {
            if (_edited.Title == null)
            {
                if (string.IsNullOrWhiteSpace(message.MessageText))
                    return Task.CompletedTask;

                _edited.Title = message.MessageText;
                return Task.CompletedTask;
            }

            _edited.Description ??= message.MessageText;
            
            return Task.CompletedTask;
        }
        
        public override async Task Action(MessageResult message)
        {
            var call = message.GetData<CallbackData>();
            await message.ConfirmAction();

            if (call == null)
                return;

            switch (call.Value)
            {
                case "confirm":
                    await PersonalBot.Database.EditNote(_old, _edited);
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
            if (_edited.Title == null)
            {
                await Device.Send("📍 Укажите заголовок: ");
                return;
            }

            if (_edited.Description == null)
            {
                await Device.Send("➕ Укажите дополнительную информацию: ");
                return;
            }

            var buttons = new ButtonForm();
            buttons.AddButtonRow(new ButtonBase("❌ Отмена", new CallbackData("a", "cancel").Serialize()));
            buttons.AddButtonRow(new ButtonBase("✅ Изменить", new CallbackData("a", "confirm").Serialize()));

            var ret = $"Заголовок: {_edited.Title}\nПодробнее: {_edited.Description}";
            
            await Device.Send(ret, buttons);
        }
    }
}