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


    public class BezierPatchC2 : PointExchange, IPatch
    {
        public Point[,] _curvesPatchPoints;
        public Point[,] _curvesPatchPoints1;
        public Point[,] _patchPoints;


        public bool ShowControlPoints { get; set; } = true;
        public bool ShowBernstein { get; set; }
        public Point[,] PatchPoints
        {
            get { return _patchPoints; }
            set
            {
                _patchPoints = value;

            }
        }
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
        bool _polylineEnabled = false;
        public bool Selected { get; set; }
        public string Name { get; set; }
        static int PatchNumber { get; set; }
        Matrix4d projection = MatrixProvider.ProjectionMatrix();
        int multiplier = 3;


        public double[] U, V, UCurve, VCurve;
        private int _u = 4;
        private int _v = 4;

        public int u
        {
            get { return _u; }
            set
            {
                if ((value >= 1) && (value <= 20))
                {
                    _u = value;
                    CalculateParametrizationVectors();
                    CalculateCurvesPatchPoints();
                }
            }
        }

        public int v
        {
            get { return _v; }
            set
            {
                if ((value >= 1) && (value <= 20))
                {
                    _v = value;
                    CalculateParametrizationVectors();
                    CalculateCurvesPatchPoints();
                }
            }
        }

        double _rotationZ = 0;
        double _rotationZOld = 0;
        public double RotationZ
        {
            get { return _rotationZ; }
            set
            {
                _rotationZ = value;
                ModelowanieGeometryczne.Model.Point[,] PointsTemporaryCollection = PatchPoints;
                for (int i = 0; i < PointsTemporaryCollection.GetLength(0); i++)
                {
                    for (int j = 0; j < PointsTemporaryCollection.GetLength(1); j++)
                    {
                        PointsTemporaryCollection[i, j] = MatrixProvider.MultiplyP(MatrixProvider.RotateZMatrix((Math.PI / 180) * (_rotationZ - _rotationZOld)), PointsTemporaryCollection[i, j]);
                    }
                }


                PatchPoints = PointsTemporaryCollection;
                _rotationZOld = RotationZ;
                CalculateParametrizationVectors();
                CalculateBezierPoints();
            }
        }


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

                for (int i = 0; i < PatchPoints.GetLength(0); i++)
                {
                    for (int j = 0; j < PatchPoints.GetLength(1) - 1; j++)
                    {

                        var _windowCoordinates = projection.Multiply(transformacja.Multiply(PatchPoints[i, j].Coordinates));
                        GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);

                        _windowCoordinates = projection.Multiply(transformacja.Multiply(PatchPoints[i, j + 1].Coordinates));
                        GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);
                    }
                }

                for (int i = 0; i < PatchPoints.GetLength(0) - 1; i++)
                {
                    for (int j = 0; j < PatchPoints.GetLength(1); j++)
                    {

                        var _windowCoordinates = projection.Multiply(transformacja.Multiply(PatchPoints[i, j].Coordinates));
                        GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);

                        _windowCoordinates = projection.Multiply(transformacja.Multiply(PatchPoints[i + 1, j].Coordinates));
                        GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);
                    }
                }

                if (PatchesAreCylinder)
                {
                    for (int i = 0; i < PatchPoints.GetLength(0); i++)
                    {


                        var _windowCoordinates = projection.Multiply(transformacja.Multiply(PatchPoints[i, 0].Coordinates));
                        GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);

                        _windowCoordinates = projection.Multiply(transformacja.Multiply(PatchPoints[i, PatchPoints.GetLength(1) - 1].Coordinates));
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
                    int patchHorizontalDivision,
                    int patchVerticalDivision,
                    bool cylinder,
                    Point[,] pointsToAdd,
                     string name1
                    )
        {
            HorizontalPatches = horizontalPatches;
            VerticalPatches = verticalPatches;
            _u = patchHorizontalDivision;
            _v = patchVerticalDivision;
            PatchesAreCylinder = cylinder;
            PatchPoints = pointsToAdd;
            Name = name1;

            _additionalPoints = new Point[1 + 3 * VerticalPatches, 1 + 3 * HorizontalPatches];
            _additionalPoints2 = new Point[verticalPatches + 3, 1 + 3 * HorizontalPatches];

            CalculateParametrizationVectors();
            CalculateBezierPoints();
        }


        public BezierPatchC2(
                int horizontalPatches,
                 int verticalPatches,
                 double patchWidth,
                 double patchHeight,
                 int patchHorizontalDivision,
                 int patchVerticalDivision,
                 bool patchesAreCylinder,
                 Vector4d startPoint
               )
        {
            StartPoint = startPoint;
            HorizontalPatches = horizontalPatches;
            VerticalPatches = verticalPatches;
            PatchWidth = patchWidth;
            PatchHeight = patchHeight;
            // PatchHorizontalDivision = patchHorizontalDivision;
            //PatchVerticalDivision = patchVerticalDivision;
            PatchesAreCylinder = patchesAreCylinder;


            _u = patchVerticalDivision;
            _v = patchHorizontalDivision;


            _patchPoints = new Point[verticalPatches + 3, horizontalPatches + 3];
            _additionalPoints = new Point[1 + 3 * VerticalPatches, 1 + 3 * HorizontalPatches];


            _additionalPoints2 = new Point[verticalPatches + 3, 1 + 3 * HorizontalPatches];
            //_additionalPoints2 = new Point[3 + VerticalPatches, 3 + horizontalPatches];
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
            CalculateParametrizationVectors();
            CalculateBezierPoints();

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
            double dx = PatchWidth / (3 + HorizontalPatches - 1);
            double dy = PatchHeight / (3 + VerticalPatches - 1);

            // var temp = new Point[4, 4];


            for (int i = 0; i < VerticalPatches + 3; i++)
            {//i pionowe płatki :
                for (int j = 0; j < HorizontalPatches + 3; j++)
                {//j iteracja pozioma, poziome płatki ..



                    _patchPoints[i, j] = new Point(StartPoint.X + j * dx, StartPoint.Y + i * dy, StartPoint.Z);



                }
            }
        }
        public void CalculateParametrizatioCurveVectors()
        {
            var uCurve = u * multiplier;
            var vCurve = v * multiplier;
            UCurve = new double[uCurve];
            VCurve = new double[vCurve];

            double deltaU = 0;
            double deltaV = 0;

            if (uCurve > 1)
            {
                deltaU = 1.0 / (uCurve - 1);
            }
            if (uCurve == 1)
            {
                deltaU = 1;
            }

            for (int i = 0; i < uCurve; i++)
            {
                UCurve[i] = i * deltaU;
            }



            if (vCurve > 1)
            {
                deltaV = 1.0 / (vCurve - 1);
            }
            if (vCurve == 1)
            {
                deltaV = 1;
            }

            for (int i = 0; i < vCurve; i++)
            {
                VCurve[i] = i * deltaV;
            }

        }

        public void CalculateParametrizationVectors()
        {
            U = new double[u];
            V = new double[v];

            double deltaU = 0;
            double deltaV = 0;

            if (u > 1)
            {
                deltaU = 1.0 / (u - 1);
            }
            if (u == 1)
            {
                deltaU = 1;
            }

            for (int i = 0; i < u; i++)
            {
                U[i] = i * deltaU;
            }



            if (v > 1)
            {
                deltaV = 1.0 / (v - 1);
            }
            if (v == 1)
            {
                deltaV = 1;
            }

            for (int i = 0; i < v; i++)
            {
                V[i] = i * deltaV;
            }
            CalculateParametrizatioCurveVectors();

        }

        public Point[,] Copy4x4PieceOfPointsCollecion(int VerticalMove, int HorizontalMove)
        {

            var temp = new Point[4, 4];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    temp[i, j] = _additionalPoints[i + VerticalMove, j + HorizontalMove];
                }
            }
            return temp;
        }

        public Point[,] Copy4x4PieceOfPointsCollecionTransposed(int HorizontalMove, int VerticalMove)
        {

            var temp = new Point[4, 4];



            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    temp[j, i] = _additionalPoints[i + VerticalMove, j + HorizontalMove];
                }
            }


            return temp;
        }


        public void CalculateCurvesPatchPoints()
        { //TODO: Przerobić

            Point[,] _pointsToDrawSinglePatch = new Point[4, 4];

            if (PatchesAreCylinder)
            {
                //_curvesPatchPoints = new Point[1 + (_u - 1) * VerticalPatches, 1 + (_v - 1) * (HorizontalPatches + 3)];
                //int ii = 0;
                //int jj = 0;

                //for (ii = 0; ii < VerticalPatches; ii++)
                //{
                //    for (jj = 0; jj < HorizontalPatches + 3; jj++)
                //    {

                //        _pointsToDrawSinglePatch = Copy4x4PieceOfPointsCollecion(3 * ii, 3 * jj);
                //        for (int i = 0; i < U.Length; i++)
                //        {
                //            for (int j = 0; j < V.Length; j++)
                //            {
                //                _curvesPatchPoints[(_u - 1) * ii + i, (_v - 1) * jj + j] = MatrixProvider.Multiply(CalculateB(U[i]), _pointsToDrawSinglePatch, CalculateB(V[j]));
                //            }
                //        }


                //    }
                //}
                _curvesPatchPoints = new Point[1 + (_u - 1) * VerticalPatches, 1 + (_v * multiplier - 1) * (HorizontalPatches + 3)];
                _curvesPatchPoints1 = new Point[(1 + (_u * multiplier - 1) * VerticalPatches), 1 + (_v - 1) * (HorizontalPatches + 3)];


                for (int ii = 0; ii < VerticalPatches; ii++)
                {
                    for (int jj = 0; jj < HorizontalPatches + 3; jj++)
                    {

                        _pointsToDrawSinglePatch = Copy4x4PieceOfPointsCollecion(3 * ii, 3 * jj);
                        for (int i = 0; i < U.Length; i++)
                        {
                            for (int j = 0; j < VCurve.Length; j++)
                            {
                                _curvesPatchPoints[(_u - 1) * ii + i, (_v * multiplier - 1) * jj + j] = MatrixProvider.Multiply(CalculateB(U[i]), _pointsToDrawSinglePatch, CalculateB(VCurve[j]));
                            }
                        }


                        for (int i = 0; i < UCurve.Length; i++)
                        {
                            for (int j = 0; j < V.Length; j++)
                            {
                                _curvesPatchPoints1[(_u * multiplier - 1) * ii + i, (_v - 1) * jj + j] = MatrixProvider.Multiply(CalculateB(UCurve[i]), _pointsToDrawSinglePatch, CalculateB(V[j]));
                            }
                        }


                    }
                }
            }

            else
            {
                _curvesPatchPoints = new Point[1 + (_u - 1) * VerticalPatches, (1 + (_v * multiplier - 1) * HorizontalPatches)];
                _curvesPatchPoints1 = new Point[(1 + (_u * multiplier - 1) * VerticalPatches), (1 + (_v - 1) * HorizontalPatches)];


                for (int ii = 0; ii < VerticalPatches; ii++)
                {
                    for (int jj = 0; jj < HorizontalPatches; jj++)
                    {

                        _pointsToDrawSinglePatch = Copy4x4PieceOfPointsCollecion(3 * ii, 3 * jj);
                        for (int i = 0; i < U.Length; i++)
                        {
                            for (int j = 0; j < VCurve.Length; j++)
                            {
                                _curvesPatchPoints[(_u - 1) * ii + i, (_v * multiplier - 1) * jj + j] = MatrixProvider.Multiply(CalculateB(U[i]), _pointsToDrawSinglePatch, CalculateB(VCurve[j]));
                            }
                        }


                        for (int i = 0; i < UCurve.Length; i++)
                        {
                            for (int j = 0; j < V.Length; j++)
                            {
                                _curvesPatchPoints1[(_u * multiplier - 1) * ii + i, (_v - 1) * jj + j] = MatrixProvider.Multiply(CalculateB(UCurve[i]), _pointsToDrawSinglePatch, CalculateB(V[j]));
                            }
                        }


                    }
                }
            }
            // GL.End();
        }



        public void DrawPatch(Matrix4d transformacja)
        {


            GL.Begin(BeginMode.Lines);
            GL.Color3(1.0, 1.0, 1.0);


            for (int i = 0; i < _curvesPatchPoints1.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < _curvesPatchPoints1.GetLength(1); j++)
                {

                    // if (_curvesPatchPoints[i + 1, j] == null || _curvesPatchPoints[i, j] == null) break;

                    var _windowCoordinates = projection.Multiply(transformacja.Multiply(_curvesPatchPoints1[i, j].Coordinates));
                    GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);


                    _windowCoordinates = projection.Multiply(transformacja.Multiply(_curvesPatchPoints1[i + 1, j].Coordinates));

                    GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);
                }

            }




            for (int i = 0; i < _curvesPatchPoints.GetLength(0); i++)
            {
                for (int j = 0; j < _curvesPatchPoints.GetLength(1) - 1; j++)
                {
                    //if (_curvesPatchPoints[i , j+1] == null || _curvesPatchPoints[i, j] == null) break;


                    var _windowCoordinates = projection.Multiply(transformacja.Multiply(_curvesPatchPoints[i, j].Coordinates));
                    GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);

                    _windowCoordinates = projection.Multiply(transformacja.Multiply(_curvesPatchPoints[i, j + 1].Coordinates));
                    GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);
                }

            }


            GL.End();
        }

        public double[] CalculateB(double u)
        {

            return new double[4] { (1 - u) * (1 - u) * (1 - u), 3 * u * (1 - u) * (1 - u), 3 * u * u * (1 - u), u * u * u };
        }


        public void AddPoints(Point a, Point b, List<Point> list,  double minDensity = 0.005)
        {
            int i = 1;
            int elementsNumber;
            elementsNumber =Math.Max((int) ((b - a).Length() / minDensity),1);
            list.Add(a);
            // list.Add(b);
            Point delta = (b - a) / i;
            Point temp=new Point(a.X, a.Y, a.Z);
            for (int j = 0; j < i; j++)
            {

                temp += delta;
                list.Add(temp);
            }
        }
        public List<Point> GeneratePointsForMilling()
        {

            List<Point> PointsList=new List<Point>();


            for (int i = 0; i < _curvesPatchPoints1.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < _curvesPatchPoints1.GetLength(1); j++)
                {

                    // if (_curvesPatchPoints[i + 1, j] == null || _curvesPatchPoints[i, j] == null) break;

                    var p1 = _curvesPatchPoints1[i, j].Coordinates;


                    var p2 = _curvesPatchPoints1[i + 1, j].Coordinates;

                    AddPoints(new Point(p1), new Point (p2), PointsList);
                }

            }




            for (int i = 0; i < _curvesPatchPoints.GetLength(0); i++)
            {
                for (int j = 0; j < _curvesPatchPoints.GetLength(1) - 1; j++)
                {
                    //if (_curvesPatchPoints[i , j+1] == null || _curvesPatchPoints[i, j] == null) break;


                    var p1 = _curvesPatchPoints[i, j].Coordinates;

                    var p2 = _curvesPatchPoints[i, j + 1].Coordinates;

                    AddPoints(new Point(p1), new Point(p2), PointsList);
                }

            }

            return PointsList;
        }


        
        public void CalculateBezierPoints()
        {
            //Fragment do wyznaczania punktów beziera
            //Wywoływany w konstruktorze klasy i przez metodę Scene.MoveSelectedPoints

            if (PatchesAreCylinder)
            {
                var PatchPoints2 = new Point[VerticalPatches + 3, HorizontalPatches + 3 + 3];
                _additionalPoints2 = new Point[VerticalPatches + 3, 1 + 9 + 3 * HorizontalPatches];
                _additionalPoints = new Point[1 + 3 * VerticalPatches, 1 + 9 + 3 * HorizontalPatches];
                for (int i = 0; i < VerticalPatches + 3; i++)
                {
                    for (int j = 0; j < HorizontalPatches + 3; j++)
                    {
                        PatchPoints2[i, j] = PatchPoints[i, j];
                    }
                }

                for (int i = 0; i < VerticalPatches + 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        PatchPoints2[i, j + HorizontalPatches + 3] = PatchPoints[i, j];
                    }
                }


                for (int k = 0; k < PatchPoints2.GetLength(0); k++)
                {

                    var tempCollection = new ObservableCollection<Point>();
                    for (int i = 0; i < PatchPoints2.GetLength(1); i++)
                    {
                        tempCollection.Add(PatchPoints2[k, i]);
                    }
                    int d = 0;
                    CalculateAdditionalPoints(tempCollection);
                    foreach (var item in _additionalPointsCollection2)
                    {
                        _additionalPoints2[k, d] = item;
                        d++;
                    }
                }

                _additionalPointsCollection2.Clear();

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
            }
            else
            {
                for (int k = 0; k < PatchPoints.GetLength(0); k++)
                {

                    var tempCollection = new ObservableCollection<Point>();
                    for (int i = 0; i < PatchPoints.GetLength(1); i++)
                    {
                        tempCollection.Add(PatchPoints[k, i]);
                    }

                    int d = 0;

                    CalculateAdditionalPoints(tempCollection);
                    foreach (var item in _additionalPointsCollection2)
                    {
                        _additionalPoints2[k, d] = item;
                        d++;
                    }
                }


                _additionalPointsCollection2.Clear();

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
            }





            CalculateCurvesPatchPoints();
        }
        public void DrawPoints(Matrix4d transformacja)
        {
            if (ShowControlPoints)
            {
                for (int i = 0; i < PatchPoints.GetLength(0); i++)
                {
                    for (int j = 0; j < PatchPoints.GetLength(1); j++)
                    {
                        PatchPoints[i, j].Draw(transformacja, 5);
                    }
                }
            }


            //TODO: Przenieść do funkcji rysującej płatek
            //CalculateBezierPoints();



            //Rysowanie punktów
            if (ShowBernstein)
            {
                for (int i = 0; i < _additionalPoints.GetLength(0); i++)
                {
                    for (int j = 0; j < _additionalPoints.GetLength(1); j++)
                    {
                        _additionalPoints[i, j].Draw(transformacja, 4);
                    }
                }
            }
        }
        public double[] GetPatchNumber(double u, double v)
        {
            double borderX;
            if (PatchesAreCylinder)
            {
                borderX = 1.0 / (3 + HorizontalPatches);
            }
            else
            {
                borderX = 1.0 / HorizontalPatches;
            }

            double borderY = 1.0 / VerticalPatches;


            double[] surfaceCoordinates = new double[4];

            surfaceCoordinates[0] = (int)(u / borderX);
            surfaceCoordinates[1] = (int)(v / borderY);

            if (PatchesAreCylinder)
            {
                if ((int)surfaceCoordinates[0] == 3+HorizontalPatches) surfaceCoordinates[0] = surfaceCoordinates[0] - 1;
            }
            else
            {
                if ((int)surfaceCoordinates[0] == HorizontalPatches) surfaceCoordinates[0] = surfaceCoordinates[0] - 1;
            }



            if ((int)surfaceCoordinates[1] == VerticalPatches) surfaceCoordinates[1] = surfaceCoordinates[1] - 1;

            surfaceCoordinates[2] = (u - borderX * surfaceCoordinates[0]) / borderX;
            surfaceCoordinates[3] = (v - borderY * surfaceCoordinates[1]) / borderY;

            return surfaceCoordinates;
        }

        public Point GetPoint(double u, double v)
        {
            double[] coord = GetPatchNumber(u, v);
            var _pointsToDrawSinglePatch = Copy4x4PieceOfPointsCollecionTransposed(3 * (int)coord[0], 3 * (int)coord[1]);
            return MatrixProvider.Multiply(CalculateB(coord[2]), _pointsToDrawSinglePatch, CalculateB(coord[3]));

        }

        public double[] CalculateDerrivativeB(double u)
        {
            return new double[4] { -3 * (u - 1) * (u - 1), 3 * u * (2 * u - 2) + 3 * (u - 1) * (u - 1), -6 * u * (u - 1) - 3 * u * u, 3 * u * u };
        }

        public Point GetPointDerivativeU(double u, double v)
        {
            double[] coord = GetPatchNumber(u, v);
            var pointsToDrawSinglePatch = Copy4x4PieceOfPointsCollecionTransposed(3 * (int)coord[0], 3 * (int)coord[1]);
            return MatrixProvider.Multiply(CalculateDerrivativeB(coord[2]), pointsToDrawSinglePatch, CalculateB(coord[3]));
        }

        public Point GetPointDerivativeV(double u, double v)
        {
            double[] coord = GetPatchNumber(u, v);
            var pointsToDrawSinglePatch = Copy4x4PieceOfPointsCollecionTransposed(3 * (int)coord[0], 3 * (int)coord[1]);
            return MatrixProvider.Multiply(CalculateB(coord[2]), pointsToDrawSinglePatch, CalculateDerrivativeB(coord[3]));
        }
    }
}
