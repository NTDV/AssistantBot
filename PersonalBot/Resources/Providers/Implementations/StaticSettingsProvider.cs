using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using PersonalBot.Resources.Providers.Declarations;

namespace PersonalBot.Resources.Providers.Implementations
{
    public class StaticSettingsProvider : ISettingsProvider
    {
        private readonly Dictionary<string, string> _settings;
        
        public StaticSettingsProvider(string path)
        {
            _settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));
        }

        public string this[string index] => _settings[index]; 
    }
}