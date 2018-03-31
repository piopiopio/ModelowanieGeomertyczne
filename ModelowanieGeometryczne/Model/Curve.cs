using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ModelowanieGeometryczne.Helpers;
using ModelowanieGeometryczne.ViewModel;
using OpenTK;

namespace ModelowanieGeometryczne.Model
{
    public abstract class Curve : ViewModelBase
    {
        public event PropertyChangedEventHandler RefreshScene;

        protected Matrix4d projekcja = MatrixProvider.ProjectionMatrix();
        protected Matrix4d projekcjaLeft = MatrixProvider.LeftProjectionMatrix();
        protected Matrix4d projekcjaRight = MatrixProvider.RightProjectionMatrix();
        protected static int CurveNumber = 0;
        protected const int RenderWidth = 1440;
        protected const int RenderHeight = 750;

        private ObservableCollection<Point> _pointsCollection = new ObservableCollection<Point>();
        private ICommand _removePoints;
        private string _name;
        private bool _polylineEnabled = true;
        private bool _selected;
        private string _curveType=null;





        #region Private Methods
        #endregion Private Methods
        #region Public Properties

        public string CurveType
        {
            get { return _curveType; }
            set { _curveType = value; }
        }
        public ICommand RemovePointsCommand { get { return _removePoints ?? (_removePoints = new ActionCommand(RemoveSelectedPoints)); } }
        
        public ObservableCollection<Point> PointsCollection
        {
            get { return _pointsCollection; }
            set { _pointsCollection = value; }
        }

        public string Name
        {
            get { return (_name); }
            set
            {
                CurveNumber++;
                _name = value;
            }
        }

        public bool PolylineEnabled
        {
            get { return _polylineEnabled; }
            set
            {
                _polylineEnabled = value;
                // OnPropertyChanged("PolylineEnabled");     
            }
        }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                OnPropertyChanged("Selected");
            }
        }


        #endregion Public Properties
        #region Private Methods
        #endregion Private Methods
        #region Public Methods

        public abstract void DrawCurve(Matrix4d transformacja);
        public abstract void DrawCurveStereoscopy(Matrix4d transformacja);
        public abstract void DrawPolyline(Matrix4d transformacja);
        public abstract void DrawPolylineStereoscopy(Matrix4d transformacja);

        internal void RemovePoints(List<Point> points)
        {
            foreach (var point in points)
            {
                if (PointsCollection.Contains(point))
                {
                    PointsCollection.Remove(point);
                }
            }
        }

        public void RemoveSelectedPoints()
        {
            var temp = PointsCollection.Where(c => c.Selected).ToList();

            foreach (var point in temp)
            {
                PointsCollection.Remove(point);
            }

            Refresh();
        }

        private void Refresh()
        {
            if (RefreshScene != null)
                RefreshScene(this, new PropertyChangedEventArgs("RefreshScene"));
        }

        public void AddPoint(Point point)
        {
            PointsCollection.Add(point);
        }
        #endregion Public Methods
    }
}

