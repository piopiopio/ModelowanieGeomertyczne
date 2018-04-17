using System;
using OpenTK;


namespace ModelowanieGeometryczne.Helpers
{
    public static class MatrixProvider
    {   //TODO: ScaleMatrix, TranslateMatrix, ProjectionMatrix
        private static double _r = 10.0;
        private static double _e = 0.05;



        public static Matrix4d ScaleMatrix(double s)
        {

            Matrix4d result = new Matrix4d(s, 0, 0, 0,
                                0, s, 0, 0,
                                0, 0, s, 0,
                                0, 0, 0, 1);
            result.Transpose();
            return result;
        }

        public static Matrix4d TranslateMatrix(double tx, double ty, double tz)
        {
            Matrix4d result = new Matrix4d(1, 0, 0, 0,
                                0, 1, 0, 0,
                                0, 0, 1, 0,
                                tx, ty, tz, 1);
            result.Transpose();
            return result;
        }
        //alpha in radians
        public static Matrix4d RotateXMatrix(double alphaX)
        {
            Matrix4d result = new Matrix4d(1, 0, 0, 0,
                                0, Math.Cos(alphaX), Math.Sin(-alphaX), 0,
                                0, Math.Sin(alphaX), Math.Cos(alphaX), 0,
                                0, 0, 0, 1);
            //result.Transpose();
            return result;
        }

        public static Matrix4d RotateYMatrix(double alphaY)
        {
            Matrix4d result = new Matrix4d(Math.Cos(alphaY), 0, Math.Sin(alphaY), 0,
                                0, 1, 0, 0,
                                -Math.Sin(alphaY), 0, Math.Cos(alphaY), 0,
                                0, 0, 0, 1);
            //result.Transpose();
            return result;

        }

        public static Matrix4d RotateZMatrix(double alphaZ)
        {
            Matrix4d result = new Matrix4d(Math.Cos(alphaZ), -Math.Sin(alphaZ), 0, 0,
                                Math.Sin(alphaZ), Math.Cos(alphaZ), 0, 0,
                                0, 0, 1, 0,
                                0, 0, 0, 1);
            // result.Transpose();
            return result;
        }

        public static double[,] CalculateNMatrix(this double[] knots, int n, int knotsCount)
        {

            double[,] nMatrix = new double[knotsCount + 2, knotsCount + 2];

            for (int i = 1; i <= knotsCount + 2; i++)
                for (int j = 1; j <= knotsCount + 2; j++)
                {
                    double t = knots[i + n - 1];
                    nMatrix[j - 1, i - 1] = knots.GetNFunctionValue(j, n, t);
                }

            return nMatrix;
        }

        public static double GetNFunctionValue(this double[] knots, int i, int n, double ti)
        {//TODO: Skasowac duplikacje kodu.
            if (n < 0)
                return 0;
            if (n == 0)
            {
                if (ti >= knots[i] && ti < knots[i + 1])
                    return 1;
                return 0;
            }

            double a = (knots[i + n] - knots[i] != 0) ? (ti - knots[i]) / (knots[i + n] - knots[i]) : 0;
            double b = (knots[i + n + 1] - knots[i + 1] != 0) ? (knots[i + n + 1] - ti) / (knots[i + n + 1] - knots[i + 1]) : 0;
            return (a * knots.GetNFunctionValue(i, n - 1, ti)) + (b * knots.GetNFunctionValue(i + 1, n - 1, ti));
        }

        public static double[] GaussElimination(this double[,] a, double[] b)
        {
            int n = b.Length;
            double[] x = new double[n];

            double[,] tmpA = new double[n, n + 1];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    tmpA[i, j] = a[i, j];
                }
                tmpA[i, n] = b[i];
            }

            double tmp = 0;

            for (int k = 0; k < n - 1; k++)
            {
                for (int i = k + 1; i < n; i++)
                {
                    tmp = tmpA[i, k] / tmpA[k, k];
                    for (int j = k; j < n + 1; j++)
                    {
                        tmpA[i, j] -= tmp * tmpA[k, j];
                    }
                }
            }

            for (int k = n - 1; k >= 0; k--)
            {
                tmp = 0;
                for (int j = k + 1; j < n; j++)
                {
                    tmp += tmpA[k, j] * x[j];
                }
                x[k] = (tmpA[k, n] - tmp) / tmpA[k, k];
            }

            return x;
        }
        //Extension method
        public static Vector4d Multiply(this Matrix4d m, Vector4d v)
        {
            return new Vector4d(v.X * m.M11 + v.Y * m.M12 + v.Z * m.M13 + v.W * m.M14,
                v.X * m.M21 + v.Y * m.M22 + v.Z * m.M23 + v.W * m.M24,
                v.X * m.M31 + v.Y * m.M32 + v.Z * m.M33 + v.W * m.M34,
              v.X * m.M41 + v.Y * m.M42 + v.Z * m.M43 + v.W * m.M44);
        }
        public static Vector4d Multiply(this Matrix4d m, ModelowanieGeometryczne.Model.Point v)
        {
            return new Vector4d(v.X * m.M11 + v.Y * m.M12 + v.Z * m.M13 + 1 * m.M14,
                v.X * m.M21 + v.Y * m.M22 + v.Z * m.M23 + 1 * m.M24,
                v.X * m.M31 + v.Y * m.M32 + v.Z * m.M33 + 1 * m.M34,
              v.X * m.M41 + v.Y * m.M42 + v.Z * m.M43 + 1 * m.M44);
        }


        public static Matrix4d ProjectionMatrix()
        {
            return ProjectionMatrix(_r, 0);
        }

        public static Matrix4d LeftProjectionMatrix()
        {
            return ProjectionMatrix(_r, -_e);
        }

        public static Matrix4d RightProjectionMatrix()
        {
            return ProjectionMatrix(_r, _e);
        }

        //TODO: StereoscopyProjectionMatrix
        private static Matrix4d ProjectionMatrix(double r, double e)
        {
            Matrix4d result = new Matrix4d(1, 0, e / (2 * r), 0,
                                           0, 1, 0, 0,
                                           0, 0, 0, 0,
                                           0, 0, 1 / r, 1);
            //result.Transpose();
            return result;
        }
    }
}