using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace FSX_Tracker
{
    public class MapViewModel : INotifyPropertyChanged
    {

        #region Events

        public event Action<GMapPolygon> AddMarkerEvent;
        public event Action RemoveMarkersEvent;
        public event Action<PointLatLng> MouseDoubleClickEvent;
        public event Action<PointLatLng> MouseDownEvent;
        public event Action<PointLatLng> SetCenterEvent;
        public event Action<int, PointLatLng> MouseWheelEvent;

        #endregion

        #region Privates Fields

        private GMapProvider _selectedProvider;
        private List<PointLatLng> _pointsList;
        private int _zoom;
        private int _minZoom;
        private int _maxZoom;

        #endregion

        #region Properties

        public int Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                _zoom = value;
                OnPropertyChanged(nameof(Zoom));
            }
        }

        public int MinZoom
        {
            get
            {
                return _minZoom;
            }
            set
            {
                _minZoom = value;
                OnPropertyChanged(nameof(MinZoom));
            }
        }

        public int MaxZoom
        {
            get
            {
                return _maxZoom;
            }
            set
            {
                _maxZoom = value;
                OnPropertyChanged(nameof(MaxZoom));
            }
        }

        public GMapProvider SelectedProvider
        {
            get { return _selectedProvider; }
            set
            {
                _selectedProvider = value;
                OnPropertyChanged(nameof(SelectedProvider));
            }
        }

        public ObservableCollection<GMapProvider> Providers { get; set; }

        #endregion

        #region Constructors

        public MapViewModel()
        {
            _pointsList = new List<PointLatLng>();
            Providers = new ObservableCollection<GMapProvider>(GMapProviders.List);
            SelectedProvider = Providers.FirstOrDefault(x => x == GMapProviders.GoogleMap);
            GMap.NET.GMaps.Instance.Mode = AccessMode.ServerAndCache;
        }

        #endregion

        #region Public Methods

        public void AddMarker(PointLatLng point)
        {
            if (_pointsList.Any())
            {
                GMapPolygon polygon = new GMapPolygon(new List<PointLatLng>
                {
                    _pointsList.Last(),
                    point
                });
                AddMarkerEvent?.Invoke(polygon);
            }
            else
            {
                GMapPolygon polygon = new GMapPolygon(new List<PointLatLng>
                {
                    point,
                    point
                });
                AddMarkerEvent?.Invoke(polygon);
            }

            _pointsList.Add(point);
        }

        public void SetCenter(PointLatLng point)
        {
            SetCenterEvent?.Invoke(point);
        }

        public void RemoveMarkers()
        {
            _pointsList.Clear();
            RemoveMarkersEvent?.Invoke();
        }

        public void RaiseMouseDoubleClickEvent(PointLatLng point)
        {
            MouseDoubleClickEvent?.Invoke(point);
        }

        public void RaiseMouseDownEvent(PointLatLng point)
        {
            MouseDownEvent?.Invoke(point);
        }

        public void RaiseMouseWheelEvent(int delta, PointLatLng point)
        {
            MouseWheelEvent?.Invoke(delta, point);
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        //public ICommand AddMarkerCommand
        //{
        //    get { return new CommandImpl<GMapPolygon>(AddMarkerExecute); }
        //}

    }
}
