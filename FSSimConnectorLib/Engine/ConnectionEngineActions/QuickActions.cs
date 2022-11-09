using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSSimConnectorLib
{
    public class QuickActions
    {
        internal FSSimConnectorEngine engine;
        public QuickActions(FSSimConnectorEngine engine)
        {
            this.engine = engine;
        }

        public async Task SetAPHeading(uint value)
        {
            await engine.AddSendEvent("HEADING_BUG_SET", value);
        }

        public async Task SetAPVerticalSpeed(uint value)
        {
            await engine.AddSendEvent("AP_VS_VAR_SET_ENGLISH", value);
        }
        
        public async Task EnableAP()
        {
            if(GetVariable("AUTOPILOT MASTER").Result.ToString() == "False")
            {
                await ToggleAP();
            }
        }
        
        public async Task DisableAP()
        {
            if(GetVariable("AUTOPILOT MASTER").Result.ToString() == "True")
            {
                await ToggleAP();
            }
        }

        public async Task ToggleAP()
        {
            await engine.AddSendEvent("AP_MASTER",  1);
        }
        
        public async Task ToggleFD()
        {
            await engine.AddSendEvent("TOGGLE_FLIGHT_DIRECTOR", 1);
        }

        private async Task<string> GetVariable(string variable)
        {
            await engine.AddVariableRequest(variable);
            await engine.LaunchActions(true);
            return engine.actionExecuter.lastVariableRequestValue;

        }
    }
}
