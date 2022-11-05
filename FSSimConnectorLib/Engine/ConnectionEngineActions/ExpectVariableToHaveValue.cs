using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FSSimConnectorLib
{
    internal class ExpectVariableToHaveValueEngine
    {
        internal string variableName { get; set; }
        internal string expectedValue { get; set; }
        internal bool stopActionsIfUnexpectedValue { get; set; }

        private FSSimConnector connector;

        private bool isVariableRecovered = false;

        private bool hasVariableExpectedValue = false;




        internal async Task<bool> ExpectVariableToHaveValue(ExpectVariableToHaveValueEngine variable, FSSimConnector connector)
        {
            this.connector = connector;
            this.connector.VariableHasBeenRecovered += sim_VariableRecovered;
            this.connector.RequestVariable(variableName);
            await new Helpers().WaitWhile(VariableIsNotRecovered);
            connector.VariableHasBeenRecovered -= sim_VariableRecovered;
            return hasVariableExpectedValue;
        }

        private bool VariableIsNotRecovered()
        {
            return !isVariableRecovered;
        }

        private void sim_VariableRecovered(object sender, RecoveredVariable e)
        {
            hasVariableExpectedValue = expectedValue == e.Value;
            isVariableRecovered = true;
        }
    }
}
