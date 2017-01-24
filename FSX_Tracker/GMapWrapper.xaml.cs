using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FSX_Tracker
{
    /// <summary>
    /// Interaction logic for GMapWrapper.xaml
    /// </summary>
    public partial class GMapWrapper : UserControl
    {
        #region Provider Propdp

        public GMapProvider Provider
        {
            get { return (GMapProvider)GetValue(ProviderProperty); }
            set { SetValue(ProviderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Provider.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProviderProperty =
            DependencyProperty.Register("Provider", typeof(GMapProvider), typeof(GMapWrapper), new PropertyMetadata(GMapProviders.EmptyProvider));

        #endregion

        #region Constructors

        public GMapWrapper()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        private void ControlDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                UnsubscribeEvents(e.OldValue as MapViewModel);
            }
            var newDC = (e.NewValue as MapViewModel);
            if (newDC != null)
            {
                SubscribeEvents(newDC);
                mapControl.MaxZoom = newDC.MaxZoom;
                mapControl.MinZoom = newDC.MinZoom;
                mapControl.Zoom = newDC.Zoom;
   
            }
        }

        private void UnsubscribeEvents(MapViewModel mapViewModel)
        {
            mapViewModel.AddMarkerEvent -= AddMarker;
            mapViewModel.SetCenterEvent -= SetCenter;
            mapViewModel.RemoveMarkersEvent -= RemoveMarkers;
            this.mapControl.MouseDoubleClick -= MouseDoubleClickHandler;
            this.mapControl.MouseDown -= MouseDownHandler;
            this.mapControl.PreviewMouseWheel -= MouseWheelHendler;
        }

        private void SubscribeEvents(MapViewModel mapViewModel)
        {
            mapViewModel.AddMarkerEvent += AddMarker;
            mapViewModel.SetCenterEvent += SetCenter;
            mapViewModel.RemoveMarkersEvent += RemoveMarkers;
            this.mapControl.MouseDoubleClick += MouseDoubleClickHandler;
            this.mapControl.MouseDown += MouseDownHandler;
            this.mapControl.PreviewMouseWheel += MouseWheelHendler;
        }

        private void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(mapControl);
            PointLatLng point = mapControl.FromLocalToLatLng((int)p.X, (int)p.Y);
            (this.DataContext as MapViewModel).RaiseMouseDownEvent(point);
        }

        private void MouseWheelHendler(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            Point p = e.GetPosition(mapControl);
            PointLatLng point = mapControl.FromLocalToLatLng((int)p.X, (int)p.Y);
            (this.DataContext as MapViewModel).RaiseMouseWheelEvent(e.Delta, point);
        }

        private void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(mapControl);
            PointLatLng point = mapControl.FromLocalToLatLng((int)p.X, (int)p.Y);
            (this.DataContext as MapViewModel).RaiseMouseDoubleClickEvent(point);
        }

        public void AddMarker(GMapPolygon polygon)
        {
            if (mapControl?.Markers != null)
            {
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        polygon.RegenerateShape(mapControl);
                        Path path = ((Path)polygon.Shape);
                        path.Stroke = Brushes.Black;
                        path.Effect = null;
                        path.Opacity = 1.0;

                        this.mapControl.Markers.Add(polygon);
                        this.mapControl.InvalidateVisual();
                    });
                }
                catch (Exception e)
                {

                }
            }
        }

        public void SetCenter(PointLatLng point)
        {

            try
            {
                Dispatcher.Invoke(() =>
                    {
                        this.mapControl.Position = point;
                    });
            }
            catch (Exception e)
            {

            }
        }

        public void RemoveMarkers()
        {
            if (mapControl?.Markers != null)
            {
                this.mapControl.Markers.Clear();
                this.mapControl.ReloadMap();
            }
        }

        public void SetZoom(double value)
        {
            mapControl.Zoom = value;
        }

        #endregion
    }
}
