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
        public double NewtonForwardStep { get; set; }
        public double NewtonStopCondition { get; set; }
        public int NewtonStepNumberCondition { get; set; }

        public double GradientDescentethodStepLength { get; set; }
        public double GradientDescentStopCondition { get; set; }
        public int GradientDescentethodStopStepsNumber { get; set; }

        public string Name { get; set; }
        private static int _curveId = 0;
        public int CurvePointsNumber { get; set; }
        public bool Selected { get; set; }
        public double[] StartPoint;
        public List<Point> NewtonPointToGo = new List<Point>();
        public List<Point[]> NewtonOuputPoint = new List<Point[]>();

        

        //Visualization
        // {
        public List<Point[]> PointsHistoryGradientDescent = new List<Point[]>();
        public List<double> FunctionValueHistory = new List<double>();
        public List<double[]> GradientHistory = new List<double[]>();

        public List<double> NewtonJacobianDeterminant = new List<double>();
        // }

        public TrimCurve(double GradientDescentethodStepLengthIn = 0.1, double GradientDescentStopConditionIn = 0.2,
            int GradientDescentethodStopStepsNumberIn = 1000, double newtonForwardStepIn = -0.1,
            double newtonStopConditionIn = 0.01, int newtonStepNumberConditionIn = 10000)
        {
            _curveId++;
            Name = "Trimming curve " + _curveId.ToString();


            CurvePointsNumber = 10;

            NewtonForwardStep = newtonForwardStepIn;
            NewtonStopCondition = newtonStopConditionIn;
            NewtonStepNumberCondition = newtonStepNumberConditionIn;

            GradientDescentethodStepLength = GradientDescentethodStepLengthIn;
            GradientDescentStopCondition = GradientDescentStopConditionIn;
            GradientDescentethodStopStepsNumber = GradientDescentethodStopStepsNumberIn;
        }


        private double debugtemporaryvariable = 0.2;
        private double debugWyznacznik;

        public double[] NewtonMethod<T>(double[] t, T BezierPatch1, T BezierPatch2, Vector3d Direction, bool addToEnd)
            where T : IPatch
        {
            double StopValue;
            int stepNumber = 0;



            var BP1 = BezierPatch1.GetPoint(t[0], t[1]);
            var StartPointXYZ = BP1;
            var BP1du = BezierPatch1.GetPointDerivativeU(t[0], t[1]);
            var BP1dv = BezierPatch1.GetPointDerivativeV(t[0], t[1]);
            var BP2 = BezierPatch2.GetPoint(t[2], t[3]);
            var BP2du = BezierPatch2.GetPointDerivativeU(t[2], t[3]);
            var BP2dv = BezierPatch2.GetPointDerivativeV(t[2], t[3]);


            var temp = BP1 - BP2;
            var temp2 = BP1 - StartPointXYZ;
            var additionalEquation = temp2.X * Direction.X + temp2.Y * Direction.Y + temp2.Z * Direction.Z -
                                     NewtonForwardStep;

            Vector4d Fun = new Vector4d(temp.X, temp.Y, temp.Z, additionalEquation);
            Vector4d tk = new Vector4d(t[0], t[1], t[2], t[3]);
            Vector4d tk_1 = new Vector4d();

            StopValue = Fun.Length;

            while (StopValue > NewtonStopCondition)
            {
                //Matrix4d jacobian = new Matrix4d(BP1du.X, BP1dv.X, -BP2du.X, -BP2dv.X,
                //    BP1du.Y, BP1dv.Y, -BP2du.Y, -BP2dv.Y,
                //    BP1du.Z, BP1dv.Z, -BP2du.Z, -BP2dv.Z,
                //    Vector3d.Dot(BP1du.GetPointAsVector3D(), Direction),
                //    Vector3d.Dot(BP1dv.GetPointAsVector3D(), Direction), 0, 1);
                Matrix4d jacobian = new Matrix4d(BP1du.X, BP1dv.X, -BP2du.X, -BP2dv.X,
                    BP1du.Y, BP1dv.Y, -BP2du.Y, -BP2dv.Y,
                    BP1du.Z, BP1dv.Z, -BP2du.Z, -BP2dv.Z,
                    Vector3d.Dot(BP1du.GetPointAsVector3D(), Direction),
                    Vector3d.Dot(BP1dv.GetPointAsVector3D(), Direction), 0, 0);


                var invertedJcobian = jacobian;
                NewtonJacobianDeterminant.Add(jacobian.Determinant);
                try
                {
                    invertedJcobian.Invert();
                }
                catch
                {
                    return null;
                }

                var debugVar = invertedJcobian.Multiply(Fun);
                //0.01->0.1
                tk_1 = tk - 0.1 * invertedJcobian.Multiply(Fun);

                tk_1 = tk - debugtemporaryvariable * invertedJcobian.Multiply(Fun);


                //if (tk_1.X > 1)
                //{
                //    tk_1.X -= 1;

                //    // tk_1.X = 1;
                //    //  return null;
                //}

                //if (tk_1.X < 0)
                //{
                //    tk_1.X += 1;
                //    //tk_1.X = 0;
                //    //return null;
                //}
                tk_1.X -= Math.Floor(tk_1.X);

                //if (tk_1.Y > 1)
                //{
                //    tk_1.Y -= 1;
                //    //tk_1.Y = 1;
                //    //return null;
                //}

                //if (tk_1.Y < 0)
                //{
                //    tk_1.Y += 1;
                //    //return null;
                //}

                tk_1.Y -= Math.Floor(tk_1.Y);
                //if (tk_1.Z > 1)
                //{
                //    tk_1.Z -= 1;
                //    //return null;
                //}

                //if (tk_1.Z < 0)
                //{
                //    tk_1.Z += 1;
                //    //return null;
                //}
                tk_1.Z -= Math.Floor(tk_1.Z);

                //if (tk_1.W > 1)
                //{
                //    tk_1.W -= 1;
                //    //return null;
                //}

                //if (tk_1.W < 0)
                //{
                //    tk_1.W += 1;
                //    // return null;
                //}
                tk_1.W -= Math.Floor(tk_1.W);

                tk = tk_1;

                BP1 = BezierPatch1.GetPoint(tk.X, tk.Y);
                BP1du = BezierPatch1.GetPointDerivativeU(tk.X, tk.Y);
                BP1dv = BezierPatch1.GetPointDerivativeV(tk.X, tk.Y);
                BP2 = BezierPatch2.GetPoint(tk.Z, tk.W);
                BP2du = BezierPatch2.GetPointDerivativeU(tk.Z, tk.W);
                BP2dv = BezierPatch2.GetPointDerivativeV(tk.Z, tk.W);

                //Visualization
                NewtonPointToGo.Add(BP1 + NewtonForwardStep * Direction);

                temp = BP1 - BP2;
                temp2 = BP1 - StartPointXYZ;
                additionalEquation = temp2.X * Direction.X + temp2.Y * Direction.Y + temp2.Z * Direction.Z -
                                     NewtonForwardStep;

                Fun = new Vector4d(temp.X, temp.Y, temp.Z, additionalEquation);
                StopValue = Fun.Length;

                stepNumber++;
                if (stepNumber > NewtonStepNumberCondition)
                {
                    //MessageBox.Show("Metoda Newtona nie znalazła rozwiązania spełniającego kryteria. ");
                    return null;
                }


            }


            Point[] tempOutputPoints = new Point[2];


            tempOutputPoints[0] = BezierPatch1.GetPoint(tk_1.X, tk_1.Y);
            tempOutputPoints[1] = BezierPatch2.GetPoint(tk_1.Z, tk_1.W);

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

        public Vector3d CalculateNewDirectionForNewtonMethod<T>(double[] t, T BezierPatch1, T BezierPatch2)
            where T : IPatch
        {
            var P1_u = BezierPatch1.GetPointDerivativeU(t[0], t[1]);
            var P1_v = BezierPatch1.GetPointDerivativeV(t[0], t[1]);
            var n1 = Point.CrossProduct(P1_u, P1_v);
            n1.Normalize();

            var P2_u = BezierPatch2.GetPointDerivativeU(t[2], t[3]);
            var P2_v = BezierPatch2.GetPointDerivativeV(t[2], t[3]);
            var n2 = Point.CrossProduct(P2_u, P2_v);
            n2.Normalize();

            return Vector3d.Normalize(Vector3d.Cross(n1, n2));
        }


        public List<double[]> StartPointHistory = new List<double[]>();

        bool checkIfNear(double u1, double v1, double u2, double v2, double epsilon)
        {
            Vector2d w1 = new Vector2d(u1, v1);
            Vector2d w2 = new Vector2d(u2, v2);

            return (w2 - w1).Length <= epsilon;
        }


        double checkIfNearLengthValue(double u1, double v1, double u2, double v2, double epsilon)
        {
            Vector2d w1 = new Vector2d(u1, v1);
            Vector2d w2 = new Vector2d(u2, u2);

            return (w2 - w1).Length;
        }

        public double[] StartPointForGradientDescentMethod = new double[4];

        public bool CalclulateTrimmedCurve<T>(Point cursor, T B1, T B2) where T : IPatch
        {

            StartPointForGradientDescentMethod = SearchStartingPointsForGradientDescentMethod(cursor, B1, B2);
            Vector3d directionForNewton;
            NewtonOuputPoint.Clear();
            PointsHistoryGradientDescent.Clear();
            StartPoint = GradientDescentMethod(StartPointForGradientDescentMethod, B1, B2);
            //var startPointForNegativeDirectionNewtonMethod = StartPoint;
            int iteriationCounter = 0;
            string log1 = "  Gradient descent method failed;";
            string log2 = "  Newton method failed;";
            bool gradientSuccess = false;
            bool newtonSuccess = false;
            //Point[] temp = new Point[2];
            //temp[0] = B1.GetPoint(StartPoint[0], StartPoint[1]);
            //temp[1] = B2.GetPoint(StartPoint[2], StartPoint[3]);
            //NewtonOuputPoint.Add(temp);

            //proba wyznaczenia dokladnie brakujacego punktu
            //var tempNewtonForwardStep = NewtonForwardStep;
            //NewtonForwardStep = 0;
            //directionForNewton = new Vector3d(0, 0, 0);
            //NewtonMethod(StartPoint, B1, B2, directionForNewton, addToEnd: true);
            //double[] StartPointForComparison = NewtonMethod(StartPoint, B1, B2, directionForNewton, addToEnd: true);
            //StartPoint = StartPointForComparison;
            //NewtonForwardStep = tempNewtonForwardStep;
            double[] StartPointForComparison = null;

            //double debugVariable = 10;
            bool nearPointFoundFlag = false;
            //StartPointHistory.Add(StartPoint);
            if (StartPoint != null)
            {
                log1 ="  Gradient descent method ok;";
                gradientSuccess = true;
            }

            while (StartPoint != null)
            {

                directionForNewton = CalculateNewDirectionForNewtonMethod(StartPoint, B1, B2);
                StartPoint = NewtonMethod(StartPoint, B1, B2, directionForNewton, addToEnd: true);
                iteriationCounter++;

                if (StartPoint != null)
                {
                    StartPointHistory.Add(StartPoint);
                    

                }

                if (StartPointForComparison == null && StartPoint != null)
                {
                    StartPointForComparison = (double[])StartPoint.Clone();
                }

                //if (debugVariable > checkIfNearLengthValue(StartPointForComparison[0], StartPointForComparison[1], StartPoint[0], StartPoint[1], 0.3))
                //{
                //    debugVariable = checkIfNearLengthValue(StartPointForComparison[0], StartPointForComparison[1], StartPoint[0], StartPoint[1], 0.3);
                //}

                if (iteriationCounter > 10 && StartPoint != null && StartPointForComparison != null)
                {
                    if (checkIfNear(StartPointForComparison[0], StartPointForComparison[1], StartPoint[0],
                        StartPoint[1], 0.005))
                    {
                        nearPointFoundFlag = true;

                        break;
                    }
                }



                //else
                //{
                //    if (checkIfNear(StartPointForComparison[0], StartPointForComparison[1], StartPoint[0],
                //        StartPoint[1], NewtonForwardStep / 2))
                //    {
                //        break;
                //    }
                //}


            }



            StartPoint = StartPointForComparison;
            //StartPoint = startPointForNegativeDirectionNewtonMethod;
            iteriationCounter = 0;
            while (StartPoint != null)
            {



                directionForNewton = (-1) * CalculateNewDirectionForNewtonMethod(StartPoint, B1, B2);
                StartPoint = NewtonMethod(StartPoint, B1, B2, directionForNewton, addToEnd: false);
                if (StartPoint != null)
                {
                    StartPointHistory.Add(StartPoint);
                   
                }

                if (nearPointFoundFlag) break;
                //if (checkIfNear(startPointForNegativeDirectionNewtonMethod[0], startPointForNegativeDirectionNewtonMethod[1], StartPoint[0], StartPoint[1], NewtonForwardStep / 2))
                //{
                //    break;
                //}

                ////iteriationCounter++;
                ////if (iteriationCounter > 10 && StartPoint != null && StartPointForComparison != null)
                ////{
                ////    if (checkIfNear(StartPointForComparison[0], StartPointForComparison[1], StartPoint[0],
                ////        StartPoint[1], 0.005))
                ////    {
                ////        break;
                ////    }
                ////}
            }

            //Nie kasować, fragment kodu do sprawdzania zwracanych punktow
            //var debugDelta = 1.0 / 6.0;
            //double debugTemp = 0.0;
            //List<Point> debugList = new List<Point>();
            //for (int i = 0; i < 7; i++)
            //{
            //    debugList.Add(B1.GetPoint(debugTemp, 0.0));
            //    debugTemp += debugDelta;
            //}

            if (StartPointHistory.Count > 0)
            {
                log2 = " Newton method ok;";
                newtonSuccess = true;
            }

            MessageBox.Show("Trim curve status:"+log1+log2);

            return newtonSuccess && gradientSuccess;
        }

        //public Point[] GetPoints(double[] t, BezierPatch BezierPatch1, BezierPatch BezierPatch2)
        //{
        //    Point[] pointsArray = new Point[2];
        //    //TODO: Sprawdzać czy t nie jest null'em jeśli jest to wywalać z funkcji
        //    pointsArray[0] = BezierPatch1.GetPoint(t[0], t[1]); // MatrixProvider.Multiply(CalculateB(t[0]), BezierPatch1, CalculateB(t[1]));
        //    pointsArray[1] = BezierPatch2.GetPoint(t[2], t[3]); //MatrixProvider.Multiply(CalculateB(t[2]), BezierPatch2, CalculateB(t[3])));
        //    return pointsArray;
        //}

        //public double[] CalculateB(double u)
        //{
        //    return new double[4] { (1 - u) * (1 - u) * (1 - u), 3 * u * (1 - u) * (1 - u), 3 * u * u * (1 - u), u * u * u };
        //}

        //public double[] CalculateDerrivativeB(double u)
        //{
        //    return new double[4] { -3 * (u - 1) * (u - 1), 3 * u * (2 * u - 2) + 3 * (u - 1) * (u - 1), -6 * u * (u - 1) - 3 * u * u, 3 * u * u };
        //}

        //public Point GetPoint(double u, Point[,] BezierPatch, double v)
        //{
        //    return MatrixProvider.Multiply(CalculateB(u), BezierPatch, CalculateB(v));

        //}

        //public Point GetPointDerivativeU(double u, Point[,] BezierPatch1, double v)
        //{
        //    return MatrixProvider.Multiply(CalculateDerrivativeB(u), BezierPatch1, CalculateB(v));
        //}

        //public Point GetPointDerivativeV(double u, Point[,] BezierPatch1, double v)
        //{
        //    return MatrixProvider.Multiply(CalculateB(u), BezierPatch1, CalculateDerrivativeB(v));
        //}

        public double FunctionToMinimalize<T>(double u1, T BezierPatch1, double v1, double u2, T BezierPatch2, double v2) where T : IPatch
        {
            return (BezierPatch1.GetPoint(u1, v1) - BezierPatch2.GetPoint(u2, v2)).Length();
        }


        public double[] GetGradient<T>(double[] t, T BezierPatch1, T BezierPatch2) where T : IPatch
        {
            double[] gradient = new double[4];
            double delta = 0.00001;

            gradient[0] = ((FunctionToMinimalize(t[0] + delta, BezierPatch1, t[1], t[2], BezierPatch2, t[3]) - FunctionToMinimalize(t[0] - delta, BezierPatch1, t[1], t[2], BezierPatch2, t[3])) / (2 * delta));
            gradient[1] = ((FunctionToMinimalize(t[0], BezierPatch1, t[1] + delta, t[2], BezierPatch2, t[3]) - FunctionToMinimalize(t[0], BezierPatch1, t[1] - delta, t[2], BezierPatch2, t[3])) / (2 * delta));
            gradient[2] = ((FunctionToMinimalize(t[0], BezierPatch1, t[1], t[2] + delta, BezierPatch2, t[3]) - FunctionToMinimalize(t[0], BezierPatch1, t[1], t[2] - delta, BezierPatch2, t[3])) / (2 * delta));
            gradient[3] = ((FunctionToMinimalize(t[0], BezierPatch1, t[1], t[2], BezierPatch2, t[3] + delta) - FunctionToMinimalize(t[0], BezierPatch1, t[1], t[2], BezierPatch2, t[3] - delta)) / (2 * delta));

            return gradient;
        }

        public double[] SearchStartingPointsForGradientDescentMethod<T>(Point cursor, T BezierPatch1, T BezierPatch2) where T : IPatch
        {

            double deltaForSelfIntersectionDetection = 0;

            if ((object)BezierPatch1 == (object)BezierPatch2)
            {
                deltaForSelfIntersectionDetection = 0.2;
            }
            double[] result = new double[4];
            int gridMesh = 20;
            double d;
            d = 1.0 / gridMesh;
            double[] length = new double[2] { Double.PositiveInfinity, Double.PositiveInfinity };
            double[] t = new double[4];
            Point[] temp = new Point[2];
            Point[] debugVariable = new Point[2];
            for (int l = 0; l < gridMesh + 1; l++)
            {
                for (int k = 0; k < gridMesh + 1; k++)
                {

                    t[0] = l * d;
                    t[1] = k * d;

                    temp[0] = BezierPatch1.GetPoint(t[0], t[1]);


                    if (length[0] > (Math.Abs((cursor.AddToAllCoordinates(deltaForSelfIntersectionDetection) - temp[0]).Length())))
                    {
                        length[0] = Math.Abs((cursor.AddToAllCoordinates(deltaForSelfIntersectionDetection) - temp[0]).Length());
                        result[0] = t[0];
                        result[1] = t[1];
                        debugVariable[0] = temp[0];
                    }

                    t[2] = l * d;
                    t[3] = k * d;
                    temp[1] = BezierPatch2.GetPoint(t[2], t[3]);
                    if (length[1] > (Math.Abs((cursor.AddToAllCoordinates(-deltaForSelfIntersectionDetection) - temp[1]).Length())))
                    {
                        length[1] = Math.Abs((cursor.AddToAllCoordinates(-deltaForSelfIntersectionDetection) - temp[1]).Length());
                        result[2] = t[2];
                        result[3] = t[3];
                        debugVariable[1] = temp[1];
                    }


                }
            }





            return result;

        }



        public double[] GradientDescentMethod<T>(double[] t, T BezierPatch1, T BezierPatch2) where T : IPatch
        {


            // double gradientDescentethodStepLength = 0.001;

            //double StopCondition = 0.02;

            int stepNumber = 0;
            double[] t_temp = (double[])t.Clone();
            double[] gradient = new double[4];
            double FunctionValue;


            FunctionValue = (BezierPatch1.GetPoint(t[0], t[1]) - BezierPatch2.GetPoint(t[2], t[3])).Length();


            while (FunctionValue > GradientDescentStopCondition)
            {

                gradient = GetGradient(t, BezierPatch1, BezierPatch2);


                for (int i = 0; i < 4; i++)
                {
                    t_temp[i] = t[i] - GradientDescentethodStepLength * gradient[i];
                    if (t_temp[i] > 1) t_temp[i] = 1;
                    if (t_temp[i] < 0) t_temp[i] = 0;

                }

                t = (double[])t_temp.Clone();
                FunctionValue = FunctionToMinimalize(t[0], BezierPatch1, t[1], t[2], BezierPatch2, t[3]);

                Point[] pointsArray = new Point[2];
                pointsArray[0] = BezierPatch1.GetPoint(t[0], t[1]); // MatrixProvider.Multiply(CalculateB(t[0]), BezierPatch1, CalculateB(t[1]));
                pointsArray[1] = BezierPatch2.GetPoint(t[2], t[3]); //MatrixProvider.Multiply(CalculateB(t[2]), BezierPatch2, CalculateB(t[3])));
                PointsHistoryGradientDescent.Add(pointsArray);
                FunctionValueHistory.Add(FunctionValue);
                GradientHistory.Add(gradient);


                if (t[0] > 1 || t[1] > 1 || t[2] > 1 || t[3] > 1 || t[0] < 0 || t[1] < 0 || t[2] < 0 || t[3] < 0)
                {

                }

                if (stepNumber > GradientDescentethodStopStepsNumber)
                {
                   // MessageBox.Show("Metoda gradientu prostego nie znalazła rozwiązania spełniającego kryteria. ");
                    return null;

                }
                stepNumber++;


            }

            return t;
        }
    }
}
