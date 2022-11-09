using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FSSimConnectorLib
{
    internal class Automation
    {
        internal async Task
        AddAutomation(FSSimConnectorEngine engine, object obj)
        {
            await new Helpers().WaitWhile(engine.connector.IsFlightModelNotInitialized);

            FlightModel flightModel = (FlightModel)engine.connector.flightModel;

            

            Task automationLoadMethod =  (Task)obj.GetType().GetMethod("LoadAutomation").Invoke(obj, new object[] { engine, flightModel, obj });


            await automationLoadMethod;
        }
    }
}
