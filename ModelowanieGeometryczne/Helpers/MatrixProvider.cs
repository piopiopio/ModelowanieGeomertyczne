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

            Matrix4d result =  new Matrix4d(s, 0, 0, 0,
                                0, s, 0, 0,
                                0, 0, s, 0,
                                0, 0, 0, 1);
            result.Transpose();
            return result;
        }

        public static Matrix4d TranslateMatrix(double tx, double ty, double tz)
        {
            Matrix4d result =new Matrix4d(1, 0, 0, 0,
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

        //Extension method
        public static Vector4d Multiply(this Matrix4d m, Vector4d v)
        {
            return new Vector4d(v.X*m.M11 + v.Y*m.M12 + v.Z*m.M13 + v.W*m.M14,
                v.X*m.M21 + v.Y*m.M22 + v.Z*m.M23 + v.W*m.M24,
                v.X*m.M31 + v.Y*m.M32 + v.Z*m.M33 + v.W * m.M34,
              v.X*m.M41 + v.Y*m.M42 + v.Z*m.M43 + v.W * m.M44);
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