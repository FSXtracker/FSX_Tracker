using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace FSX_Tracker
{
    public class Chart2DViewModel : INotifyPropertyChanged
    {

        private List<KeyValuePair<int, double>> _valueList;
        private int _maxX;
        private int _countX;
        public List<double> _y = new List<double>();
        public List<double> _x = new List<double>();

        public List<KeyValuePair<int, double>> ValueList
        {
            get
            {
                return _valueList;
            }
            private set
            {
                _valueList = value;
                OnPropertyChanged(nameof(ValueList));
            }
        }

        public Chart2DViewModel(int maxX)
        {
            _valueList = new List<KeyValuePair<int, double>>();
            _maxX = maxX;
            _countX = 0;
        }

        public void Add(double y)
        {
            if (_valueList.Count == _maxX)
            {
                _valueList.RemoveAt(0);
                _y.RemoveAt(0);
                _x.RemoveAt(0);
            }

            _valueList.Add(new KeyValuePair<int, double>(_countX++, y));
            _y.Add(y);
            _x.Add(_countX);

            OnPropertyChanged(nameof(ValueList));

            OnPropertyChanged(nameof(Test));

        }

        public EnumerableDataSource<Point> Test
        {
            get
            {
                List<Point> p = new List<Point>();
                for (int i = 0; i < 100 && i < _x.Count; i++)
                {
                    p.Add(new Point((int)_x[i], (int)_y[i]));
                }

                EnumerableDataSource<Point> ds = new EnumerableDataSource<Point>(p);

                //ds.SetXMapping(x => x);
                //ds.SetYMapping(y => plane.y);
                ds.SetXYMapping(x => x);

                return ds;
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
