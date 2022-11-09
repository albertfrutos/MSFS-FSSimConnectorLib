using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSSimConnectorLib
{
    internal interface IAutomation
    {
        Task LoadAutomation(FSSimConnectorEngine engine, FlightModel flightModel, object obj);
    }
}
