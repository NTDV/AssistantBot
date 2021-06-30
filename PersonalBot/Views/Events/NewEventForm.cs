using System;
using System.Globalization;
using System.Threading.Tasks;
using PersonalBot.Data.Models;
using PersonalBot.Views.Components;
using TelegramBotBase.Base;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;

#pragma warning disable 1998

namespace PersonalBot.Views.Events
{
    public class NewEventForm : AutoCleanForm
    {
        public Event Event => new()
            {ChatId = Device.DeviceId, Title = Title, Place = Place, Time = DatePicker.SelectedDate.Add(_time), Info = Info};
        
        private SafeCalendarPicker DatePicker { get; set; }
        private string Time { get; set; }
        private string Title { get; set; }
        private string Place { get; set; }
        private string Info { get; set; }
        private int? RenderMessageId { get; set; }

        private TimeSpan _time;

        public NewEventForm()
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

            if (Place == null)
            {
                Place = message.MessageText;
                return;
            }
            if (Info == null)
            {
                Info = message.MessageText;
                return;
            }
            if (Time == null)
            {
                if (!TimeSpan.TryParseExact(message.MessageText, new[] {"h\\:m", "h\\:mm", "hh\\:m", "hh\\:mm"},
                    CultureInfo.InvariantCulture, out _time)) 
                    return;
                
                
                Time = message.MessageText;
                DatePicker = new SafeCalendarPicker {Title = "Укажите дату события" };
                AddControl(DatePicker);
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
                    await PersonalBot.Database.AddEventAsync(Event);
                    await NavigateTo(new EventsMenuForm());
                    break;
                case "back":
                    await NavigateTo(new EventsMenuForm());
                    break;
                default:
                    return;
            }
        }

        public override async Task Render(MessageResult message)
        {
            if (Title == null)
            {
                await Device.Send("📍 Укажите заголовок: ");
                return;
            }

            if (Place == null)
            {
                await Device.Send("🎫 Укажите информацию о месте и времени: ");
                return;
            }
            if (Info == null)
            {
                await Device.Send("➕ Укажите дополнительную информацию: ");
                return;
            }
            if (Time == null)
            {
                await Device.Send("🕜 Укажите время: ");
                return;
            }

            var buttons = new ButtonForm();
            buttons.AddButtonRow(new ButtonBase("❌ Отмена", new CallbackData("a", "cancel").Serialize()));
            buttons.AddButtonRow(new ButtonBase("✅ Создать", new CallbackData("a", "create").Serialize()));

            var ret = $"Заголовок: {Title}\nМесто и время: {Place}\nДата уведомления: {DatePicker.SelectedDate.Add(_time):F}\nПодробнее: {Info}";
            
            if (RenderMessageId != null)
            {
                await Device.Edit(RenderMessageId.Value, ret, buttons);
            }
            else
            {
                var m = await Device.Send(ret, buttons);
                RenderMessageId = m.MessageId;
            }
        }
    }
}