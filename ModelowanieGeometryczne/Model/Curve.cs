using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ModelowanieGeometryczne.Helpers;
using ModelowanieGeometryczne.ViewModel;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Specialized;

namespace ModelowanieGeometryczne.Model
{
    public abstract class Curve : ViewModelBase
    {
        public event PropertyChangedEventHandler RefreshScene;

        protected static Matrix4d projekcja = MatrixProvider.ProjectionMatrix();
        protected Matrix4d projekcjaLeft = MatrixProvider.LeftProjectionMatrix();
        protected Matrix4d projekcjaRight = MatrixProvider.RightProjectionMatrix();
        protected static int CurveNumber = 0;
        protected const int RenderWidth = 1440;
        protected const int RenderHeight = 750;

        protected ObservableCollection<Point> _pointsCollection = new ObservableCollection<Point>();
      
        private ICommand _removePoints;
        private string _name;
        private bool _polylineEnabled = true;
        private bool _selected;
        private string _curveType="C0";


        public Curve()
        {
        }

        private void _pointsCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            recalculatePoints();
        }

        virtual protected void recalculatePoints()
        {

        }

        #region Private Methods
        #endregion Private Methods
        #region Public Properties

        public string CurveType
        {
            get { return _curveType; }
            set { _curveType = value; }
        }
        public ICommand RemovePointsCommand { get { return _removePoints ?? (_removePoints = new ActionCommand(RemoveSelectedPoints)); } }
        
        public virtual ObservableCollection<Point> PointsCollection
        {
            get
            {
                return _pointsCollection;
            }
            set
            {
                _pointsCollection = value;
                _pointsCollection.CollectionChanged += _pointsCollection_CollectionChanged;
            }
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

        public void DrawBezierCurveStereoscopy(Matrix4d transformacja, ObservableCollection<Point> Points )
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
            GL.Begin(BeginMode.Lines);
            GL.Color3(0.6, 0.0, 0.0);
            int j = 0;

            ObservableCollection<Point> temp = new ObservableCollection<Point>();
            ObservableCollection<Point> transformedProjectedPoints = new ObservableCollection<Point>();
            foreach (var p in Points)
            {
                var a = projekcjaRight.Multiply(transformacja.Multiply(p));
                transformedProjectedPoints.Add(new Point(a.X, a.Y, a.Z));
            }

            foreach (var p in transformedProjectedPoints)
            {
                j++;
                temp.Add(p);
                if (j % 4 == 0 || p == transformedProjectedPoints.Last())
                {
                    double divisions = GetDivisions(temp);
                    var point = Casteljeu(temp, 0);
                    for (double t = divisions / 2; t <= 1; t += divisions / 2)
                    {
                        GL.Vertex2(point.X, point.Y);
                        point = Casteljeu(temp, t);
                        GL.Vertex2(point.X, point.Y);
                    }
                    temp.Clear();
                    temp.Add(p);
                    j = 1;
                }


            }

            GL.Color3(0.0, 0.0, 0.6);
            j = 0;
            ObservableCollection<Point> temp2 = new ObservableCollection<Point>();
            foreach (var p in Points)
            {
                j++;
                temp2.Add(p);
                if (j % 4 == 0 || p == PointsCollection.Last())
                {
                    double divisions = GetDivisions(transformacja, temp2);

                    var point = Casteljeu(temp2, 0);
                    var windowCoordinates = projekcjaLeft.Multiply(transformacja.Multiply(point));
                    for (double t = divisions / 2; t <= 1; t += divisions / 2)
                    {
                        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);

                        point = Casteljeu(temp2, t);
                        windowCoordinates = projekcjaLeft.Multiply(transformacja.Multiply(point));
                        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);


                    }
                    temp2.Clear();
                    temp2.Add(p);
                    j = 1;
                }
            }
            GL.End();
        }

        protected void DrawBezierCurve(Matrix4d transformacja, IEnumerable<Point> points)
        {
            GL.Begin(BeginMode.Lines);
            GL.Color3(1.0, 1.0, 1.0);

            ObservableCollection<Point> transformedProjectedPoints = new ObservableCollection<Point>();
            ObservableCollection<Point> temp = new ObservableCollection<Point>();
            foreach (var p in points)
            {
                var a = projekcjaRight.Multiply(transformacja.Multiply(p));
                transformedProjectedPoints.Add(new Point(a.X, a.Y, a.Z));
            }

            foreach (var p in transformedProjectedPoints)
            {
                temp.Add(p);
                if (temp.Count % 4 == 0 || p == transformedProjectedPoints.Last())
                {
                    var divisions = GetDivisions(transformacja, temp);
                    var point = Casteljeu(temp, 0);
                 
                    for (double t = divisions / 2; t <= 1; t += divisions / 2)
                    {
                        GL.Vertex2(point.X, point.Y);
                        point = Casteljeu(temp, t);
                        GL.Vertex2(point.X, point.Y);
                    }
                    temp.Clear();
                    temp.Add(p);
                }
            }
            GL.End();
        }

        protected Vector4d Casteljeu(ObservableCollection<Point> points, double t)
        {
            int counter = points.Count;
            while (points.Count < 4)
                points.Add(points.Last());
            var xValues = new Vector4d(points[0].X, points[1].X, points[2].X, points[3].X);
            var yValues = new Vector4d(points[0].Y, points[1].Y, points[2].Y, points[3].Y);
            var zValues = new Vector4d(points[0].Z, points[1].Z, points[2].Z, points[3].Z);

            for (int i = 0; i < counter; i++)
            {
                xValues.X = xValues.X * (1 - t) + (xValues.Y * t);
                xValues.Y = xValues.Y * (1 - t) + (xValues.Z * t);
                xValues.Z = xValues.Z * (1 - t) + (xValues.W * t);

                yValues.X = yValues.X * (1 - t) + (yValues.Y * t);
                yValues.Y = yValues.Y * (1 - t) + (yValues.Z * t);
                yValues.Z = yValues.Z * (1 - t) + (yValues.W * t);

                zValues.X = zValues.X * (1 - t) + (zValues.Y * t);
                zValues.Y = zValues.Y * (1 - t) + (zValues.Z * t);
                zValues.Z = zValues.Z * (1 - t) + (zValues.W * t);
            }

            return new Vector4d(xValues.X, yValues.X, zValues.X, 1);
        }

        public double GetDivisions(Matrix4d transformacja, ObservableCollection<Point> temp)
        {
            int j = temp.Count; 
            double length = 0;
            for (int i = 0; i < j - 1; i++)
            {
                Vector4d a = projekcja.Multiply(transformacja.Multiply(temp[i + 1])) -
                             projekcja.Multiply(transformacja.Multiply(temp[i]));
                a.X *= 1440;
                a.Y *= 750;
                length += a.Length;
            }

            return 0.1 / length;
        }

        public double GetDivisions(ObservableCollection<Point> temp)
        {
            int j = temp.Count; 
            double length = 0;
            for (int i = 0; i < j - 1; i++)
            {
                Vector4d a = temp[i + 1].Coordinates - temp[i].Coordinates;
                a.X *= 1440;
                a.Y *= 750;
                length += a.Length;
            }

            return 0.1 / length;
        }
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

        public void Refresh()
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

