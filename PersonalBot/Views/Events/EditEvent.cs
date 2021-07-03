using System;
using System.Globalization;
using System.Threading.Tasks;
using PersonalBot.Data.Models;
using PersonalBot.Views.Components;
using TelegramBotBase.Base;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;

namespace PersonalBot.Views.Events
{
    public class EditEvent : AutoCleanForm
    {
        private readonly Event _edited = new();
        private readonly Event _old;

        private readonly SafeCalendarPicker _datePicker = new () {Title = "Укажите дату события" };
        private TimeSpan? _time;
        private int? _renderMessageId;
        
        public EditEvent(Event old)
        {
            DeleteMode = eDeleteMode.OnLeavingForm;

            _old = old;

            Init += async (_, _) => await Device.Send(
                $"Редактирование напоминания\nЗаголовок: {_old.Title}\nМесто и время: {_old.Place}\nДата уведомления: {_old.Time:F}\nПодробнее: {_old.Info}");
        }

        public override async Task Load(MessageResult message)
        {
            if (_edited.Title == null)
            {
                if (string.IsNullOrWhiteSpace(message.MessageText))
                    return;

                _edited.Title = message.MessageText;
                return;
            }

            if (_edited.Place == null)
            {
                _edited.Place = message.MessageText;
                return;
            }

            if (_edited.Info == null)
            {
                _edited.Info = message.MessageText;
                return;
            }

            if (_time == null)
            {
                if (!TimeSpan.TryParseExact(message.MessageText, new[] {"h\\:m", "h\\:mm", "hh\\:m", "hh\\:mm"},
                    CultureInfo.InvariantCulture, out var time))
                    return;

                _time = time;
                AddControl(_datePicker);
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
                case "confirm":
                    await PersonalBot.Database.EditEvent(_old, _edited);
                    await NavigateTo(new EventsMenu());
                    break;
                case "back":
                    await NavigateTo(new EventsMenu());
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

            if (_edited.Place == null)
            {
                await Device.Send("🎫 Укажите информацию о месте и времени: ");
                return;
            }
            
            if (_edited.Info == null)
            {
                await Device.Send("➕ Укажите дополнительную информацию: ");
                return;
            }
            
            if (_time == null)
            {
                await Device.Send("🕜 Укажите время: ");
                return;
            }

            _edited.Time = _datePicker.SelectedDate.Add(_time.Value);

            var buttons = new ButtonForm();
            buttons.AddButtonRow(new ButtonBase("❌ Отмена", new CallbackData("a", "cancel").Serialize()));
            buttons.AddButtonRow(new ButtonBase("✅ Изменить", new CallbackData("a", "confirm").Serialize()));

            var ret = $"Заголовок: {_edited.Title}\nМесто и время: {_edited.Place}\nДата уведомления: {_edited.Time:F}\nПодробнее: {_edited.Info}";
            
            if (_renderMessageId != null)
            {
                await Device.Edit(_renderMessageId.Value, ret, buttons);
            }
            else
            {
                var m = await Device.Send(ret, buttons);
                _renderMessageId = m.MessageId;
            }
        }
    }
}