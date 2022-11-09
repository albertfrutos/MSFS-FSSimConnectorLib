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
        internal Action actionExecuter = new Action();
        public QuickActions quickActions = null;

        public bool Initialize()
        {
            if (connector.Connect())
            {
                isConnected = true;
                connector.Initialize();
                quickActions = new QuickActions(this);
            }
            
            return isConnected;
        }

        public async Task AddSendEvent(string eventName, uint eventValue, bool useLastVariableRequestValue = false)
        {
            await actionExecuter.AddSendEvent(eventName, eventValue, useLastVariableRequestValue);
        }
        public async Task AddVariableRequest(string variableName)
        {
            await actionExecuter.AddVariableRequest(variableName);
        }
        public async Task AddClearActions()
        {
            await actionExecuter.AddClearActions();
        }
        public async Task AddAutomation(object automation)
        {
            connector.lockEngine = true;
            await actionExecuter.AddAutomation(this, automation);
            connector.lockEngine = false;
        }

        public async Task AddVariableRequest(List<string> variableNamesList)
        {
            foreach (string variableName in variableNamesList)
            {
                await actionExecuter.AddVariableRequest(variableName);
            }
        }

        public async Task<string> LaunchActions(bool deleteAfterExecution = false)
        {
            if (isConnected)
            {
                if (deleteAfterExecution)
                {
                    await AddClearActions();
                }
                await actionExecuter.LaunchActions(connector);
            }
            else
            {
                Console.WriteLine("No connection to the simulator, so cannot execute the actions");
            }

            return "a";
            
        }
        public async Task WaitMillis(int millis)
        {
            await actionExecuter.AddWaitMillis(millis);
        }

        public async Task WaitSeconds(int seconds)
        {
            await actionExecuter.AddWaitSeconds(seconds);
        }
        
        public async Task WaitUntilVariableIsHigher(string variable, int thresholdValue)
        {
            await actionExecuter.WaitUntilVariableIsHigher(variable, thresholdValue);
        }

        public async Task ExpectVariableToBe(string variable, string value, bool breakExecutionIfNotAsExpected = false)
        {
            await actionExecuter.ExpectVariableToBe(variable, value, breakExecutionIfNotAsExpected);
        }

        public async Task ExpectVariableToBe(string variable, bool value, bool breakExecutionIfNotAsExpected = false)
        {
            await ExpectVariableToBe(variable, value.ToString(), breakExecutionIfNotAsExpected);
        }
    }
}

