using GMap.NET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FSX_Tracker
{
    //PAN JANEK!
    class MasterViewModel : INotifyPropertyChanged
    {
        #region Private Fileds

        private MapViewModel _mapVM;
        private PlaneViewModel _planeVM;
        private Chart2DViewModel _chart2DVM;
        private FSXConnectionManager.CONNECTION_STATE _simconnectConnectionState;
        private int _zoom;
        private int _minZoom;
        private int _maxZoom;
        private bool _nextMouseButtonDownOff;

        private bool _autoMapCenter;

        private double _userLat;
        private double _userLng;
        private double _userAltitude;
        #endregion

        #region Constructors

        public MasterViewModel()
        {
            _autoMapCenter = true;
            _nextMouseButtonDownOff = false;
            _simconnectConnectionState = FSXConnectionManager.CONNECTION_STATE.Disconnected;

            _chart2DVM = new Chart2DViewModel(Constants.MaxChartXValue);
            _planeVM = new PlaneViewModel();
            _mapVM = new MapViewModel();

            ResetMapZoom();

            _zoom = Constants.StartZoom;
            _mapVM.MaxZoom = Constants.MaxZoom;
            _mapVM.MinZoom = Constants.MinZoom;
            _mapVM.Zoom = Constants.StartZoom;


            ConnectionManager.DataReceived += ProcessSimConnectMessage;
            ConnectionManager.ChangeConnectionState += ChangeSimconnectConnection;
            _mapVM.MouseDoubleClickEvent += MouseDoubleClick;
            _mapVM.MouseWheelEvent += MouseWheelEvent;
            _mapVM.MouseDownEvent += MouseDown;

            ConnectToSimconnect();
        }

        #endregion

        #region Properties

        public string UserAltitude
        {
            get
            {
                return _userAltitude.ToString();
            }
            set
            {
                Regex regex = new Regex(@"^(\d+)([.,](\d+))?$");
                if (regex.IsMatch(value) && !string.IsNullOrEmpty(value))
                {
                    _userAltitude = double.Parse(value.Replace(',','.'), CultureInfo.InvariantCulture);
                    OnPropertyChanged(nameof(UserAltitude));
                }
            }
        }

        public string UserLat
        {
            get
            {
                return _userLat.ToString();
            }
            set
            {
                Regex regex = new Regex(@"^(\d+)([.,](\d+))?$");
                if (regex.IsMatch(value) && !string.IsNullOrEmpty(value))
                {
                    _userLat = double.Parse(value.Replace(',', '.'), CultureInfo.InvariantCulture);
                    OnPropertyChanged(nameof(UserLat));
                }
            }
        }

        public string UserLng
        {
            get
            {
                return _userLng.ToString();
            }
            set
            {
                Regex regex = new Regex(@"^(\d+)([.,](\d+))?$");
                if (regex.IsMatch(value) && !string.IsNullOrEmpty(value))
                {
                    _userLng = double.Parse(value.Replace(',', '.'), CultureInfo.InvariantCulture);
                    OnPropertyChanged(nameof(UserLng));
                }
            }
        }
        public PlaneViewModel PlaneVM
        {
            get
            {
                return _planeVM;
            }
        }

        public Chart2DViewModel Chart2DVM
        {
            get
            {
                return _chart2DVM;
            }
        }

        public bool AutoMapCenter
        {
            get
            {
                return _autoMapCenter;
            }
            set
            {
                _autoMapCenter = value;
            }
        }

        public FSXConnectionManager.CONNECTION_STATE SimconnectConnectionState
        {
            get
            {
                return _simconnectConnectionState;
            }
            set
            {
                _simconnectConnectionState = value;
                OnPropertyChanged(nameof(SimconnectConnectionState));
            }

        }

        public MapViewModel MapVM
        {
            get
            {
                return _mapVM;
            }
            set
            {
                _mapVM = value;
                OnPropertyChanged(nameof(MapVM));
            }
        }

        public FSXConnectionManager ConnectionManager
        {
            get
            {
                return FSXConnectionManager.Instance;
            }
        }

        #endregion

        #region ICommands

        public ICommand ResetMapZoomCommand
        {
            get { return new CommandImpl(ResetMapZoom); }
        }

        public ICommand AutoMapCenterCommand
        {
            get { return new CommandImpl<bool>(SetAutoMapCenter); }
        }

        public ICommand ConnectToSimconnectCommand
        {
            get { return new CommandImpl(ConnectToSimconnect); }
        }

        public ICommand DisconnectFromSimconnectCommand
        {
            get { return new CommandImpl(DisconnectFromSimconnect); }
        }

        public ICommand SetLatLonCommand
        {
            get { return new CommandImpl(SetLatLon); }
        }

        public ICommand SetAltitudeCommand
        {
            get { return new CommandImpl(SetAltitude); }
        }

        #endregion

        #region Private Methods

        private void ResetMapZoom()
        {
            _maxZoom = Constants.MaxZoom;
            _minZoom = Constants.MinZoom;
        }

        private void MouseDoubleClick(PointLatLng point)
        {
            _nextMouseButtonDownOff = true;
            SendLatLonToSimconnect(point.Lat, point.Lng);
            AutoMapCenter = true;

        }

        private void SendLatLonToSimconnect(double lat, double lng)
        {
            SimConnectDataStruct planeStructure = (SimConnectDataStruct)_planeVM;
            planeStructure.latitude = lat;
            planeStructure.longitude = lng;
            _mapVM.RemoveMarkers();
            FSXConnectionManager.Instance.SendDataToSimconnect(planeStructure);
            
        }

        private void SendAltitudeToSimconnect(double altitude)
        {
            SimConnectDataStruct planeStructure = (SimConnectDataStruct)_planeVM;
            planeStructure.altitude = altitude;
            FSXConnectionManager.Instance.SendDataToSimconnect(planeStructure);

        }

        private void MouseWheelEvent(int delta, PointLatLng point)
        {
            if (delta < 0)
            {
                _maxZoom -= Constants.ZoomStep;
                if (_maxZoom < Constants.MinZoom)
                {
                    _maxZoom = Constants.MinZoom;
                }

            }
            else
            {
                _maxZoom += Constants.ZoomStep;
                if (_maxZoom > Constants.MaxZoom)
                {
                    _maxZoom = Constants.MaxZoom;
                }
            }

            if (!_autoMapCenter)
            {
                if (delta < 0)
                {
                    _zoom -= Constants.ZoomStep;
                    if (_zoom < Constants.MinZoom)
                    {
                        _zoom = Constants.MinZoom;
                    }
                }
                else
                {
                    _zoom += Constants.ZoomStep;
                    if (_zoom > Constants.MaxZoom)
                    {
                        _zoom = Constants.MaxZoom;
                    }
                }
                _mapVM.SetCenter(point);
                _mapVM.Zoom = _zoom;

            }
        }

        private void SetAutoMapCenter(bool value)
        {
            AutoMapCenter = value;
        }

        private void MouseDown(PointLatLng point)
        {
            if (!_nextMouseButtonDownOff)
            {
                AutoMapCenter = false;
                
            }
            _nextMouseButtonDownOff = false;
        }

        private void ConnectToSimconnect()
        {
            ConnectionManager.Connect();
        }

        private void DisconnectFromSimconnect()
        {
            ConnectionManager.Disconnect();
        }

        private void ProcessSimConnectMessage(SimConnectDataStruct dataStruct)
        {
            _planeVM.Update(dataStruct);

            PointLatLng planePoint = new PointLatLng(_planeVM.Latitude, _planeVM.Longitude);
            _mapVM.AddMarker(planePoint);

            if (_autoMapCenter)
            {
                _mapVM.SetCenter(planePoint);
                _zoom = (int)Math.Min(_maxZoom - Math.Ceiling(_planeVM.GroundSpeed / (_maxZoom - _minZoom)), _maxZoom);
                _mapVM.Zoom = _zoom;

            }

            _chart2DVM.Add(_planeVM.Airspeed);
        }

        private void ChangeSimconnectConnection(FSXConnectionManager.CONNECTION_STATE state)
        {
            SimconnectConnectionState = state;
        }

        private void SetLatLon()
        {
            SendLatLonToSimconnect(_userLat, _userLng);
            AutoMapCenter = true;
        }

        private void SetAltitude()
        {
            SendAltitudeToSimconnect(_userAltitude);
        }


        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
