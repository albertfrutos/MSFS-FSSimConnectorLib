using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSSimConnectorLib
{
    public class FlightModel
    {
        private string flightModelPath;
        public ReferenceSpeeds ReferenceSpeeds { get; set; }

        public FlightModel(string planeAirCraftCfgPath)
        {
            this.flightModelPath = planeAirCraftCfgPath;

            IniFile ini = new IniFile(flightModelPath);  //  https://stackoverflow.com/questions/217902/reading-writing-an-ini-file

            this.ReferenceSpeeds = new ReferenceSpeeds(ini);
        }

        public FlightModel()
        {

        }
    }
}
