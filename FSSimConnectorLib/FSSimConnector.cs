using FSSimConnectorLib.SetEvents;
using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static FSSimConnectorLib.Entities;
using FSSimConnectorLib.LibConfiguration;

namespace FSSimConnectorLib
{
    public class FSSimConnector
    {
        private static SimConnect Connection = null;
        

        internal VariablesUpdater variablesUpdater = new VariablesUpdater();

        internal EventsUpdater eventsUpdater = new EventsUpdater();

        internal SimulationVariableRequestor variableRequestor = null;

        internal SimulationEventSender eventSetter = null;

        internal Configuration configuration = new Configuration();

        public event EventHandler<RecoveredVariable> VariableHasBeenRecovered;
        public event EventHandler<SentEvent> EventHasBeenSent;

        internal bool lockEngineStep = false;
        internal bool isVariableRequestInProgress = false;
        internal bool isEventSendingInProgress = true;


        internal bool Connect()
        {
            try
            {
                Connection = new Microsoft.FlightSimulator.SimConnect.SimConnect("Data Request", IntPtr.Zero, 0x402, null, 0);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred while connecting to MSFS: " + ex.Message);
                return false;
            }
        }

        internal bool IsEngineLocked()
        {
            return lockEngineStep;
        }

        public void Disconnect()
        {
            variableRequestor.DisposeTimer();

            if (Connection != null)
            {
                Connection.Dispose();
                Connection = null;
            }
        }

        public void LoadVariables(bool reloadVariables = true)
        {
            if (reloadVariables)
            {
                variableRequestor.LoadVariables();
            }
        }

        

        public void UpdateVariables(bool reloadVariables = true)
        {
            try
            {
                variablesUpdater.UpdateVariables(configuration.variablesConfig);
                LoadVariables(reloadVariables);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred while updating the variables: " + ex.Message);
            }
        }

        public void UpdateEvents()
        {
            try
            {
                eventsUpdater.UpdateEvents(configuration.eventsConfig);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred while updating the events: " + ex.Message);
            }
        }

        public void Initialize(bool updateVariables = false)
        {
            configuration = new Configuration().LoadConfiguration();

            Connect();
            InitializeRequestor(updateVariables);
            InitializeSetter();
        }

        internal void InitializeRequestor(bool updateVariables = false)
        {
            try
            {
                if (updateVariables)
                {
                    UpdateVariables(false);
                }
                variableRequestor = new SimulationVariableRequestor(Connection, this);
                variableRequestor.Initialize();
            }
            catch (Exception ex)
            {
                Disconnect();
                Console.WriteLine("An exception occurred while initializing the variable requestor module: " + ex.Message);
            }
        }

        public void InitializeSetter()
        {
            try
            {
                eventSetter = new SimulationEventSender(Connection,this);
                eventSetter.Initialize();
            }
            catch(Exception ex)
            {
                Disconnect();
                Console.WriteLine("An exception occurred while initializing the event setter module: " + ex.Message);
            }
        }

        public async void SendEvent(string eventName, uint eventValue, bool updateVariableAfterEvent = false, string variableNameToUpdate = "")
        {
            try
            {
                eventSetter.SendEvent(eventName, eventValue);

                if (updateVariableAfterEvent)
                {
                    variableRequestor.RequestVariable(variableNameToUpdate);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred while setting the event (" + eventName + ") to MSFS: " + ex.Message);
            }
        }

        public void RequestVariable(List<string> variableNamesList)
        {
            foreach (string variableName in variableNamesList)
            {
                RequestVariable(variableName);
            }
        }

        public void RequestVariable(string variableName)
        {
            try
            {
                isVariableRequestInProgress = true;
                variableRequestor.RequestVariable(variableName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred while requesting the variable (" + variableName +  ") to MSFS: " + ex.Message);
            }
        }

        public void OnVariableRecovery(RecoveredVariable e)
        {
            VariableHasBeenRecovered?.Invoke(this, e);
            isVariableRequestInProgress = false;
            lockEngineStep = false;
        }

        public void OnEventSent(SentEvent e)
        {
            EventHasBeenSent?.Invoke(this, e);
            lockEngineStep = false;
        }

        public bool isConnected()
        {
            return Connection != null;
        }


        public static async Task WaitUntil(Func<bool> condition, int frequency = 500, int timeout = -1)
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
