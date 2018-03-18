using System;
using System.Collections.Specialized;
using System.Windows.Media.Media3D;

namespace ModelowanieGeometryczne.Model
{
    public class Point
    {
        private Vector3D _coordinates;
        private string _name;
        #region Private Methods
        #endregion Private Methods
        #region Public Properties

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Vector3D Coordinates
        {
            get { return _coordinates;}
            set { _coordinates = value; }
        }
        #endregion Public Properties

        public Point(double x, double y, double z)
        {
            _coordinates.X = x;
            _coordinates.Y = y;
            _coordinates.Z = z;
            _name = DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString();
        }
        #region Private Methods
        #endregion Private Methods
        #region Public Methods
        #endregion Public Methods
    }
}

