using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using PersonalBot.Resources.Providers.Declarations;

namespace PersonalBot.Resources.Providers.Implementations
{
    public class UpdatableSettingsProvider : ISettingsProvider
    {
        private readonly string _path;
        
        public UpdatableSettingsProvider(string path)
        {
            _path = path;
        }
        
        public string this[string index] => 
            JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(_path))[index];
    }
}