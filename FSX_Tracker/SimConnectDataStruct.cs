using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FSX_Tracker
{
    public struct SimConnectDataStruct
    {
        #region Public Fields

        [SimConnect(0, "Plane Longitude", "degrees")]
        public double longitude;

        [SimConnect(1, "Plane Latitude", "degrees")]
        public double latitude;

        [SimConnect(2, "Plane Altitude", "kilometers")]
        public double altitude;

        [SimConnect(3, "AIRSPEED TRUE", "knots")]
        public double airspeed;

        [SimConnect(4, "ATTITUDE INDICATOR PITCH DEGREES", "degrees")]
        public double pitch;

        [SimConnect(5, "ATTITUDE INDICATOR BANK DEGREES", "degrees")]
        public double bank;

        [SimConnect(6, "Delta Heading Rate", "degrees per second")]
        public double delta;

        [SimConnect(7, "Turn Coordinator Ball", "number")]
        public double turn;

        [SimConnect(8, "Heading Indicator", "degrees")]
        public double heading;

        [SimConnect(9, "Vertical Speed", "ft/min")]
        public double verticalSpeed;

        [SimConnect(10, "GPS GROUND SPEED", "m/s")]
        public double groundSpeed;

        #endregion

        #region Public Methods

        public static IEnumerable<SimConnectAttribute> GetMetaData()
        {
            var fields = typeof(SimConnectDataStruct).GetFields(BindingFlags.Instance | BindingFlags.Public);
            List<SimConnectAttribute> fieldsAttributes = new List<SimConnectAttribute>();
            fields.ToList().ForEach(x => {
                var attribute = x.GetCustomAttribute<SimConnectAttribute>();
                if (attribute != null) { fieldsAttributes.Add(attribute); } }
            );
            return fieldsAttributes.OrderBy(x => x.Index);
        }

        #endregion

    }


    [System.AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class SimConnectAttribute : Attribute
    {
        #region Private Fields
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        private readonly int _index;
        private readonly string _header;
        private readonly string _unit;
        private readonly SIMCONNECT_DATATYPE _type;
        private readonly float _fEpsilon;

        #endregion

        #region Public Methods

        // This is a positional argument
        public SimConnectAttribute(int index, string header, string unit, SIMCONNECT_DATATYPE type = SIMCONNECT_DATATYPE.FLOAT64, float fEpsilon = 0.0f)
        {
            this._index = index;
            this._unit = unit;
            this._header = header;
            this._type = type;
            this._fEpsilon = fEpsilon;
        }

        #endregion

        #region Properties

        public string Header
        {
            get { return _header; }
        }

        public int Index
        {
            get { return _index; }
        }

        public string Unit
        {
            get { return _unit; }
        }

        public SIMCONNECT_DATATYPE Type
        {
            get { return _type; }
        }

        public float FEpsilon
        {
            get { return _fEpsilon; }
        }

        #endregion
    }
}
