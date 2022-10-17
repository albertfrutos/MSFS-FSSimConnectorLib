using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSSimConnectorLib
{
    public class FSSimConnectorEngine
    {
        public FSSimConnector connector = new FSSimConnector();
        internal bool isConnected = false;
        Action actionExecuter = new Action();

        public bool Initialize()
        {
            if (connector.Connect())
            {
                isConnected = true;
                connector.Initialize();
            }
            
            return isConnected;
        }

        public void AddSendEvent(string eventName, uint eventValue)
        {
            actionExecuter.AddSendEvent(eventName, eventValue);
        }
        public void AddVariableRequest(string variableName)
        {
            actionExecuter.AddVariableRequest(variableName);
        }

        public void AddVariableRequest(List<string> variableNamesList)
        {
            foreach (string variableName in variableNamesList)
            {
                actionExecuter.AddVariableRequest(variableName);
            }
        }


        public void ClearActions()
        {
            actionExecuter.ClearActions();
        }

        public void LaunchActions()
        {
            if (isConnected)
            {
                actionExecuter.LaunchActions(connector);
            }
            else
            {
                Console.WriteLine("No connection to the simulator, so cannot execute the actions");
            }
            
        }

        public void WaitMillis(int millis)
        {
            actionExecuter.AddWaitMillis(millis);
        }

        public void WaitSeconds(int seconds)
        {
            actionExecuter.AddWaitSeconds(seconds);
        }



    }
}

