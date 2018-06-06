using ModelowanieGeometryczne.Model;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using ModelowanieGeometryczne.Helpers;

namespace ModelowanieGeometryczne
{


    public class BezierPatchC2
    {

        public Point[,] _patchPoints;
        public Point[,] _additionalPoints;
        public Point[,] _additionalPoints2;

        //  public Patch[,] Surface;

        private ObservableCollection<Point> _additionalPointsCollection = new ObservableCollection<Point>();
        private ObservableCollection<Point> _additionalPointsCollection2 = new ObservableCollection<Point>();

        public int HorizontalPatches { get; set; }
        public int VerticalPatches { get; set; }
        public double PatchWidth { get; set; }
        public double PatchHeight { get; set; }
        private int _patchHorizontalDivision = 4;
        private int _patchVerticalDivision = 4;
        public bool PatchesAreCylinder { get; set; }
        public Vector4d StartPoint { get; set; }
        // private ObservableCollection<Point> _vertices = new ObservableCollection<Point>();
        bool _polylineEnabled = true;
        public bool Selected { get; set; }
        public string Name { get; set; }
        static int PatchNumber { get; set; }
        Matrix4d projection = MatrixProvider.ProjectionMatrix();

        private void CalculateAdditionalPoints(ObservableCollection<Point> PointsCollection)
        {


            _additionalPointsCollection.Clear();
            for (int i = 0; i < PointsCollection.Count - 1; i++)
            {
                _additionalPointsCollection.Add(new Point(PointsCollection[i].X + (PointsCollection[i + 1].X - PointsCollection[i].X) / 3, PointsCollection[i].Y + (PointsCollection[i + 1].Y - PointsCollection[i].Y) / 3, PointsCollection[i].Z + (PointsCollection[i + 1].Z - PointsCollection[i].Z) / 3));
                _additionalPointsCollection.Add(new Point((PointsCollection[i].X + 2 * (PointsCollection[i + 1].X - PointsCollection[i].X) / 3), (PointsCollection[i].Y + 2 * (PointsCollection[i + 1].Y - PointsCollection[i].Y) / 3), (PointsCollection[i].Z + 2 * (PointsCollection[i + 1].Z - PointsCollection[i].Z) / 3)));
            }

            _additionalPointsCollection2.Clear();

            int k = 1;


            _additionalPointsCollection2.Add(new Point(_additionalPointsCollection[k].X + (_additionalPointsCollection[k + 1].X - _additionalPointsCollection[k].X) / 2, _additionalPointsCollection[k].Y + (_additionalPointsCollection[k + 1].Y - _additionalPointsCollection[k].Y) / 2, _additionalPointsCollection[k].Z + (_additionalPointsCollection[k + 1].Z - _additionalPointsCollection[k].Z) / 2));
            _additionalPointsCollection2.Add(_additionalPointsCollection[k + 1]);
            for (k = 3; k < _additionalPointsCollection.Count - 3; k += 2)
            {
                _additionalPointsCollection2.Add(_additionalPointsCollection[k]);
                _additionalPointsCollection2.Add(new Point(_additionalPointsCollection[k].X + (_additionalPointsCollection[k + 1].X - _additionalPointsCollection[k].X) / 2, _additionalPointsCollection[k].Y + (_additionalPointsCollection[k + 1].Y - _additionalPointsCollection[k].Y) / 2, _additionalPointsCollection[k].Z + (_additionalPointsCollection[k + 1].Z - _additionalPointsCollection[k].Z) / 2));
                _additionalPointsCollection2.Add(_additionalPointsCollection[k + 1]);
            }

            _additionalPointsCollection2.Add(_additionalPointsCollection[k]);
            _additionalPointsCollection2.Add(new Point(_additionalPointsCollection[k].X + (_additionalPointsCollection[k + 1].X - _additionalPointsCollection[k].X) / 2, _additionalPointsCollection[k].Y + (_additionalPointsCollection[k + 1].Y - _additionalPointsCollection[k].Y) / 2, _additionalPointsCollection[k].Z + (_additionalPointsCollection[k + 1].Z - _additionalPointsCollection[k].Z) / 2));


            //int d=0;
            ////_additionalPoints.Clear();

            //for (int i = 0; i < _patchPoints.GetLength(1) - 1; i++)
            //{
            //    _additionalPoints[0,d]=(new Point(_patchPoints[0, i].X + (_patchPoints[0, i + 1].X - _patchPoints[0,i].X) / 3, _patchPoints[0, i].Y + (_patchPoints[0, i + 1].Y - _patchPoints[0, i].Y) / 3, _patchPoints[0,i].Z + (_patchPoints[0,i + 1].Z - _patchPoints[0,i].Z) / 3));
            //    _additionalPoints[0,d+1]=(new Point((_patchPoints[0,i].X + 2 * (_patchPoints[0, i + 1].X - _patchPoints[0,i].X) / 3), (_patchPoints[0,i].Y + 2 * (_patchPoints[0, i + 1].Y - _patchPoints[0,i].Y) / 3), (_patchPoints[0,i].Z + 2 * (_patchPoints[0,i + 1].Z - _patchPoints[0,i].Z) / 3)));
            //    d += 2;
            //}

            ////_additionalPointsCollection2.Clear();

            //int k = 1;
            //int j = 2;

            //_additionalPoints2[0,0]=(new Point(_additionalPoints[0,k].X + (_additionalPoints[0,k + 1].X - _additionalPoints[0,k].X) / 2, _additionalPoints[0,k].Y + (_additionalPoints[0,k + 1].Y - _additionalPoints[0,k].Y) / 2, _additionalPoints[0,k].Z + (_additionalPoints[0,k + 1].Z - _additionalPoints[0,k].Z) / 2));
            //_additionalPoints2[0,1]=(_additionalPoints[0,k + 1]);
            //for (k = 3; k < _additionalPoints.GetLength(0) - 3; k += 2)
            //{

            //    _additionalPoints2[0,j]=(_additionalPoints[0,k]);
            //    _additionalPoints2[0, j+1]=(new Point(_additionalPoints[0,k].X + (_additionalPoints[0,k + 1].X - _additionalPoints[0,k].X) / 2, _additionalPoints[0,k].Y + (_additionalPoints[0,k + 1].Y - _additionalPoints[0,k].Y) / 2, _additionalPoints[0,k].Z + (_additionalPoints[0,k + 1].Z - _additionalPoints[0,k].Z) / 2));
            //    _additionalPoints2[0, j+2]=(_additionalPoints[0,k + 1]);
            //    j += 3;
            //}

            //_additionalPoints2[0,j]=(_additionalPoints[0,k]);
            //_additionalPoints2[0,j+1]=(new Point(_additionalPoints[0,k].X + (_additionalPoints[0,k + 1].X - _additionalPoints[0,k].X) / 2, _additionalPoints[0,k].Y + (_additionalPoints[0,k + 1].Y - _additionalPoints[0,k].Y) / 2, _additionalPoints[0,k].Z + (_additionalPoints[0,k + 1].Z - _additionalPoints[0,k].Z) / 2));


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

        public void DrawPolyline(Matrix4d transformacja)
        {
            if (PolylineEnabled)
            {
                GL.Begin(BeginMode.Lines);
                GL.Color3(1.0, 1.0, 1.0);

                for (int i = 0; i < _patchPoints.GetLength(0); i++)
                {
                    for (int j = 0; j < _patchPoints.GetLength(1) - 1; j++)
                    {

                        var _windowCoordinates = projection.Multiply(transformacja.Multiply(_patchPoints[i, j].Coordinates));
                        GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);

                        _windowCoordinates = projection.Multiply(transformacja.Multiply(_patchPoints[i, j + 1].Coordinates));
                        GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);
                    }
                }

                for (int i = 0; i < _patchPoints.GetLength(0) - 1; i++)
                {
                    for (int j = 0; j < _patchPoints.GetLength(1); j++)
                    {

                        var _windowCoordinates = projection.Multiply(transformacja.Multiply(_patchPoints[i, j].Coordinates));
                        GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);

                        _windowCoordinates = projection.Multiply(transformacja.Multiply(_patchPoints[i + 1, j].Coordinates));
                        GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);
                    }
                }

                if (PatchesAreCylinder)
                {
                    for (int i = 0; i < _patchPoints.GetLength(0); i++)
                    {


                        var _windowCoordinates = projection.Multiply(transformacja.Multiply(_patchPoints[i, 0].Coordinates));
                        GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);

                        _windowCoordinates = projection.Multiply(transformacja.Multiply(_patchPoints[i, _patchPoints.GetLength(1) - 1].Coordinates));
                        GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);

                    }
                }
                GL.End();
            }
        }

        public int PatchHorizontalDivision
        {
            get
            {
                return _patchHorizontalDivision;
            }
            set
            {
                if ((value >= 3) && (value < 20))
                {
                    _patchHorizontalDivision = value;
                }
            }
        }
        public int PatchVerticalDivision
        {
            get
            {
                return _patchVerticalDivision;
            }
            set
            {
                if ((value >= 3) && (value < 20))
                {
                    _patchVerticalDivision = value;
                }
            }
        }

        public BezierPatchC2(
            int horizontalPatches,
             int verticalPatches,
             double patchWidth,
             double patchHeight,
             int patchHorizontalDivision,
             int patchVerticalDivision,
             bool patchesAreCylinder,
             Vector4d startPoint)
        {
            StartPoint = startPoint;
            HorizontalPatches = horizontalPatches;
            VerticalPatches = verticalPatches;
            PatchWidth = patchWidth;
            PatchHeight = patchHeight;
            // PatchHorizontalDivision = patchHorizontalDivision;
            //PatchVerticalDivision = patchVerticalDivision;
            PatchesAreCylinder = patchesAreCylinder;




            _patchPoints = new Point[verticalPatches + 3, horizontalPatches + 3];
            _additionalPoints = new Point[1 + 3 * VerticalPatches, 1 + 3 * horizontalPatches];


            //_additionalPoints2 = new Point[1 + 3 * VerticalPatches, 1 + 3 * horizontalPatches];
            _additionalPoints2 = new Point[3 + VerticalPatches, 3 + horizontalPatches];
            if (patchesAreCylinder)
            {
                SetUpVerticesCylinder();
            }

            else
            {
                SetUpPatchVertices();
            }

            PatchNumber++;
            Name = "Bezier patch number " + PatchNumber + " type: C2";

        }

        private void SetUpVerticesCylinder()
        {

            double dy = PatchHeight / VerticalPatches;
            double alpha = (Math.PI * 2.0f) / (HorizontalPatches + 3);

            for (int i = 0; i < (VerticalPatches + 3); i++)
            {//i pionowe płatki :
                for (int j = 0; j < (HorizontalPatches + 3); j++)
                {//j pionowe płatki :
                    //patchwidth is radius when cylinder is set
                    var point = new Point(StartPoint.X + PatchWidth * Math.Cos(alpha * j), StartPoint.Y + (i * dy), StartPoint.Z + PatchWidth * Math.Sin(alpha * j));
                    _patchPoints[i, j] = point;
                }
            }
        }

        private void SetUpPatchVertices()
        {
            double dx = PatchWidth / (3+HorizontalPatches-1);
            double dy = PatchHeight / (3+VerticalPatches-1);

            // var temp = new Point[4, 4];


            for (int i = 0; i < VerticalPatches + 3; i++)
            {//i pionowe płatki :
                for (int j = 0; j < HorizontalPatches + 3; j++)
                {//j iteracja pozioma, poziome płatki ..



                    _patchPoints[i, j] = new Point(StartPoint.X + j * dx, StartPoint.Y + i * dy, StartPoint.Z);



                }
            }
        }


        public void DrawPoints(Matrix4d transformacja)
        {
           
             for (int i = 0; i < _patchPoints.GetLength(0); i++)
            {
                for (int j = 0; j < _patchPoints.GetLength(1); j++)
                {
                    _patchPoints[i, j].Draw(transformacja);
                }
            }



            //Fragment do wyznaczania punktów beziera
            //Wywoływać tylko przy zmianie _pathPoints!!!!!!!!!!

            for (int k = 0; k < _patchPoints.GetLength(0); k++)
            {

                var tempCollection = new ObservableCollection<Point>();
                for (int i = 0; i < _patchPoints.GetLength(1); i++)
                {
                    tempCollection.Add(_patchPoints[k, i]);
                }
                int d = 0;
                CalculateAdditionalPoints(tempCollection);
                foreach (var item in _additionalPointsCollection2)
                {
                    _additionalPoints2[k, d] = item;
                    d++;
                }
            }




            for (int k = 0; k < _additionalPoints2.GetLength(1); k++)
            {

                var tempCollection = new ObservableCollection<Point>();

                for (int i = 0; i < _additionalPoints2.GetLength(0); i++)
                {
                    tempCollection.Add(_additionalPoints2[i, k]);
                }
                int d = 0;
                CalculateAdditionalPoints(tempCollection);

                foreach (var item in _additionalPointsCollection2)
                {//Może warto zmienić kolejność indeksów na d,k teraz k,d?
                    _additionalPoints[d, k] = item;
                    d++;
                }
            }



            //Rysowanie punktów

            for (int i = 0; i < _additionalPoints.GetLength(0); i++)
            {
                for (int j = 0; j < _additionalPoints.GetLength(1); j++)
                {
                    _additionalPoints[i, j].Draw(transformacja);
                }
            }
        }
    }
}
