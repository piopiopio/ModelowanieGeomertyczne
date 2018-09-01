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
        private double alpha = 0.01; //step length
        public double[] StartPoint;
        //TODO: Tylko dla pojedynczych płatów beziera, rozwinąć również dla łączonych.
        public string Name { get; set; }
        private static int number = 0;
        public int CurvePointsNumber { get; set; }
        public List<Point[]> PointsHistoryGradientDescent = new List<Point[]>();
        public List<double> FunctionValueHistory = new List<double>();
        public List<double[]> GradientHistory = new List<double[]>();

        public TrimCurve()
        {
            number++;
            Name = "Trimming curve " + number.ToString();
            CurvePointsNumber = 10;
        }
        //public Vector4d NewtonOuputPoint;
        //public void NewtonMethod(double[] t, Point StartPointXYZ, Point[,] BezierPatch1, Point[,] BezierPatch2, Vector3d Direction)
        //{
        //    double epsilonStep = 0.2;
        //    var BP1 = GetPoint(t[0], BezierPatch1, t[1]);
        //    var BP1du = GetPointDerivativeU(t[0], BezierPatch1, t[1]);
        //    var BP1dv = GetPointDerivativeV(t[0], BezierPatch1, t[1]);

        //    var BP2 = GetPoint(t[2], BezierPatch2, t[3]);
        //    var BP2du = GetPointDerivativeU(t[2], BezierPatch2, t[3]);
        //    var BP2dv = GetPointDerivativeV(t[2], BezierPatch2, t[3]);

        //    var temp = BP1 - StartPointXYZ;

        //    var additionalEquation = temp.X * Direction.X + temp.Y * Direction.Y + temp.Z * Direction.Z - epsilonStep;
        //    Vector4d Fun = new Vector4d(temp.X, temp.Y, temp.Z, additionalEquation);

        //    Matrix4d jacobian = new Matrix4d(BP1du.X, BP1dv.X, -BP2du.X, -BP2dv.X,
        //        BP1du.Y, BP1dv.Y, -BP2du.Y, -BP2dv.Y,
        //        BP1du.Z, BP1dv.Z, -BP2du.Z, -BP2dv.Z,
        //        Vector3d.Dot(BP1du.GetPointAsVector3D(), Direction), Vector3d.Dot(BP1dv.GetPointAsVector3D(), Direction), 0, 1);
        //    jacobian.Transpose();

        //    var invertedJcobian = jacobian;
        //    invertedJcobian.Invert();

        //    Vector4d StartPointVector = new Vector4d(StartPointXYZ.X, StartPointXYZ.Y, StartPointXYZ.Z, 0);
        //    Vector4d Xk1 = StartPointVector - invertedJcobian.Multiply(Fun);
        //    NewtonOuputPoint = Xk1;
        //}
        public List<Point[]> NewtonOuputPoint = new List<Point[]>();

        public Point NewtonStartPoint;

        public Point NewtonPointToGo;
        public double[] NewtonMethod(double[] t, Point[,] BezierPatch1, Point[,] BezierPatch2, Vector3d Direction)
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

                // Vector4d StartPointVector = new Vector4d(StartPointXYZ.X, StartPointXYZ.Y, StartPointXYZ.Z, 0);

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
                    break;
                }


            }


            Point[] tempOutputPoints = new Point[2];
            tempOutputPoints[0] = GetPoint(tk_1.X, BezierPatch1, tk_1.Y);
            tempOutputPoints[1] = GetPoint(tk_1.Z, BezierPatch2, tk_1.W);
            NewtonOuputPoint.Add(tempOutputPoints);

            double[] result= new double[]{tk.X, tk.Y, tk.Z, tk.W};
            return result;
        }

        public Vector3d CalculateNewDirectionForNewtonMethod(double[] t, Point[,] BezierPatch1, Point[,] BezierPatch2, double delta = 0.001)
        {//epsilon długość skoku;
            //delta potrzebne do pochodnej mrs

            var tu_plusDelta = t[0] + delta;
            var tu_minusDelta = t[0] - delta;

            var tv_plusDelta = t[1] + delta;
            var tv_minusDelta = t[1] - delta;

            var P_u = ((GetPoint(tu_plusDelta, BezierPatch1, t[1]) - GetPoint(tu_minusDelta, BezierPatch1, t[1]))) / (2 * delta);
            var P_v = ((GetPoint(t[0], BezierPatch1, tv_plusDelta) - GetPoint(t[0], BezierPatch1, tv_minusDelta))) / (2 * delta);


            var n1 = Point.CrossProduct(P_u, P_v);
            n1.Normalize();


            tu_plusDelta = t[2] + delta;
            tu_minusDelta = t[2] - delta;

            tv_plusDelta = t[3] + delta;
            tv_minusDelta = t[3] - delta;

            var P2_u = ((GetPoint(tu_plusDelta, BezierPatch1, t[3]) - GetPoint(tu_minusDelta, BezierPatch1, t[3]))) / (2 * delta);
            var P2_v = ((GetPoint(t[2], BezierPatch1, tv_plusDelta) - GetPoint(t[2], BezierPatch1, tv_minusDelta))) / (2 * delta);


            var n2 = Point.CrossProduct(P2_u, P2_v);
            n2.Normalize();


            return Vector3d.Normalize(Vector3d.Cross(n1, n2));

            //TODO: Przetestować

        }

        public Vector3d DirectionForNewton;
        public void CalclulateTrimmedCurve(double[] t, BezierPatch B1, BezierPatch B2)
        {
            StartPoint = GradientDescentMethod(t, B1.GetAllPointsInOneArray(), B2.GetAllPointsInOneArray());
            //DirectionForNewton = CalculateNewDirectionForNewtonMethod(StartPoint, B1.GetAllPointsInOneArray(), B2.GetAllPointsInOneArray());
            //StartPoint = NewtonMethod(StartPoint, B1.GetAllPointsInOneArray(), B2.GetAllPointsInOneArray(), DirectionForNewton);

            while (true)
            {


                DirectionForNewton = CalculateNewDirectionForNewtonMethod(StartPoint, B1.GetAllPointsInOneArray(),B2.GetAllPointsInOneArray());
                StartPoint=NewtonMethod(StartPoint, B1.GetAllPointsInOneArray(), B2.GetAllPointsInOneArray(), DirectionForNewton);
                if (StartPoint == null)
                {
                    break;
                }

            }

        }

        public Point[] GetPoints(double[] t, Point[,] BezierPatch1, Point[,] BezierPatch2)
        {
            Point[] PointsArray = new Point[2];
            //TODO: Sprawdzać czy t nie jest null'em jeśli jest to wywalać z funkcji
            PointsArray[0] = MatrixProvider.Multiply(CalculateB(t[0]), BezierPatch1, CalculateB(t[1]));
            PointsArray[1] = MatrixProvider.Multiply(CalculateB(t[2]), BezierPatch2, CalculateB(t[3]));
            return PointsArray;
        }

        public Point GetPointDerivativeU(double u, Point[,] BezierPatch1, double v, double delta = 0.00001)
        {
            var tu_plusDelta = u + delta;
            var tu_minusDelta = u - delta;

            return (GetPoint(tu_plusDelta, BezierPatch1, v) - GetPoint(tu_minusDelta, BezierPatch1, v)) / (2 * delta);
        }

        public Point GetPointDerivativeV(double u, Point[,] BezierPatch1, double v, double delta = 0.00001)
        {
            var tv_plusDelta = v + delta;
            var tv_minusDelta = v - delta;

            return ((GetPoint(u, BezierPatch1, tv_plusDelta) - GetPoint(u, BezierPatch1, tv_minusDelta))) / (2 * delta);
        }

        public Point GetPoint(double u, Point[,] BezierPatch, double v)
        {
            return MatrixProvider.Multiply(CalculateB(u), BezierPatch, CalculateB(v));

        }

        public double[] CalculateB(double u)
        {
            return new double[4] { (1 - u) * (1 - u) * (1 - u), 3 * u * (1 - u) * (1 - u), 3 * u * u * (1 - u), u * u * u };
        }

        public double Bu1PointBv1_Bu2PointBv2(double u1, Point[,] BezierPatch1, double v1, double u2, Point[,] BezierPatch2, double v2)
        {//Obliczanie wartości minimalizowanej funkcji funkcji celu
            return (MatrixProvider.Multiply(CalculateB(u1), BezierPatch1, CalculateB(v1)) - MatrixProvider.Multiply(CalculateB(u2), BezierPatch2, CalculateB(v2))).Length();
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
            //t = [u0; v0; u1; v1];
            double delta = 0.0001;

            double alpha = 0.01;
            double StopCondition = 0.02;

            int stopStepsNumber = 1000;
            int stepNumber = 0;
            double[] t_temp = (double[])t.Clone();
            double[] gradient = new double[4];
            double FunctionValue;

            FunctionValue = Bu1PointBv1_Bu2PointBv2(t[0], BezierPatch1, t[1], t[2], BezierPatch2, t[3]);

            while (FunctionValue > StopCondition)
            {
                gradient[0] = ((Bu1PointBv1_Bu2PointBv2(t[0] + delta, BezierPatch1, t[1], t[2], BezierPatch2, t[3]) - Bu1PointBv1_Bu2PointBv2(t[0] - delta, BezierPatch1, t[1], t[2], BezierPatch2, t[3])) / (2 * delta));
                gradient[1] = ((Bu1PointBv1_Bu2PointBv2(t[0], BezierPatch1, t[1] + delta, t[2], BezierPatch2, t[3]) - Bu1PointBv1_Bu2PointBv2(t[0], BezierPatch1, t[1] - delta, t[2], BezierPatch2, t[3])) / (2 * delta));
                gradient[2] = ((Bu1PointBv1_Bu2PointBv2(t[0], BezierPatch1, t[1], t[2] + delta, BezierPatch2, t[3]) - Bu1PointBv1_Bu2PointBv2(t[0], BezierPatch1, t[1], t[2] - delta, BezierPatch2, t[3])) / (2 * delta));
                gradient[3] = ((Bu1PointBv1_Bu2PointBv2(t[0], BezierPatch1, t[1], t[2], BezierPatch2, t[3] + delta) - Bu1PointBv1_Bu2PointBv2(t[0], BezierPatch1, t[1], t[2], BezierPatch2, t[3] - delta)) / (2 * delta));

                for (int i = 0; i < 4; i++)
                {
                    t_temp[i] = t[i] - alpha * gradient[i];
                    if (t_temp[i] > 1) t_temp[i] = 1;
                    if (t_temp[i] < 0) t_temp[i] = 0;

                }

                t = (double[])t_temp.Clone();
                FunctionValue = Bu1PointBv1_Bu2PointBv2(t[0], BezierPatch1, t[1], t[2], BezierPatch2, t[3]);
                var Function = Bu1PointBv1_Bu2PointBv2(t[0], BezierPatch1, t[1], t[2], BezierPatch2, t[3]);

                PointsHistoryGradientDescent.Add(GetPoints(t, BezierPatch1, BezierPatch2));
                FunctionValueHistory.Add(FunctionValue);
                GradientHistory.Add(gradient);
                if (stepNumber > stopStepsNumber)
                {
                    return null;
                    //TODO: Zrobiś w finalnej wersji return t i messegebox.
                }
                stepNumber++;
            }

            return t;
        }
    }
}
