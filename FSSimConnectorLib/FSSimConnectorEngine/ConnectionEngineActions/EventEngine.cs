using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSSimConnectorLib
{
    public class EventEngine
    {
        public string Name { get; set; }
        public uint Value { get; set; }
        public bool UseLastVariableValue { get; set; } = false;

    }
}
