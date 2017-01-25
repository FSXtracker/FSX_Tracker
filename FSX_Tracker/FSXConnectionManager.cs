using System;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.FlightSimulator.SimConnect;
using System.Threading.Tasks;

namespace FSX_Tracker
{
    public class FSXConnectionManager
    {

        #region Events

        public event Action<SimConnectDataStruct> DataReceived;
        public event Action<CONNECTION_STATE> ChangeConnectionState;

        #endregion

        #region private Fields

        private bool _pause { get; set; }

        private static FSXConnectionManager _instance;

        private SimConnect _simConnect { get; set; }

        private EventWaitHandle _simconnectReady = new EventWaitHandle(false, EventResetMode.AutoReset);

        private CancellationTokenSource _simconnectReciveMessageCancelTokenSource;

        #endregion

        #region Enums

        public enum CONNECTION_STATE
        {
            Disconnected = 0,
            Connecting = 1,
            Connected = 2,

        }

        private enum SIMCONNECT_DATA_DEFINITION_ID
        {
            planeLocation
        }

        private enum DATA_REQUEST_ID
        {
            REQUEST_1
        }

        private enum EVENT_ID
        {
            EVENT_PAUSE,
            EVENT_BRAKES
        }

        #endregion

        #region Constructors

        private FSXConnectionManager()
        {
            ChangeConnectionState?.Invoke(CONNECTION_STATE.Disconnected);
        }

        #endregion

        #region properties 

        public static FSXConnectionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FSXConnectionManager();
                }
                return _instance;
            }
        }

        #endregion

        #region Public Methods

        public bool Connect()
        {
            if (_simConnect == null)
            {
                try
                {
                    ChangeConnectionState?.Invoke(CONNECTION_STATE.Connecting);
                    _simConnect = new SimConnect("FSX Plane Tracer", (IntPtr)0, 0, _simconnectReady, 0);
                    SetupEventHandlers();
                    SetupPulling();
                    


                    _simconnectReciveMessageCancelTokenSource = new CancellationTokenSource();
                    Task.Factory.StartNew(() =>
                    {
                        SimconnectReciveMessage(_simconnectReciveMessageCancelTokenSource.Token);
                    }, _simconnectReciveMessageCancelTokenSource.Token);
                       
                    return true;
                }
                catch (COMException)
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public void Disconnect()
        {
            if (_simConnect != null)
            {  

                _simconnectReciveMessageCancelTokenSource.Cancel();
                _simConnect.UnsubscribeFromSystemEvent(SIMCONNECT_DATA_DEFINITION_ID.planeLocation);
                ChangeConnectionState?.Invoke(CONNECTION_STATE.Disconnected);
                _simConnect.Dispose();
                _simConnect = null;
            }
        }

        public static bool TestSimconnect()
        {
            try
            {
                new SimConnect("FSX Plane Tracer", (IntPtr)0, 0, new EventWaitHandle(false, EventResetMode.AutoReset), 0).Dispose();
                return true;
            }
            catch (Exception)
            {
                return false; 
            }
        }

        public bool SendDataToSimconnect(SimConnectDataStruct data)
        {

            if (_simConnect != null)
            {
                _simConnect.SetDataOnSimObject(SIMCONNECT_DATA_DEFINITION_ID.planeLocation, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_DATA_SET_FLAG.DEFAULT, data);
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Private Methods

        private void SetupEventHandlers()
        {
            try
            {
                _simConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(SimconnectOnRecvOpen);
                _simConnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(SimconnectOnRecvQuit);
                _simConnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(simconnect_OnRecvException);
                _simConnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(SimconnectOnRecvSimobjectData);
                _simConnect.OnRecvEvent += new SimConnect.RecvEventEventHandler(SimconnectOnRecvEvent);
            }
            catch (COMException)
            {

            }
        }

        private void SetupPulling()
        {

            foreach (var simConnectDataMember in SimConnectDataStruct.GetMetaData())
            {
                _simConnect.AddToDataDefinition(SIMCONNECT_DATA_DEFINITION_ID.planeLocation, simConnectDataMember.Header, simConnectDataMember.Unit, simConnectDataMember.Type, simConnectDataMember.FEpsilon, SimConnect.SIMCONNECT_UNUSED);
            }
            _simConnect.MapClientEventToSimEvent(EVENT_ID.EVENT_PAUSE, "pause_toggle");
            _simConnect.MapClientEventToSimEvent(EVENT_ID.EVENT_BRAKES, "brakes");


            _simConnect.RegisterDataDefineStruct<SimConnectDataStruct>(SIMCONNECT_DATA_DEFINITION_ID.planeLocation);
            _simConnect.RequestDataOnSimObject(DATA_REQUEST_ID.REQUEST_1, SIMCONNECT_DATA_DEFINITION_ID.planeLocation, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, 0, 0, 0, 0);
        }

        private void SimconnectReciveMessage(CancellationToken ct)
        {
            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }
                _simconnectReady.WaitOne();
                try 
                {
                    _simConnect.ReceiveMessage();
                }
                catch (System.NullReferenceException)
                {

                }
            }
        }

        private void SimconnectOnRecvEvent(SimConnect sender, SIMCONNECT_RECV_EVENT data)
        {
            switch ((EVENT_ID)data.uEventID)
            {
                case EVENT_ID.EVENT_PAUSE:
                    _pause = !_pause;
                    break;
                case EVENT_ID.EVENT_BRAKES:

                    break;
            }
        }

        private void SimconnectOnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            var simConnectData = (SimConnectDataStruct)data.dwData[0];
            DataReceived?.Invoke(simConnectData);
        }

        private void SimconnectOnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            ChangeConnectionState?.Invoke(CONNECTION_STATE.Connected);
        }

        private void SimconnectOnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Disconnect();
        }

        private void simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {

        }

        #endregion
    }
}
