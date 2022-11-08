using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSSimConnectorLib
{
    public class TakeOff : IAutomation
    {
        public int climbRate { get; set; }
        public int gearUpAltitudeFromGround { get; set;}
        public int targetAltitude { get; set; }
        public bool onlyAvailableOnGround { get; set; } = false;

        public async Task LoadAutomation(FSSimConnectorEngine engine, FlightModel flightModel, object obj)
        {
            #region before

            await engine.AddVariableRequest("PLANE ALTITUDE");
            engine.connector.lockEngine = false;
            await engine.LaunchActions(true);
            engine.connector.lockEngine = true;
            var currentPlaneAltitude = Convert.ToInt32(engine.actionExecuter.lastVariableRequestValue);

            #endregion

            #region automation

            engine.ExpectVariableToBe("SIM ON GROUND", true, onlyAvailableOnGround);

            var gearUpAltitude = this.gearUpAltitudeFromGround + currentPlaneAltitude;
            var targetAltitude = this.targetAltitude;
            var climbRate = Convert.ToUInt32(this.climbRate);

            //Get current heading and set it into the AP
            await engine.AddVariableRequest("PLANE HEADING DEGREES GYRO");
            await engine.AddSendEvent("HEADING_BUG_SET", 0, true);

            //AP on
            await engine.AddSendEvent("AP_MASTER", 1);

            //FD on
            await engine.AddSendEvent("TOGGLE_FLIGHT_DIRECTOR", 1);

            //set VS on and VS
            await engine.AddSendEvent("AP_PANEL_VS_HOLD", 1);
            await engine.AddSendEvent("AP_VS_VAR_SET_ENGLISH", climbRate);

            //PARKING BREAKS off
            await engine.AddSendEvent("PARKING_BRAKE_SET", 0);

            //full engine
            await engine.AddSendEvent("THROTTLE_FULL", 0);

            await engine.WaitUntilVariableIsHigher("PLANE ALTITUDE", gearUpAltitude);
            await engine.AddSendEvent("GEAR_UP", 0);
            await engine.WaitUntilVariableIsHigher("PLANE ALTITUDE", targetAltitude);

            //set VS to 0
            await engine.AddSendEvent("AP_VS_VAR_SET_ENGLISH", climbRate);


            await engine.AddSendEvent("AP_ALT_VAR_SET_ENGLISH", Convert.ToUInt32(targetAltitude));
            await engine.AddSendEvent("AP_ALT_HOLD", 1);

            #endregion
        }
    }
}
