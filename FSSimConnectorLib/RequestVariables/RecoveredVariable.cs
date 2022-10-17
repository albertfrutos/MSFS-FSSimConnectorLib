using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSSimConnectorLib
{
    public class RecoveredVariable : EventArgs
    {
        public Variable Variable { get; set; }
        public string Value { get; set; }

        public RecoveredVariable (Variable variable, string value)
        {
            Variable = variable;
            Value = value;
        }
    }
}
