using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using ModelowanieGeometryczne.Helpers;
using OpenTK;
using System.Windows.Media.Media3D;

namespace ModelowanieGeometryczne.Model
{
    public class TrimCurve
    {

        //TODO: Tylko dla pojedynczych płatów beziera, rozwinąć również dla łączonych.
        public string Name { get; set; }
        private static int _curveId = 0;
        public int CurvePointsNumber { get; set; }
        public bool Selected { get; set; }
        public double[] StartPoint;
        public Point NewtonPointToGo;
        public List<Point[]> NewtonOuputPoint = new List<Point[]>();



        //Visualization
        // {
        public List<Point[]> PointsHistoryGradientDescent = new List<Point[]>();
        public List<double> FunctionValueHistory = new List<double>();
        public List<double[]> GradientHistory = new List<double[]>();
        // }

        public TrimCurve()
        {
            _curveId++;
            Name = "Trimming curve " + _curveId.ToString();
            CurvePointsNumber = 10;
        }




        public double[] NewtonMethod(double[] t, Point[,] BezierPatch1, Point[,] BezierPatch2, Vector3d Direction, bool addToEnd)
        {

            double epsilonStep = -0.1;
            double StopCondition = 0.01;
            double StopValue;
            int stepNumberCondition = 10000;
            int stepNumber = 0;



            var BP1 = GetPoint(t[0], BezierPatch1, t[1]);
            var StartPointXYZ = BP1;
            var BP1du = GetPointDerivativeU(t[0], BezierPatch1, t[1]);
            var BP1dv = GetPointDerivativeV(t[0], BezierPatch1, t[1]);
            var BP2 = GetPoint(t[2], BezierPatch2, t[3]);
            var BP2du = GetPointDerivativeU(t[2], BezierPatch2, t[3]);
            var BP2dv = GetPointDerivativeV(t[2], BezierPatch2, t[3]);


            var temp = BP1 - BP2;
            var temp2 = BP1 - StartPointXYZ;
            var additionalEquation = temp2.X * Direction.X + temp2.Y * Direction.Y + temp2.Z * Direction.Z - epsilonStep;

            Vector4d Fun = new Vector4d(temp.X, temp.Y, temp.Z, additionalEquation);
            Vector4d tk = new Vector4d(t[0], t[1], t[2], t[3]);
            Vector4d tk_1 = new Vector4d();

            StopValue = Fun.Length;

            while (StopValue > StopCondition)
            {
                Matrix4d jacobian = new Matrix4d(BP1du.X, BP1dv.X, -BP2du.X, -BP2dv.X,
                    BP1du.Y, BP1dv.Y, -BP2du.Y, -BP2dv.Y,
                    BP1du.Z, BP1dv.Z, -BP2du.Z, -BP2dv.Z,
                    Vector3d.Dot(BP1du.GetPointAsVector3D(), Direction),
                    Vector3d.Dot(BP1dv.GetPointAsVector3D(), Direction), 0, 1);


                var invertedJcobian = jacobian;
                invertedJcobian.Invert();

                var debugVar = invertedJcobian.Multiply(Fun);
                tk_1 = tk - 0.01 * invertedJcobian.Multiply(Fun);

                if (tk_1.X > 1)
                {
                    tk_1.X = 1;
                    return null;
                }

                if (tk_1.X < 0)
                {
                    tk_1.X = 0;
                    return null;
                }

                if (tk_1.Y > 1)
                {
                    tk_1.Y = 1;
                    return null;
                }

                if (tk_1.Y < 0)
                {
                    tk_1.Y = 0;
                    return null;
                }

                if (tk_1.Z > 1)
                {
                    tk_1.Z = 1;
                    return null;
                }

                if (tk_1.Z < 0)
                {
                    tk_1.Z = 0;
                    return null;
                }


                if (tk_1.W > 1)
                {
                    tk_1.W = 1;
                    return null;
                }

                if (tk_1.W < 0)
                {
                    tk_1.W = 0;
                    return null;
                }


                tk = tk_1;

                BP1 = GetPoint(tk.X, BezierPatch1, tk.Y);
                BP1du = GetPointDerivativeU(tk.X, BezierPatch1, tk.Y);
                BP1dv = GetPointDerivativeV(tk.X, BezierPatch1, tk.Y);
                BP2 = GetPoint(tk.Z, BezierPatch2, tk.W);
                BP2du = GetPointDerivativeU(tk.Z, BezierPatch2, tk.W);
                BP2dv = GetPointDerivativeV(tk.Z, BezierPatch2, tk.W);

                //Visualization
                NewtonPointToGo = BP1 + epsilonStep * Direction;

                temp = BP1 - BP2;
                temp2 = BP1 - StartPointXYZ;
                additionalEquation = temp2.X * Direction.X + temp2.Y * Direction.Y + temp2.Z * Direction.Z -
                                     epsilonStep;

                Fun = new Vector4d(temp.X, temp.Y, temp.Z, additionalEquation);
                StopValue = Fun.Length;

                stepNumber++;
                if (stepNumber > stepNumberCondition)
                {
                    MessageBox.Show("Metoda Newtona nie znalazła rozwiązania spełniającego kryteria. ");
                    break;
                }


            }


            Point[] tempOutputPoints = new Point[2];


            tempOutputPoints[0] = GetPoint(tk_1.X, BezierPatch1, tk_1.Y);
            tempOutputPoints[1] = GetPoint(tk_1.Z, BezierPatch2, tk_1.W);

            if (addToEnd)
            {
                NewtonOuputPoint.Add(tempOutputPoints);
            }
            else
            {
                NewtonOuputPoint.Insert(0, tempOutputPoints);
            }

            double[] result = new double[] { tk.X, tk.Y, tk.Z, tk.W };
            return result;
        }

        public Vector3d CalculateNewDirectionForNewtonMethod(double[] t, Point[,] BezierPatch1, Point[,] BezierPatch2)
        {
            var P1_u = GetPointDerivativeU(t[0], BezierPatch1, t[1]);
            var P1_v = GetPointDerivativeV(t[0], BezierPatch1, t[1]);
            var n1 = Point.CrossProduct(P1_u, P1_v);
            n1.Normalize();

            var P2_u = GetPointDerivativeU(t[2], BezierPatch2, t[3]);
            var P2_v = GetPointDerivativeV(t[2], BezierPatch2, t[3]);
            var n2 = Point.CrossProduct(P2_u, P2_v);
            n2.Normalize();

            return Vector3d.Normalize(Vector3d.Cross(n1, n2));
        }



        public void CalclulateTrimmedCurve(double[] t, BezierPatch B1, BezierPatch B2)
        {
            Vector3d directionForNewton;
            NewtonOuputPoint.Clear();
            PointsHistoryGradientDescent.Clear();
            StartPoint = GradientDescentMethod(t, B1.GetAllPointsInOneArray(), B2.GetAllPointsInOneArray());
            var startPointForNegativeDirectionNewtonMethod = StartPoint;


            while (true)
            {


                directionForNewton = CalculateNewDirectionForNewtonMethod(StartPoint, B1.GetAllPointsInOneArray(), B2.GetAllPointsInOneArray());
                StartPoint = NewtonMethod(StartPoint, B1.GetAllPointsInOneArray(), B2.GetAllPointsInOneArray(), directionForNewton, addToEnd: true);
                if (StartPoint == null)
                {
                    break;
                }

            }



            StartPoint = startPointForNegativeDirectionNewtonMethod;

            while (true)
            {


                directionForNewton = (-1) * CalculateNewDirectionForNewtonMethod(StartPoint, B1.GetAllPointsInOneArray(), B2.GetAllPointsInOneArray());
                StartPoint = NewtonMethod(StartPoint, B1.GetAllPointsInOneArray(), B2.GetAllPointsInOneArray(), directionForNewton, addToEnd: false);
                if (StartPoint == null)
                {
                    break;
                }

            }

        }

        public Point[] GetPoints(double[] t, Point[,] BezierPatch1, Point[,] BezierPatch2)
        {
            Point[] pointsArray = new Point[2];
            //TODO: Sprawdzać czy t nie jest null'em jeśli jest to wywalać z funkcji
            pointsArray[0] = MatrixProvider.Multiply(CalculateB(t[0]), BezierPatch1, CalculateB(t[1]));
            pointsArray[1] = MatrixProvider.Multiply(CalculateB(t[2]), BezierPatch2, CalculateB(t[3]));
            return pointsArray;
        }

        public double[] CalculateB(double u)
        {
            return new double[4] { (1 - u) * (1 - u) * (1 - u), 3 * u * (1 - u) * (1 - u), 3 * u * u * (1 - u), u * u * u };
        }

        public double[] CalculateDerrivativeB(double u)
        {
            return new double[4] { -3 * (u - 1) * (u - 1), 3 * u * (2 * u - 2) + 3 * (u - 1) * (u - 1), -6 * u * (u - 1) - 3 * u * u, 3 * u * u };
        }

        public Point GetPoint(double u, Point[,] BezierPatch, double v)
        {
            return MatrixProvider.Multiply(CalculateB(u), BezierPatch, CalculateB(v));

        }

        public Point GetPointDerivativeU(double u, Point[,] BezierPatch1, double v)
        {
            return MatrixProvider.Multiply(CalculateDerrivativeB(u), BezierPatch1, CalculateB(v));
        }

        public Point GetPointDerivativeV(double u, Point[,] BezierPatch1, double v)
        {
            return MatrixProvider.Multiply(CalculateB(u), BezierPatch1, CalculateDerrivativeB(v));
        }

        public double FunctionToMinimalize(double u1, Point[,] BezierPatch1, double v1, double u2, Point[,] BezierPatch2, double v2)
        {
            return (GetPoint(u1, BezierPatch1, v1) - GetPoint(u2, BezierPatch2, v2)).Length();
        }


        public double[] GetGradient(double[] t, Point[,] BezierPatch1, Point[,] BezierPatch2)
        {
            double[] gradient = new double[4];
            double delta = 0.00001;

            gradient[0] = ((FunctionToMinimalize(t[0] + delta, BezierPatch1, t[1], t[2], BezierPatch2, t[3]) - FunctionToMinimalize(t[0] - delta, BezierPatch1, t[1], t[2], BezierPatch2, t[3])) / (2 * delta));
            gradient[1] = ((FunctionToMinimalize(t[0], BezierPatch1, t[1] + delta, t[2], BezierPatch2, t[3]) - FunctionToMinimalize(t[0], BezierPatch1, t[1] - delta, t[2], BezierPatch2, t[3])) / (2 * delta));
            gradient[2] = ((FunctionToMinimalize(t[0], BezierPatch1, t[1], t[2] + delta, BezierPatch2, t[3]) - FunctionToMinimalize(t[0], BezierPatch1, t[1], t[2] - delta, BezierPatch2, t[3])) / (2 * delta));
            gradient[3] = ((FunctionToMinimalize(t[0], BezierPatch1, t[1], t[2], BezierPatch2, t[3] + delta) - FunctionToMinimalize(t[0], BezierPatch1, t[1], t[2], BezierPatch2, t[3] - delta)) / (2 * delta));

            return gradient;
        }

        public double[] SearchStartingPointsForGradientDescentMethod(Point cursor, Point[,] BezierPatch1, Point[,] BezierPatch2)
        {
            double[] result = new double[4];
            int gridMesh = 20;
            double d;
            d = 1.0 / gridMesh;
            double[] length = new double[2] { Double.PositiveInfinity, Double.PositiveInfinity };
            double[] t = new double[4];
            Point[] temp = new Point[2];
            for (int i = 0; i < gridMesh + 1; i++)
            {
                for (int j = 0; j < gridMesh + 1; j++)
                {
                    t[0] = i * d;
                    t[1] = j * d;
                    t[2] = i * d;
                    t[3] = j * d;
                    temp = GetPoints(t, BezierPatch1, BezierPatch2);

                    if (length[0] > (Math.Abs((cursor - temp[0]).Length())))
                    {
                        length[0] = Math.Abs((cursor - temp[0]).Length());
                        result[0] = t[0];
                        result[1] = t[1];
                    }
                    if (length[1] > (Math.Abs((cursor - temp[1]).Length())))
                    {
                        length[1] = Math.Abs((cursor - temp[1]).Length());
                        result[2] = t[2];
                        result[3] = t[3];
                    }

                }
            }

            return result;

        }



        public double[] GradientDescentMethod(double[] t, Point[,] BezierPatch1, Point[,] BezierPatch2)
        {
            double gradientDescentethodStepLength = 0.01;
            double StopCondition = 0.02;
            int gradientDescentethodStopStepsNumber = 1000;
            int stepNumber = 0;
            double[] t_temp = (double[])t.Clone();
            double[] gradient = new double[4];
            double FunctionValue;


            FunctionValue = (GetPoint(t[0], BezierPatch1, t[1]) - GetPoint(t[2], BezierPatch2, t[3])).Length();


            while (FunctionValue > StopCondition)
            {

                gradient = GetGradient(t, BezierPatch1, BezierPatch2);


                for (int i = 0; i < 4; i++)
                {
                    t_temp[i] = t[i] - gradientDescentethodStepLength * gradient[i];
                    if (t_temp[i] > 1) t_temp[i] = 1;
                    if (t_temp[i] < 0) t_temp[i] = 0;

                }

                t = (double[])t_temp.Clone();
                FunctionValue = FunctionToMinimalize(t[0], BezierPatch1, t[1], t[2], BezierPatch2, t[3]);


                PointsHistoryGradientDescent.Add(GetPoints(t, BezierPatch1, BezierPatch2));
                FunctionValueHistory.Add(FunctionValue);
                GradientHistory.Add(gradient);

                if (stepNumber > gradientDescentethodStopStepsNumber)
                {
                    MessageBox.Show("Metoda gradientu prostego nie znalazła rozwiązania spełniającego kryteria. ");
                    return t;

                }
                stepNumber++;
            }

            return t;
        }
    }
}
