using System;
using OpenTK;

namespace ModelowanieGeometryczne.Helpers
{
    public static class MatrixProvider
    {

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

    }
}