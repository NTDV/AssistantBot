using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PersonalBot.Controllers;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace PersonalBot.Views
{
    public class StartForm : AutoCleanForm
    {
        public override async Task Action(MessageResult message)
        { 
            var call = message.GetData<CallbackData>();
            await message.ConfirmAction();

            if (call == null)
                return;

            switch (call.Value)
            {
                case "currency":
                case "corona":
                    return;
                case "notes":
                    await NavigateTo(new NotesMenuForm());
                    break;
                case "about":
                case "weather":
                    return;
                case "events":
                    await NavigateTo(new EventsMenuForm());
                    break;
                default:
                    return;
            }
        }


        public override async Task Render(MessageResult message)
        {
            ButtonForm form = new ButtonForm();

            form.AddButtonRow(
                new ButtonBase("Курс", new CallbackData("a", "currency").Serialize()),
                new ButtonBase("Коронавирус", new CallbackData("a", "corona").Serialize()),
                new ButtonBase("Заметки", new CallbackData("a", "notes").Serialize()));
            
            form.AddButtonRow(
                new ButtonBase("О Помогаторе", new CallbackData("a", "about").Serialize()),
                new ButtonBase("Погода", new CallbackData("a", "weather").Serialize()),
                new ButtonBase("События", new CallbackData("a", "events").Serialize()));

            await Device.Send("Выберите один из пунктов ниже", form);
        }
    }
}