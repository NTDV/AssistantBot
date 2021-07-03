using System.Threading.Tasks;
using PersonalBot.Controllers;
using TelegramBotBase.Base;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;

namespace PersonalBot.Views.Covid
{
    public class Covid : AutoCleanForm
    {
        private readonly CovidStatsProvider _covidStats;
        
        public Covid()
        {
            DeleteMode = eDeleteMode.OnLeavingForm;
            _covidStats = new CovidStatsProvider(PersonalBot.Settings);
        }

        public override async Task Action(MessageResult message)
        {
            var call = message.GetData<CallbackData>();
            await message.ConfirmAction();

            if (call == null)
                return;

            switch (call.Value)
            {
                case "back":
                    await NavigateTo(new Start());
                    break;
                
                default:
                    return;
            }
        }

        public override async Task Render(MessageResult message)
        {
            await Device.Send(await _covidStats.GetStatistic(null));
            
            ButtonForm form = new ButtonForm();
            form.AddButtonRow(new ButtonBase("◀️ Назад", new CallbackData("a", "back").Serialize()));

            await Device.Send("☣️ Самое важное, что можно сделать, чтобы защитить себя, — это поддерживать чистоту рук и поверхностей. Конечно, не стоит забывать и про ношение маски!", form);
        }
    }
}