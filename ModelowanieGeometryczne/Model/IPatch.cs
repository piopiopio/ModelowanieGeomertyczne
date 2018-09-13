using System;

namespace ModelowanieGeometryczne.Model
{
    public interface IPatch
    {
        Point GetPoint(double u, double v);

        Point GetPointDerivativeU(double u, double v);

        Point GetPointDerivativeV(double u, double v);
        

    }
}