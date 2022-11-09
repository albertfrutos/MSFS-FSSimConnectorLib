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

namespace FSSimConnectorLib
{
    internal class SimulationVariableRequestor : Entities
    {
        private static SimConnect Connection = null;

        FSSimConnector connector = null;

        internal bool isProcessing = false;

        public static List<Variable> VariablesList { get; private set; } = null;

        Timer requestSimulatorDataTimer = null;

        internal Queue processingVariablesQueue = new Queue();
        internal Queue pendingVariablesQueue = new Queue();

        

        internal SimulationVariableRequestor(SimConnect AppConnection, FSSimConnector fSSimConnector)
        {
            Connection = AppConnection;
            connector = fSSimConnector;
        }

        internal SimConnect Initialize(VariablesConfig variablesConfig)
        {
            VariablesList = new Variable().LoadVariables(variablesConfig.variablesFile);
            Connection.OnRecvOpen += new SimConnect.RecvOpenEventHandler(Simconnect_OnRecvOpen);
            Connection.OnRecvQuit += new SimConnect.RecvQuitEventHandler(Simconnect_OnRecvQuit);
            Connection.OnRecvException += new SimConnect.RecvExceptionEventHandler(Simconnect_OnRecvException);
           
            requestSimulatorDataTimer = new Timer(TimerCallback, null, 0, 20);

            return Connection;
        }

        internal void DisposeTimer()
        {
            requestSimulatorDataTimer.Dispose();
        }

        internal void LoadVariables(VariablesConfig variablesConfig)
        {
            VariablesList = new Variable().LoadVariables(variablesConfig.variablesFile);
        }

        internal async Task RequestVariable(string variableName)
        {
            pendingVariablesQueue.Enqueue(variableName);
        }

        internal void ProcessVariableRequest(string variableName)
        {
            Variable variable = new Variable().GetVariableInformation(variableName, VariablesList);

            if (!(variable is null) && !(variable.type is null))
            {
                Entities entities = new Entities();

                var dataType = entities.VariableTypes[variable.type];
                var defineID = entities.DefineIDs[variable.type];
                var unit = variable.unit;

                if (variable.type == "string")
                {
                    Connection.AddToDataDefinition(defineID, variableName, "", dataType, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    Connection.RegisterDataDefineStruct<StringType>(DEFINITIONS.StringType);
                    //return;
                }
                else if (variable.type == "int32")
                {
                    Connection.AddToDataDefinition(defineID, variableName, unit, dataType, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    Connection.RegisterDataDefineStruct<NumType>(DEFINITIONS.NumType);
                }
                else if (variable.type == "bool")
                {
                    Connection.AddToDataDefinition(defineID, variableName, unit, dataType, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    Connection.RegisterDataDefineStruct<BoolType>(DEFINITIONS.BoolType);
                }
                
                Connection.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(Simconnect_OnRecvSimobjectDataBytype);

                Connection.RequestDataOnSimObjectType(DATA_REQUESTS.REQUEST_1, defineID, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);

                processingVariablesQueue.Enqueue(variable);
                Connection.ClearDataDefinition(defineID);
                Connection.ReceiveMessage();
            }
            else
            {
                Console.WriteLine("Variable name {0} provided is null or it's type is not supported", variableName);
                Console.WriteLine("Only variables which are numbers, strings or booleans are supported");
            }
        }

        internal void TimerCallback(Object o)
        {
            try
            {
                if (Connection != null)
                {
                    Connection.ReceiveMessage();
                    if (isProcessing is false && pendingVariablesQueue.Count > 0)
                    {
                        isProcessing = true;
                        ProcessVariableRequest(pendingVariablesQueue.Dequeue().ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
                
            }
        }

        private static void Simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            Console.WriteLine("An exception occurred Simconnect_OnRecvException: {0}", data.dwException.ToString());
        }

        private static void Simconnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {

        }

        private static void Simconnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Console.WriteLine("Simulator has exited. Closing connection and exiting Simulator module");

            return;

        }

        private void Simconnect_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            try
            {
                if (processingVariablesQueue.Count == 0)
                {
                    return;
                }

                Variable variable = (Variable)processingVariablesQueue.Dequeue();
                string parameterValue = null;

                if (variable.type == "string")
                {
                    StringType result = (StringType)data.dwData[0];
                    parameterValue = result.stringVar;
                }
                else if (variable.type == "int32")
                {
                    NumType result = (NumType)data.dwData[0];
                    parameterValue = result.numVar.ToString();
                }
                else if (variable.type == "bool")
                {
                    BoolType result = (BoolType)data.dwData[0];
                    parameterValue = result.boolVar.ToString();
                }

                var recoveredVariable = new RecoveredVariable(variable, parameterValue);
                connector.OnVariableRecovery(recoveredVariable);

                isProcessing = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while receiving variable: {1}", ex.Message);
            }
        }

        private static FieldInfo[] GetFieldInfo(SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data, ref object currentData)
        {
            FieldInfo[] fi = null;

            if (data.dwDefineID == 0)
            {
                currentData = (StringType)data.dwData[0];

                fi = typeof(StringType).GetFields(BindingFlags.Public | BindingFlags.Instance);
            }
            if (data.dwDefineID == 1)
            {
                currentData = (NumType)data.dwData[0];

                fi = typeof(NumType).GetFields(BindingFlags.Public | BindingFlags.Instance);
            }
            if (data.dwDefineID == 2)
            {
                currentData = (BoolType)data.dwData[0];

                fi = typeof(BoolType).GetFields(BindingFlags.Public | BindingFlags.Instance);
            }

            return fi;
        }

        internal bool AreTherePendingVariables()
        {
            return processingVariablesQueue.Count > 0 || pendingVariablesQueue.Count > 0;
        }




    }
}
