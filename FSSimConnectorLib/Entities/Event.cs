using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace FSSimConnectorLib
{
    public class Event
    {
        public string name { get; set; }
        public string unit { get; set; }
        public string type { get; set; }
        public string parameters { get; set; }
        public string description { get; set; }
        public string group { get; set; }

    }
}
