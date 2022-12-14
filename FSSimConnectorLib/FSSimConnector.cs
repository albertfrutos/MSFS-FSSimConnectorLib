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
using FSSimConnectorLib;
using System.IO;
using System.Text.RegularExpressions;

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

        public object flightModel { get; set; } = null;

        public event EventHandler<RecoveredVariable> VariableHasBeenRecovered;
        public event EventHandler<SentEvent> EventHasBeenSent;

        internal bool lockEngine = false;
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

        internal bool IsEngineStepLocked()
        {
            return lockEngineStep;
        }
        
        internal bool IsEngineLocked()
        {
            return lockEngine;
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
                variableRequestor.LoadVariables(configuration.variablesConfig);
            }
        }

        public async void LoadCurrentPlaneFlightModel()
        {

            if (Connection != null)
            {
                Connection.OnRecvSystemState += new SimConnect.RecvSystemStateEventHandler(GetAirCraftCfgPath);
                Connection.RequestSystemState(DATA_REQUESTS.REQUEST_1, "AircraftLoaded");
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

        public async void Initialize(bool updateVariables = false)
        {
            configuration = new Configuration().LoadConfiguration();

            Connect();
            InitializeRequestor(updateVariables);
            InitializeSetter();
            
            LoadCurrentPlaneFlightModel();
        }

        internal bool IsFlightModelNotInitialized()
        {
            return this.flightModel == null;
        }

        private void GetAirCraftCfgPath(SimConnect sender, SIMCONNECT_RECV_SYSTEM_STATE data)
        {
            List<string> installedAircrafts = new Helpers().SearchFileInAllDirectories(configuration.paths.baseFSPathOfficial, "aircraft.cfg");
            installedAircrafts.AddRange(new Helpers().SearchFileInAllDirectories(configuration.paths.baseFSPathCommunity, "aircraft.cfg"));

            var currentAircraftCfgPath = installedAircrafts.Where(z => z.EndsWith(data.szString, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().ToString();

            if (currentAircraftCfgPath != "")
            {
                string flightModelPath = Path.Combine(Path.GetDirectoryName(currentAircraftCfgPath), "flight_model.cfg");

                flightModel = new FlightModel(flightModelPath);

            }
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
                variableRequestor.Initialize(configuration.variablesConfig);
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

        public async Task SendEvent(string eventName, uint eventValue, bool updateVariableAfterEvent = false, string variableNameToUpdate = "")
        {
            try
            {
                await eventSetter.SendEvent(eventName, eventValue);

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

        public async Task RequestVariable(string variableName)
        {
            try
            {
                isVariableRequestInProgress = true;
                await variableRequestor.RequestVariable(variableName);
                await new Helpers().WaitWhile(IsRequestInProgress);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred while requesting the variable (" + variableName +  ") to MSFS: " + ex.Message);
            }
        }

        public bool IsRequestInProgress()
        {
            return isVariableRequestInProgress;
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
