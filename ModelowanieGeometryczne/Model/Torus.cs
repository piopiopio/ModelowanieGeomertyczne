using System.Runtime.InteropServices;
using System.Windows;

using ModelowanieGeometryczne.ViewModel;

namespace ModelowanieGeometryczne.Model
{
    public class Torus:ViewModelBase
    {
        private double _r;
        private double _R;
        private double _l;
        private double _v;


        #region Private Methods

        public Torus()
        {
            _r = 1;
            _R = 2;
            _l = 0.1;
            _v = 0.1;
        }
        #endregion Private Methods
        #region Public Properties
        public double r
        {
            get { return _r; }

            set
            {
                _r = value; 
                OnPropertyChanged("r");
               
            }


        }

        public double R
        {
            get { return _R; }
            set
            {
                _R = value;
                OnPropertyChanged("R");
            }
        }

        public double L
        {
            get { return _l; }
            set
            {
                _l = value;
                OnPropertyChanged("L");
            }
        }

        public double V
        {
            get { return _v; }
            set
            {
                _v = value;
                OnPropertyChanged("V");
            }
        }
        #endregion Public Properties
        #region Private Methods
        #endregion Private Methods
        #region Public Methods
        #endregion Public Methods
    }
}

