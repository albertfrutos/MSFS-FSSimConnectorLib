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
        public WaitEngine Wait { get; set; }

        internal List<Action> actions = new List<Action>();

        

        internal void AddSendEvent(string eventName, uint eventValue)
        {
            actions.Add(new Action()
            {
                Event = new EventEngine()
                {
                    Name = eventName,
                    Value = eventValue
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
            foreach (Action action in actions)
            {
                await WaitUntil(connector.IsEngineLocked);

                connector.lockEngineStep = true;

                if (action.Event != null)
                {
                    connector.SendEvent(action.Event.Name, action.Event.Value);
                }
                else if (action.Variable != null)
                {
                    connector.RequestVariable(action.Variable.variableName);
                }
                else if (action.Wait != null)
                {
                    new WaitEngine().WaitMilliseconds(action.Wait.WaitMs);
                    connector.lockEngineStep = false;
                }
            }
        }

        internal void AddWaitSeconds(int seconds)
        {
            AddWaitMillis(seconds*1000);
        }

        internal void ClearActions()
        {
            actions.Clear();
        }

        internal void AddWaitMillis(int millis)
        {
            actions.Add(new Action()
            {
                Wait = new WaitEngine()
                {
                    WaitMs = millis
                }
            });

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
