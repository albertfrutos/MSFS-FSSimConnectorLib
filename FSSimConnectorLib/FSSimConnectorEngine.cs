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

        

        public void AddSendEvent(string eventName, uint eventValue, bool useLastVariableRequestValue = false)
        {
            actionExecuter.AddSendEvent(eventName, eventValue, useLastVariableRequestValue);
        }
        public void AddVariableRequest(string variableName)
        {
            actionExecuter.AddVariableRequest(variableName);
        }
        public async Task AddAutomation(object automation)
        {
            connector.lockEngineStep = true;
            await actionExecuter.AddAutomation(this, automation);
            connector.lockEngineStep = false;
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

        public async void LaunchActions()
        {
            if (isConnected)
            {
                await new Helpers().WaitWhile(connector.IsEngineLocked);
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
        
        public void WaitUntilVariableIsHigher(string variable, int thresholdValue)
        {
            actionExecuter.WaitUntilVariableIsHigher(variable, thresholdValue);
        }

        public void ExpectVariableToBe(string variable, string value, bool breakExecutionIfNotAsExpected = false)
        {
            actionExecuter.ExpectVariableToBe(variable, value, breakExecutionIfNotAsExpected);
        }

        public void ExpectVariableToBe(string variable, bool value, bool breakExecutionIfNotAsExpected = false)
        {
            ExpectVariableToBe(variable, value.ToString(), breakExecutionIfNotAsExpected);
        }




    }
}

