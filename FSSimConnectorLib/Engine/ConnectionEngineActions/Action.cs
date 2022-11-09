using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FSSimConnectorLib
{
    internal class Action
    {
        public EventEngine Event { get; set; }
        public VariableEngine Variable { get; set; }
        public WaitEngine WaitMs { get; set; }
        public WaitVariableComparisonEngine WaitVariable { get; set; }
        public ClearEngine ClearEngine { get; set; }
        public ExpectVariableToHaveValueEngine ExpectVariableToHaveValue { get; set; }

        internal List<Action> actions = new List<Action>();

        internal string lastVariableRequestValue = "";

        //internal bool 

        
        

        internal async Task AddSendEvent(string eventName, uint eventValue, bool useLastVariableRequestValue = false)
        {
            actions.Add(new Action()
            {
                Event = new EventEngine()
                {
                    Name = eventName,
                    Value = eventValue,
                    UseLastVariableValue = useLastVariableRequestValue
                }
            });
        }

        internal async Task AddVariableRequest(string variableName)
        {
            actions.Add(new Action()
            {
                Variable = new VariableEngine()
                {
                    variableName = variableName
                }
            });
        }

        internal async Task LaunchActions( FSSimConnector connector)
        {
            await new Helpers().WaitWhile(connector.IsEngineLocked);

            //connector.lockEngine = true;
            connector.VariableHasBeenRecovered += ReturnVariableValue;
            foreach (Action action in actions.ToList())
            {
                await new Helpers().WaitWhile(connector.IsEngineStepLocked);

                connector.lockEngineStep = true;

                if (action.Event != null)
                {
                    uint value = action.Event.UseLastVariableValue ? Convert.ToUInt32(lastVariableRequestValue) : action.Event.Value;

                    await connector.SendEvent(action.Event.Name, value);
                }
                else if (action.Variable != null)
                {
                    await connector.RequestVariable(action.Variable.variableName);
                }
                else if (action.WaitMs != null)
                {
                    await action.WaitMs.WaitMilliseconds(action.WaitMs.WaitMs);
                    connector.lockEngineStep = false;
                }
                else if (action.WaitVariable != null)
                {
                    await action.WaitVariable.WaitVariableComparison(action.WaitVariable, connector);
                    connector.lockEngineStep = false;
                }
                else if (action.ExpectVariableToHaveValue != null)
                {
                    bool isExpectedValueOK = await action.ExpectVariableToHaveValue.ExpectVariableToHaveValue(action.ExpectVariableToHaveValue, connector);
                    connector.lockEngineStep = false;
                    if(!isExpectedValueOK && action.ExpectVariableToHaveValue.stopActionsIfUnexpectedValue)
                    {
                        Console.WriteLine("The rest of actions will not be executed.");
                        break;
                    }
                }
                else if (action.ClearEngine != null)
                {
                    await ClearActions();
                    connector.lockEngineStep = false;
                }
            }

            
        }

        internal async Task AddAutomation(object engine, object automation)
        {
            await new Automation().AddAutomation((FSSimConnectorEngine)engine, automation);
            Console.WriteLine("automation added");
        }

        internal async Task ExpectVariableToBe(string variable, string value, bool breakExecutionIfNotAsExpected)
        {
            actions.Add(new Action()
            {
                ExpectVariableToHaveValue = new ExpectVariableToHaveValueEngine()
                {
                    variableName = variable,
                    expectedValue = value,
                    stopActionsIfUnexpectedValue = breakExecutionIfNotAsExpected
                }
            });
        }

        private void ReturnVariableValue(object sender, RecoveredVariable e)
        {
            lastVariableRequestValue = e.Value;
        }

        internal async Task AddWaitSeconds(int seconds)
        {
            AddWaitMillis(seconds*1000);
        }

        internal async Task ClearActions()
        {
            actions.Clear();
            actions = new List<Action>();
        }
        
        internal async Task AddClearActions()
        {
            actions.Add(new Action()
            {
                ClearEngine = new ClearEngine()
            });
        }

        internal async Task WaitUntilVariableIsHigher(string variable, int thresholdValue)
        {
            actions.Add(new Action()
            {
                WaitVariable = new WaitVariableComparisonEngine()
                {
                    variableName = variable,
                    thresholdValue = thresholdValue,
                    comparison = '>'
                }
            });
        }

        internal async Task AddWaitMillis(int millis)
        {
            actions.Add(new Action()
            {
                WaitMs = new WaitEngine()
                {
                    WaitMs = millis
                }
            });

        }
    }
}
