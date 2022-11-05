using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSSimConnectorLib
{
    public class SentEvent : EventArgs
    {
        public string Name { get; set; }
        public uint Id { get; set; }
        public string Value { get; set; }

        public SentEvent(string eventName, uint id, string value)
        {
            Name = eventName;
            Id = id;
            Value = value;
        }
    }
}
