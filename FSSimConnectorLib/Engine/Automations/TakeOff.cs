using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSSimConnectorLib
{
    public class TakeOff
    {
        public int climbRate { get; set; }
        public int gearUpAltitude { get; set;}
        public int targetAltitude { get; set; }
        public bool onlyAvailableOnGround { get; set; } = false;

        public void Load(FSSimConnectorEngine engine, FlightModel flightModel, object obj)
        {
            engine.ExpectVariableToBe("SIM ON GROUND", true, onlyAvailableOnGround);

            var gearUpAltitude = this.gearUpAltitude; // Convert.ToInt32(obj.GetType().GetProperty("gearUpAltitude").GetValue(obj));
            var targetAltitude = this.targetAltitude; // Convert.ToInt32(obj.GetType().GetProperty("targetAltitude").GetValue(obj));
            var climbRate = Convert.ToUInt32(this.climbRate); //Convert.ToUInt32(obj.GetType().GetProperty("climbRate").GetValue(obj)) > 0 ? Convert.ToUInt32(obj.GetType().GetProperty("climbRate").GetValue(obj)) : Convert.ToUInt32(flightModel.ReferenceSpeeds.ClimbSpeed);

            //Get current heading and set it into the AP
            engine.AddVariableRequest("PLANE HEADING DEGREES GYRO");
            engine.AddSendEvent("HEADING_BUG_SET", 0, true);
           
            //AP on
            engine.AddSendEvent("AP_MASTER", 1);

            //FD on
            engine.AddSendEvent("TOGGLE_FLIGHT_DIRECTOR", 1);

            //set VS on and VS 300ft/min
            engine.AddSendEvent("AP_PANEL_VS_HOLD", 1);
            engine.AddSendEvent("AP_VS_VAR_SET_ENGLISH", climbRate); //Convert.ToUInt32(obj.GetType().GetProperty("climbRate").GetValue(obj)));

            //PARKING BREAKS off
            engine.AddSendEvent("PARKING_BRAKE_SET", 0);

            //full engine
            engine.AddSendEvent("THROTTLE_FULL", 0);

            engine.WaitUntilVariableIsHigher("PLANE ALTITUDE", gearUpAltitude);
            engine.AddSendEvent("GEAR_UP", 0);
            engine.WaitUntilVariableIsHigher("PLANE ALTITUDE", targetAltitude);

            //set VS to 0
            engine.AddSendEvent("AP_VS_VAR_SET_ENGLISH", climbRate); //Convert.ToUInt32(obj.GetType().GetProperty("climbRate").GetValue(obj)));


            engine.AddSendEvent("AP_ALT_VAR_SET_ENGLISH", Convert.ToUInt32(targetAltitude));
            engine.AddSendEvent("AP_ALT_HOLD", 1);
            Console.WriteLine(targetAltitude);
        }
    }
}
