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

            obj.GetType().GetMethod("Load").Invoke(obj, new object[] { engine, flightModel, obj });
            
        }


    }
}
