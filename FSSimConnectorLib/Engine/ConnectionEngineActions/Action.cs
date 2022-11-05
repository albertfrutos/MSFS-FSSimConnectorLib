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
        public WaitVariableEngine WaitVariable { get; set; }
        public ExpectVariableToHaveValueEngine ExpectVariableToHaveValue { get; set; }

        internal List<Action> actions = new List<Action>();

        internal string lastVariableRequestValue = "";
        

        internal void AddSendEvent(string eventName, uint eventValue, bool useLastVariableRequestValue = false)
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

        internal void AddVariableRequest(string variableName)
        {
            actions.Add(new Action()
            {
                Variable = new VariableEngine()
                {
                    variableName = variableName
                }
            });
        }

        internal async void LaunchActions( FSSimConnector connector)
        {
            connector.VariableHasBeenRecovered += ReturnVariableValue;
            foreach (Action action in actions)
            {
                await new Helpers().WaitWhile(connector.IsEngineLocked);

                connector.lockEngineStep = true;

                if (action.Event != null)
                {
                    uint value = action.Event.UseLastVariableValue ? Convert.ToUInt32(lastVariableRequestValue) : action.Event.Value;

                    connector.SendEvent(action.Event.Name, value);
                }
                else if (action.Variable != null)
                {
                    connector.RequestVariable(action.Variable.variableName);
                }
                else if (action.WaitMs != null)
                {
                    action.WaitMs.WaitMilliseconds(action.WaitMs.WaitMs);
                    connector.lockEngineStep = false;
                }
                else if (action.WaitVariable != null)
                {
                    await action.WaitVariable.WaitVariable(action.WaitVariable, connector);
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
            }
        }

        internal async Task AddAutomation(object engine, object automation)
        {
            await new Automation().AddAutomation((FSSimConnectorEngine)engine, automation);
            Console.WriteLine("automation added");
        }

        internal void ExpectVariableToBe(string variable, string value, bool breakExecutionIfNotAsExpected)
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

        internal void AddWaitSeconds(int seconds)
        {
            AddWaitMillis(seconds*1000);
        }

        internal void ClearActions()
        {
            actions.Clear();
        }

        internal void WaitUntilVariableIsHigher(string variable, int thresholdValue)
        {
            actions.Add(new Action()
            {
                WaitVariable = new WaitVariableEngine()
                {
                    variableName = variable,
                    thresholdValue = thresholdValue,
                    comparison = '>'
                }
            });
        }

        internal void AddWaitMillis(int millis)
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
