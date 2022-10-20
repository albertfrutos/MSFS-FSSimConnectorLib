using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSSimConnectorLib.LibConfiguration
{
    public class Configuration
    {
        public VariablesConfig variablesConfig { get; set; }
        public EventsConfig eventsConfig { get; set; }

        public Configuration LoadConfiguration()
        {
            var configText = File.ReadAllText(@"Configuration\config.json");
            Configuration config = JsonConvert.DeserializeObject<Configuration>(configText);
            return config;
        }
    }
    public class VariablesConfig
    {
        public string variablesFile { get; set; }
        public List<string> variablesURLs { get; set; }

    }
    public class EventsConfig
    {
        public string eventsFile { get; set; }
        public string enumEventsFile { get; set; }
        public List<string> eventsURLs { get; set; }
    }


}
