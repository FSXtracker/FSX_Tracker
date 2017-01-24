using System.Collections.Generic;
using System.ComponentModel;

namespace FSX_Tracker
{
    public class PlaneViewModel : INotifyPropertyChanged
    {
        #region Private Filds

        private double _longitude;
        private double _latitude;
        private double _altitude;
        private double _airspeed;
        private double _pitch;
        private double _bank;
        private double _delta;
        private double _turn;
        private double _heading;
        private double _verticalSpeed;
        private double _groundSpeed;

        #endregion

        #region Properties

        public double Longitude
        {
            get
            {
                return _longitude;
            }
            private set
            {
                _longitude = value;
                OnPropertyChanged(nameof(Longitude));
            }
        }

        public double Latitude
        {
            get
            {
                return _latitude;
            }
            private set
            {
                _latitude = value;
                OnPropertyChanged(nameof(Latitude));
            }
        }

        public double Altitude
        {
            get
            {
                return _altitude;
            }
            private set
            {
                _altitude = value;
                OnPropertyChanged(nameof(Altitude));
            }
        }

        public double Airspeed
        {
            get
            {
                return _airspeed;
            }
            private set
            {
                _airspeed = value;
                OnPropertyChanged(nameof(Airspeed));
            }
        }

        public double Pitch
        {
            get
            {
                return _pitch;
            }
            private set
            {
                _pitch = value;
                OnPropertyChanged(nameof(Pitch));
            }
        }

        public double Bank
        {
            get
            {
                return _bank;
            }
            private set
            {
                _bank = value;
                OnPropertyChanged(nameof(Bank));
            }
        }

        public double Delta
        {
            get
            {
                return _delta;
            }
            private set
            {
                _delta = value;
                OnPropertyChanged(nameof(Delta));
            }
        }

        public double Turn
        {
            get
            {
                return _turn;
            }
            private set
            {
                _turn = value;
                OnPropertyChanged(nameof(Turn));
            }
        }

        public double Heading
        {
            get { return _heading; }
            private set
            {
                _heading = value;
                OnPropertyChanged(nameof(Heading));
            }
        }

        public double VerticalSpeed
        {
            get
            {
                return _verticalSpeed;
            }
            private set
            {
                _verticalSpeed = value;
                OnPropertyChanged(nameof(VerticalSpeed));
            }
        }

        public double GroundSpeed
        {
            get
            {
                return _groundSpeed;
            }
            private set { _groundSpeed = value; OnPropertyChanged(nameof(GroundSpeed)); }
        }

        #endregion

        #region Constructors

        public PlaneViewModel()
        {

        }

        #endregion

        #region Public Methods

        public void Update(SimConnectDataStruct planeStructure)
        {
            Longitude = planeStructure.longitude;
            Latitude = planeStructure.latitude;
            Altitude = planeStructure.altitude;
            Airspeed = planeStructure.airspeed;
            Pitch = planeStructure.pitch;
            Bank = planeStructure.bank;
            Delta = planeStructure.delta;
            Turn = planeStructure.turn;
            Heading = planeStructure.heading;
            VerticalSpeed = planeStructure.verticalSpeed;
            GroundSpeed = planeStructure.groundSpeed;
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Explicit Operators

        public static explicit operator SimConnectDataStruct(PlaneViewModel plane)
        {
            SimConnectDataStruct planeStructure = new SimConnectDataStruct()
            {

                longitude = plane.Longitude,
                latitude = plane.Latitude,
                altitude = plane.Altitude,
                airspeed = plane.Airspeed,
                pitch = plane.Pitch,
                bank = plane.Bank,
                delta = plane.Delta,
                turn = plane.Turn,
                heading = plane.Heading,
                verticalSpeed = plane.VerticalSpeed,
                groundSpeed = plane.GroundSpeed
            };
            return planeStructure;
        }

        #endregion

    }
}


