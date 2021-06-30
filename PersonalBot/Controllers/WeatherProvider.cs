using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PersonalBot.Resources.Providers.Declarations;
using Telegram.Bot.Types;

namespace PersonalBot.Controllers
{
    public class WeatherProvider
    {
        private readonly ISettingsProvider _settings;

        public WeatherProvider(ISettingsProvider settingsProvider)
        {
            _settings = settingsProvider;
        }

        private async Task<string> CallApi(string url)
        {
            Console.WriteLine(url);
            var client = new HttpClient();
            var content = await client.GetStringAsync(url);
            dynamic json = JObject.Parse(content);

            string lastUpdated = Convert.ToString(json.current.last_updated),
                region = Convert.ToString(json.location.region),
                condition = Convert.ToString(json.current.condition.text).ToLower(),
                temp = Convert.ToString(json.current.temp_c),
                feelslike = Convert.ToString(json.current.feelslike_c),
                wind = Convert.ToString(json.current.wind_kph),
                gust = Convert.ToString(json.current.gust_kph),
                windDir = Convert.ToString(json.current.wind_dir).Replace('N', 'С').Replace('E', 'В').Replace('S', 'Ю').Replace('W', 'З'),
                pressure = (Convert.ToDouble(json.current.pressure_mb) * 1.333223874D).ToString("F1"),
                humidity = Convert.ToString(json.current.humidity),
                precip = Convert.ToString(json.current.precip_mm),
                vis = Convert.ToString(json.current.vis_km),
                uv = Convert.ToString(json.current.uv);
                

            return (
$@"На {lastUpdated} в {region} {condition}
Температура: {temp} °C
Ощущается как: {feelslike} °C
Скорость ветра: {wind} км/ч
Порывы ветра: {gust} км/ч
Направление ветра: {windDir}
Атмосферное давление: {pressure} мм.р.ст.
Влажность: {humidity}%
Количество осадков: {precip} мм
Видимость: {vis} км
УФ-индекс: {uv}
");
        }
        
        public async Task<string> GetWeather(string city)
        {
            return await CallApi(string.Format(_settings["weather_api_mask"], _settings["weather_api_token"], city));
        }

        public async Task<string> GetWeather(Location location)
        {
            return await GetWeather($"{location.Latitude.ToString().Replace(',', '.')},{location.Longitude.ToString().Replace(',', '.')}");
        }
    }
}