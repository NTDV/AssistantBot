using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PersonalBot.Resources.Providers.Declarations;

namespace PersonalBot.Controllers
{
    public class CovidStatsProvider
    {
        private readonly ISettingsProvider _settings;

        public CovidStatsProvider(ISettingsProvider settingsProvider)
        {
            _settings = settingsProvider;
        }

        private async Task<string> CallApi(string url)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_settings["covid_api_url"]),
                Headers =
                {
                    { "x-rapidapi-key", _settings["covid_api_token"] },
                    { "x-rapidapi-host", _settings["covid_host"] },
                },
            };
            
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                
                var body = await response.Content.ReadAsStringAsync();
                dynamic json = JObject.Parse(body);

                string name = "России",
                    population = Convert.ToString(json.response[0].population),

                    newCases = Convert.ToString(json.response[0].cases.@new),
                    activeCases = Convert.ToString(json.response[0].cases.active),
                    criticalCases = Convert.ToString(json.response[0].cases.critical),
                    recoveredCases = Convert.ToString(json.response[0].cases.recovered),
                    totalCases = Convert.ToString(json.response[0].cases.total),

                    newDeaths = Convert.ToString(json.response[0].deaths.@new),
                    totalDeaths = Convert.ToString(json.response[0].deaths.total),

                    day = Convert.ToString(json.response[0].day);

                return
                    $@"По состоянию на {day} в {name}
Всего населения: {population}
Новых заражений: {newCases}
Сейчас заражаено: {activeCases}
В тяжелом состоянии: {criticalCases}
Выздоровело: {recoveredCases}
Всего заражений: {totalCases}
Новых смертей: {newDeaths}
Всего погибло: {totalDeaths}";
            }
        }
        
        public async Task<string> GetStatistic(string city)
        {
            return await CallApi(null);
        }
    }
}