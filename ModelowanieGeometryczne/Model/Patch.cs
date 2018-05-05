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
        Point[,] _patchPoints;
        public int u, v;
        public double[] U, V;
        public Point[,] CalculatedPoints;
        Matrix4d projection = MatrixProvider.ProjectionMatrix();


        public Patch(Point[,] a, int _u = 10, int _v = 10)
        {
            u = _u;
            v = _v;
            _patchPoints = a;
            CalculateParametrizationVectors();
            CalculatePoints();
        }

        public Patch()
        {
            _patchPoints = new Point[4, 4];
            //for (int i = 0; i < 4; i++)
            //{
            //    _patchPoints[i] = new Point[4];
            //}
        }

        public Point[,] PatchPoints
        {
            get { return _patchPoints; }
            set { _patchPoints = value; }
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

        public void DrawPatch(Matrix4d transformacja)
        {
            GL.Begin(BeginMode.Lines);
            
            for (int i = 0; i < U.Length; i++)
            {
                for (int j = 0; j < V.Length-1; j++)
                {
                    CalculatedPoints[i, j] = MatrixProvider.Multiply(CalculateB(U[i]), _patchPoints, CalculateB(V[j]));

                    var _windowCoordinates = projection.Multiply(transformacja.Multiply(CalculatedPoints[i, j].Coordinates));
                    GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);

                    CalculatedPoints[i, j+1] = MatrixProvider.Multiply(CalculateB(U[i]), _patchPoints, CalculateB(V[j+1]));

                    _windowCoordinates = projection.Multiply(transformacja.Multiply(CalculatedPoints[i, j+1].Coordinates));
                    GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);
                }
            }

            for (int i = 0; i < U.Length-1; i++)
            {
                for (int j = 0; j < V.Length; j++)
                {
                    CalculatedPoints[i, j] = MatrixProvider.Multiply(CalculateB(U[i]), _patchPoints, CalculateB(V[j]));
                    var _windowCoordinates = projection.Multiply(transformacja.Multiply(CalculatedPoints[i, j].Coordinates));
                    GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);
                    CalculatedPoints[i+1, j] = MatrixProvider.Multiply(CalculateB(U[i+1]), _patchPoints, CalculateB(V[j]));
                    _windowCoordinates = projection.Multiply(transformacja.Multiply(CalculatedPoints[i+1, j].Coordinates));
                    GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);
                }
            }

            GL.End();
        }

        public void CalculatePoints()
        {

            CalculatedPoints = new Point[U.Length, V.Length];
            for (int i = 0; i < U.Length; i++)
            {
                for (int j = 0; j < V.Length; j++)
                {
                    CalculatedPoints[i,j]=MatrixProvider.Multiply(CalculateB(U[i]), _patchPoints, CalculateB(V[j]));
                }
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
