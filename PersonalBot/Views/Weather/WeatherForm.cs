using System.Threading.Tasks;
using PersonalBot.Controllers;
using TelegramBotBase.Base;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;

namespace PersonalBot.Views.Weather
{
    public class WeatherForm : AutoCleanForm
    {
        private readonly WeatherProvider _weather;
        
        public WeatherForm()
        {
            DeleteMode = eDeleteMode.OnLeavingForm;
            _weather = new WeatherProvider(PersonalBot.Settings);
            
            Init += async (_,_) => 
                await Device.RequestLocation("Моё местоположение", "Запрос на отправку местоположения нужен, чтобы показать прогноз погоды для вашей местности");
            
            Closed += async (_,_) => 
                await Device.HideReplyKeyboard();
        }

        public override async Task SentData(DataResult message)
        {
            if (message.Location != null)
            {
                var weather = await _weather.GetWeather(message.Location);
                await Device.Send(weather);
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
                case "back":
                    await NavigateTo(new StartForm());
                    break;
                
                default:
                    return;
            }
        }

        public override async Task Render(MessageResult message)
        {
            var city = message.MessageText?.Trim() ?? "";
            if (city != "")
            {
                var weather = await _weather.GetWeather(city);
                await Device.Send(weather);
            }
            
            ButtonForm form = new ButtonForm();
            form.AddButtonRow(new ButtonBase("◀️ Назад", new CallbackData("a", "back").Serialize()));

            await Device.Send("Для получения прогноза погоды отправьте название города или своё местоположение", form);
        }
    }
}