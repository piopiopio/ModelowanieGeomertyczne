using System;
using OpenTK;


namespace ModelowanieGeometryczne.Helpers
{
    public static class MatrixProvider
    {   //TODO: ScaleMatrix, TranslateMatrix, ProjectionMatrix

        public static Matrix4d ScaleMatrix(double s)
        {

            return new Matrix4d(s, 0, 0, 0,
                                0, s, 0, 0,
                                0, 0, s, 0,
                                0, 0, 0, 1);
        }

        public static Matrix4d TranslateMatrix(double tx, double ty, double tz)
        {
            return new Matrix4d(1, 0, 0, 0,
                                0, 1, 0, 0,
                                0, 0, 1, 0,
                                tx, ty, tz, 1);
        }
        //alpha in radians
        public static Matrix4d RotateXMatrix(double alphaX)
        {    
            Matrix4d result = new Matrix4d(1, 0, 0, 0,
                                0, Math.Cos(alphaX), Math.Sin(-alphaX), 0,
                                0, Math.Sin(alphaX), Math.Cos(alphaX), 0,
                                0, 0, 0, 1);
            result.Transpose(); 
            return result;
        }

        public static Matrix4d RotateYMatrix(double alphaY)
        {
            Matrix4d result = new Matrix4d(Math.Cos(alphaY), 0, Math.Sin(alphaY), 0,
                                0, 1, 0, 0,
                                -Math.Sin(alphaY), 0, Math.Cos(alphaY), 0,
                                0, 0, 0, 1);
            result.Transpose();
            return result;

        }

        public static Matrix4d RotateZMatrix(double alphaZ)
        {
            Matrix4d result = new Matrix4d(Math.Cos(alphaZ), -Math.Sin(alphaZ), 0, 0,
                                Math.Sin(alphaZ), Math.Cos(alphaZ), 0, 0,
                                0, 0, 1, 0,
                                0, 0, 0, 1);
            result.Transpose();
            return result;
        }

        public static Matrix4d ProjectionMatrix(double r)
        {
            Matrix4d result = new Matrix4d(1, 0, 0, 0,
                                           0, 1, 0, 0,
                                           0, 0, 0, 0,
                                           0, 0, 1/r, 1);
            result.Transpose();
            return result;
        }

    }
}