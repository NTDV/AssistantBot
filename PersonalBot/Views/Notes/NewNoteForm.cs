using System;
using System.Globalization;
using System.Threading.Tasks;
using PersonalBot.Controllers;
using PersonalBot.Data.Models;
using PersonalBot.Views.Components;
using TelegramBotBase.Args;
using TelegramBotBase.Base;
using TelegramBotBase.Controls.Inline;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;
#pragma warning disable 1998

namespace PersonalBot.Views
{
    public class NewNoteForm : AutoCleanForm
    {
        public Note Note => new()
            {ChatId = Device.DeviceId, Title = Title, Description = Description};
        
        private string Title { get; set; }
        private string Description { get; set; }

        public NewNoteForm()
        {
            DeleteMode = eDeleteMode.OnLeavingForm;
        }

        public override async Task Load(MessageResult message)
        {
            if (Title == null)
            {
                if (message.MessageText.Trim() == "")
                    return;
                
                Title = message.MessageText;
                return;
            }

            if (Description == null)
            {
                Description = message.MessageText;
                return;
            }
        }

        public override async Task Action(MessageResult message)
        {
            var call = message.GetData<CallbackData>();
            await message.ConfirmAction();

            if (call == null)
                return;

            switch (call.Value)
            {
                case "create":
                    await PersonalBot.Database.AddNoteAsync(Note);
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
            if (Title == null)
            {
                await Device.Send("Укажите заголовок: ");
                return;
            }
            if (Description == null)
            {
                await Device.Send("Укажите дополнительную информацию: ");
                return;
            }

            var buttons = new ButtonForm();
            buttons.AddButtonRow(new ButtonBase("Отмена", new CallbackData("a", "cancel").Serialize()));
            buttons.AddButtonRow(new ButtonBase("Создать", new CallbackData("a", "create").Serialize()));

            var ret = $"Заголовок: {Title}\nПодробнее: {Description}";
            
            await Device.Send(ret, buttons);
        }
    }
}