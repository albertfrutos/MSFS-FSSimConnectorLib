using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSSimConnectorLib
{
    public class Configuration
    {
        public VariablesConfig variablesConfig { get; set; }
        public EventsConfig eventsConfig { get; set; }

        public Paths paths  { get; set; }

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

    public class Paths
    {
        public string baseFSPathOfficial { get; set; }
        public string baseFSPathCommunity { get; set; }
    }


}
