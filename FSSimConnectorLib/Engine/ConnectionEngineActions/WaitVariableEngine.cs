using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FSSimConnectorLib
{
    internal class WaitVariableEngine
    {
        internal string variableName { get; set; }
        internal int thresholdValue { get; set; }
        internal char comparison { get; set; }
        private int currentValue { get; set; }

        private bool isValueReached { get; set; }

        private FSSimConnector connector;



        internal async Task WaitVariable(WaitVariableEngine waitVariable, FSSimConnector connector)
        {
            this.connector = connector;
            Console.WriteLine("Waiting until {0} {1} {2}",variableName,comparison, thresholdValue.ToString());
            
            this.connector.VariableHasBeenRecovered += sim_VariableRecovered;
            this.connector.RequestVariable(variableName);
            await new Helpers().WaitWhile(ValueIsNotReached);
            Console.WriteLine("Reached {0} {1} {2}", variableName, comparison, thresholdValue.ToString());
            connector.VariableHasBeenRecovered -= sim_VariableRecovered;
        }

        private bool ValueIsNotReached()
        {
            return !isValueReached;
        }

        private void sim_VariableRecovered(object sender, RecoveredVariable e)
        {
            Console.WriteLine("here");
            currentValue = Convert.ToInt32(e.Value);

            bool result = false;

            switch (comparison)
            {
                case '<':
                    result = currentValue < thresholdValue;
                    break;
                case '>':
                    result = currentValue > thresholdValue;
                    break;
                case '=':
                    result = currentValue == thresholdValue;
                    break;
            }

            isValueReached = result;

            if (!isValueReached)
            {
                connector.RequestVariable(variableName);
            }
        }
    }
}
