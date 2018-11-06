using ModelowanieGeometryczne.Helpers;
using ModelowanieGeometryczne.ViewModel;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace ModelowanieGeometryczne.Model
{
    public class Patch : ViewModelBase
    {
        public Point[,] _curvesPatchPoints;
        public Point[,] _curvesPatchPoints1;
        Point[,] _patchPoints;
        private int _u, _v; //u-pionowe v-poziome
        public double[] U, V, UCurve, VCurve;
        public Point[,] CalculatedPoints;
        Matrix4d projection = MatrixProvider.ProjectionMatrix();
        int multiplierU = 5;
        int multiplierV = 5;
        public int u
        {
            get { return _u; }
            set
            {
                _u = value;
                CalculateParametrizationVectors();
                CalculateCurvesPatchPoints();

            }
        }

        public int v
        {
            get { return _v; }
            set
            {
                _v = value;
                CalculateParametrizationVectors();
                CalculateCurvesPatchPoints();
            }
        }





        public Patch(Point[,] a, int uu = 10, int vv = 10)
        {
            _u = uu;
            _v = vv;
            _patchPoints = a;
            CalculateParametrizationVectors();
            CalculateCurvesPatchPoints();

        }

        public Patch()
        {
            _patchPoints = new Point[4, 4];

        }

        public Point[,] PatchPoints
        {
            get
            {
                return _patchPoints;
            }
            set
            {
                _patchPoints = value;

            }
        }

        public void DrawPoints(Matrix4d transformacja)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    _patchPoints[i, j].Draw(transformacja);
                }
            }
        }

        public void DrawPolyline(Matrix4d transformacja)
        {
            GL.Begin(BeginMode.Lines);
            GL.Color3(1.0, 1.0, 1.0);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4 - 1; j++)
                {

                    var _windowCoordinates = projection.Multiply(transformacja.Multiply(PatchPoints[i, j].Coordinates));
                    GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);

                    _windowCoordinates = projection.Multiply(transformacja.Multiply(PatchPoints[i, j + 1].Coordinates));
                    GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);
                }
            }

            for (int i = 0; i < 4 - 1; i++)
            {
                for (int j = 0; j < 4; j++)
                {

                    var _windowCoordinates = projection.Multiply(transformacja.Multiply(PatchPoints[i, j].Coordinates));
                    GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);

                    _windowCoordinates = projection.Multiply(transformacja.Multiply(PatchPoints[i + 1, j].Coordinates));
                    GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);
                }
            }

            GL.End();
        }

        public Point[,] Transpose(Point[,] a)
        {
            var a1 = a.GetLength(0);
            var a2 = a.GetLength(1);
            Point[,] temp= new Point[a2,a1];
            for (int i = 0; i < a1; i++)
            {
                for (int j = 0; j < a2; j++)
                {
                    temp[j, i] = a[i, j];
                }
            }

            return temp;
        }

        public Point GetPoint(double u, double v)
        {
            Point[,] _pointsToDrawSinglePatch = Transpose(PatchPoints);
            return MatrixProvider.Multiply(CalculateB(u), _pointsToDrawSinglePatch, CalculateB(v));
        }

        public Point GetPointGregory(double u, double v)
        {
            Point[,] _pointsToDrawSinglePatch = (PatchPoints);
            return MatrixProvider.Multiply(CalculateB(u), _pointsToDrawSinglePatch, CalculateB(v));
        }

        public double[] CalculateDerrivativeB(double u)
        {
            return new double[4] { -3 * (u - 1) * (u - 1), 3 * u * (2 * u - 2) + 3 * (u - 1) * (u - 1), -6 * u * (u - 1) - 3 * u * u, 3 * u * u };
        }

        public Point GetPointDerrivativeU(double u, double v)
        {
            Point[,] bezierPatch1 = Transpose(PatchPoints);
            return MatrixProvider.Multiply(CalculateDerrivativeB(u), bezierPatch1, CalculateB(v));
        }

        public Point GetPointDerrivativeV(double u, double v)
        {
            Point[,] bezierPatch1 = Transpose(PatchPoints);
            return MatrixProvider.Multiply(CalculateB(u), bezierPatch1, CalculateDerrivativeB(v));
        }

        public void CalculateCurvesPatchPoints()
        {
            int multiplier = multiplierU;
            const int VerticalPatches = 1;
            const int HorizontalPatches = 1;
            Point[,] _pointsToDrawSinglePatch = PatchPoints;

            _curvesPatchPoints = new Point[1 + (_u - 1) * VerticalPatches, (1 + (_v * multiplier - 1) * HorizontalPatches)];
            _curvesPatchPoints1 = new Point[(1 + (_u * multiplier - 1) * VerticalPatches), (1 + (_v - 1) * HorizontalPatches)];


            for (int ii = 0; ii < VerticalPatches; ii++)
            {
                for (int jj = 0; jj < HorizontalPatches; jj++)
                {

                    //_pointsToDrawSinglePatch = PatchPoints;
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

        public void AddPoints(Point a, Point b, List<Point> list, double minDensity = 0.005)
        {
            int i = 1;
            int elementsNumber;
            elementsNumber = Math.Max((int)((b - a).Length() / minDensity), 1);
            list.Add(a);
            // list.Add(b);
            Point delta = (b - a) / i;
            Point temp = new Point(a.X, a.Y, a.Z);
            for (int j = 0; j < i; j++)
            {

                temp += delta;
                list.Add(temp);
            }
        }

        public void GeneratePointsForMilling(List<Point> PointsList)
        {




            for (int i = 0; i < _curvesPatchPoints1.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < _curvesPatchPoints1.GetLength(1); j++)
                {

                    // if (_curvesPatchPoints[i + 1, j] == null || _curvesPatchPoints[i, j] == null) break;

                    var p1 = _curvesPatchPoints1[i, j].Coordinates;


                    var p2 = _curvesPatchPoints1[i + 1, j].Coordinates;

                    AddPoints(new Point(p1), new Point(p2), PointsList);
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

        public void CalculateParametrizatioCurveVectors()
        {
            var uCurve = u * multiplierU;
            var vCurve = v * multiplierV;
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




        public void SetParametrization(int _u, int _v)
        {
            u = _u;
            v = _v;
            CalculateParametrizationVectors();
        }

        public double[] CalculateB(double u)
        {

            return new double[4] { (1 - u) * (1 - u) * (1 - u), 3 * u * (1 - u) * (1 - u), 3 * u * u * (1 - u), u * u * u };
        }

        //public Matrix4d ConvertArrayPointstoMatrix(Point[,] Array)
        //{
        //    return new Matrix4d(Array[0, 0], Array[0, 1], Array[0, 2], Array[0, 3], Array[1, 0], Array[1,1], Array[1, 2], Array[1, 3], Array[2, 0], Array[2, 1], Array[2, 2], Array[2, 3], Array[3, 0], Array[3, 1], Array[3, 2], Array[3, 3]);
        //}
    }
}
