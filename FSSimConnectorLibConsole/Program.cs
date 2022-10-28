using FSSimConnectorLib;
using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSSimConnectorLibConsole
{
    internal class Program
    {
        static string varValue = "0";
        static void Main(string[] args)
        {


            FSSimConnector sim = new FSSimConnector();
            FSSimConnectorEngine engine = new FSSimConnectorEngine();
            //sim.Initialize();
            /*
            sim.Initialize();
            sim.UpdateVariables(false);
            sim.UpdateVariables();
            sim.UpdateEvents();
            */

            

            if (engine.Initialize())
            {

                
                engine.connector.VariableHasBeenRecovered += sim_VariableHasBeenRecovered;
                engine.connector.EventHasBeenSent += sim_EventHasBeenSent;

                engine.AddAutomation(new TakeOff()
                {
                    gearUpAltitude = 350,
                    targetAltitude = 500,
                    climbRate = 500
                });

                //engine.AddVariableRequest("AUTOPILOT HEADING LOCK DIR");


                engine.LaunchActions();


                /*
                engine.AddVariableRequest("AUTOPILOT HEADING LOCK DIR");
                engine.WaitMillis(4000);
                engine.AddVariableRequest("AUTOPILOT MASTER");
                engine.AddVariableRequest("ATC ID");
                engine.AddVariableRequest(new List<string> { "HSI DISTANCE", "AUTOPILOT ALTITUDE LOCK VAR" });
                engine.AddSendEvent("HEADING_BUG_SET", 50);
                engine.AddVariableRequest("AUTOPILOT HEADING LOCK DIR");
                engine.LaunchActions();
                */


                /*
                engine.AddSendEvent("HEADING_BUG_SET", 126);
                engine.WaitMillis(10000);
                engine.AddSendEvent("HEADING_BUG_SET", 216);
                engine.WaitMillis(10000);
                engine.AddSendEvent("HEADING_BUG_SET", 306);
                engine.WaitMillis(10000);
                engine.AddSendEvent("HEADING_BUG_SET", 36);
                engine.WaitMillis(10000);
                engine.AddSendEvent("HEADING_BUG_SET", 120);
                engine.WaitMillis(10000);
                engine.AddSendEvent("HEADING_BUG_SET", 126);
                engine.LaunchActions();
                */




            }



            /*
            sim.Initialize();

            if (sim.isConnected())
            {
                
                sim.VariableHasBeenRecovered += sim_VariableHasBeenRecovered;
                sim.EventHasBeenSent += sim_EventHasBeenSent;

                sim.RequestVariable("AUTOPILOT HEADING LOCK DIR");
                sim.RequestVariable("AUTOPILOT MASTER");
                sim.RequestVariable("ATC ID");
                sim.RequestVariable(new List<string> { "HSI DISTANCE" });
                sim.RequestVariable(new List<string> { "AUTOPILOT ALTITUDE LOCK VAR" });
                sim.SendEvent("HEADING_BUG_SET", 50, true, "AUTOPILOT HEADING LOCK DIR");
                sim.RequestVariable("ATC ID");
                

            }
            else
            {
                Console.WriteLine("An error ocurred while connecting to the simulator");
            }
            
                */

            while (true)
            {

            }
        }

        private static void sim_EventHasBeenSent(object sender, SentEvent e)
        {
            Console.WriteLine("Sent event is: \t {0} \t\t\t with value: {1}", e.Name, e.Value);
        }

        public static void sim_VariableHasBeenRecovered(object sender, RecoveredVariable e)
        {
            Console.WriteLine("Recovered variable is:\t {0} \t\t\t with value:{1}", e.Variable.name, e.Value);
            varValue = e.Value;
        }






    }
}
