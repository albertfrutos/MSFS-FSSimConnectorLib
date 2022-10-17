using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSSimConnectorLib.SetEvents
{
    internal class SimulationEventSender : Entities
    {
        private static SimConnect Connection = null;

        FSSimConnector connector = null;

        List<string> mappedEvents = new List<string>();


        internal SimulationEventSender (SimConnect AppConnection, FSSimConnector fSSimConnector)
        {
            Connection = AppConnection;
            connector = fSSimConnector;
        }


        internal  void SendEvent(string eventName, uint eventValue)
        { 
            if (CheckIfEventExists(eventName))
            {
                EVENTS eventToSend = (EVENTS)Enum.Parse(typeof(EVENTS), eventName);
                if (!mappedEvents.Contains(eventName))
                {
                    Connection.MapClientEventToSimEvent((Enum)eventToSend, eventName);
                    mappedEvents.Add(eventName);
                }
                Connection.TransmitClientEvent(0U, (Enum)eventToSend, eventValue, (Enum)NOTIFICATION_GROUPS.GROUP0, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
                connector.OnEventSent(new SentEvent(eventName, 0, eventValue.ToString()));
            }
            else
            {
                Console.WriteLine("Received event ...{0} is not defined. Will not be processed nor sent to the simulator.",eventName);
            }
        }



        internal bool CheckIfEventExists(string eventName)
        {
            return Enum.IsDefined(typeof(EVENTS), eventName);
        }

        internal SimConnect Initialize()
        {
            return Connection;
        }

        public static async Task WaitUntil(Func<bool> condition, int frequency = 20, int timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (condition()) await Task.Delay(frequency);
            });

            if (waitTask != await Task.WhenAny(waitTask,
                    Task.Delay(timeout)))
                throw new TimeoutException();
        }


    }
}
