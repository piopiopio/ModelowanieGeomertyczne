using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelowanieGeometryczne.Model
{
    class GregoryPatch
    {
        GregoryPatch(BezierPatch P0, BezierPatch P1, BezierPatch P2)
        {

        }

        public static Point MergePoints( Point p0, Point p1)
        {
            return new Point((p0.X + p1.X) / 2, (p0.Y + p1.Y) / 2, (p0.Z + p1.Z) / 2);
        }

        //public static void MergePoints(List<Point> p)
        //{
        //    Point temp = new Point((p[0].X + p[1].X) / 2, (p[0].Y + p[1].Y) / 2, (p[0].Z + p[1].Z) / 2);
        //    // p[0].X = (p[0].X + p[1].X) / 2;
        //    //  p[0].Y = (p[0].Y + p[1].Y) / 2;
        //    // p[0].Z = (p[0].Z + p[1].Z) / 2;
        //    p[0] = temp;
        //    p[1] = temp;
        //    //p[1] = p[0];
        //}
    }
}
