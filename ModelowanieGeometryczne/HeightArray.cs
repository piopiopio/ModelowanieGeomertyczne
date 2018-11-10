using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelowanieGeometryczne.Model;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ModelowanieGeometryczne
{
    class HeightArray
    {

        private const double dimension = 15;
        private const int arraySize = 20;
        public Point[,] PointsArray = new Point[arraySize, arraySize];
        private const double factor = dimension / arraySize;
        public double delta = dimension / arraySize;

        public void Clear()
        {
            for (int i = 0; i < arraySize; i++)
            {
                for (int j = 0; j < arraySize; j++)
                {
                    PointsArray[i, j] = new Point(0, 0, 0);
                }
            }
        }

        public HeightArray()
        {
            Clear();
        }

        public HeightArray(ObservableCollection<BezierPatchC2> BezierPatchC2Collection, ObservableCollection<BezierPatch> BezierPatchCollection, bool increaseAccuracy=true)
        {
            Clear();
            foreach (var item in BezierPatchC2Collection)
            {
                if (increaseAccuracy)
                {
                    item.u = 10;
                    item.v = 10;
                }

                ConsiderList(item.GeneratePointsForMilling());

                if (increaseAccuracy)
                {
                    item.u = 4;
                    item.v = 4;
                }
            }

            foreach (var item in BezierPatchCollection)
            {
                if (increaseAccuracy)
                {
                    item.PatchHorizontalDivision = 10;
                    item.PatchVerticalDivision = 10;
                }

                ConsiderList(item.GeneratePointsForMilling());

                if (increaseAccuracy)
                {
                    item.PatchHorizontalDivision = 4;
                    item.PatchVerticalDivision = 4;
                }
            }
        }
        public void ConsiderList(List<Point> L)
        {
            foreach (var item1 in L)
            {
                ConsiderPoint(item1);
            }
        }
        public void ConsiderPoint(Point p)
        {
            int Xcoord = Math.Min((int)Math.Floor((p.X + dimension / 2) / factor), arraySize - 1);
            int Ycoord = Math.Min((int)Math.Floor((p.Y + dimension / 2) / factor), arraySize - 1);


           if (PointsArray[Xcoord, Ycoord].Z <= p.Z)
            {
                PointsArray[Xcoord, Ycoord] = p;
            }
        }

        public double Z_max
        {
            get
            {
                double z = 0;
                for (int i = 0; i < arraySize; i++)
                {
                    for (int j = 0; j < arraySize; j++)
                    {
                        z = Math.Max(PointsArray[i, j].Z, z);
                    }
                }

                return z;
            }
        }
        public void Draw(Matrix4d M)
        {

            //for (int i = 0; i < arraySize; i++)
            //{
            //    for (int j = 0; j < arraySize; j++)
            //    {
            //        if (PointsArray[i, j].Z>0.01 || PointsArray[i, j].Z < -0.01) PointsArray[i, j].Draw(M,10,0.3,0.1,0.6);

            //    }
            //}

            for (int i = 0; i < arraySize; i++)
            {
                for (int j = 0; j < arraySize; j++)
                {
                    // if (PointsArray[i, j].Z > 0.01 || PointsArray[i, j].Z < -0.01) (new Point(delta * (i - arraySize / 2), delta*(j - arraySize / 2), PointsArray[i, j].Z)).Draw(M,10,1,1,0);
                   (new Point(delta * (i - arraySize / 2), delta * (j - arraySize / 2), PointsArray[i, j].Z)).Draw(M, 10, 1, 1, 0);
                   // PointsArray[i, j].Draw(M, 10, 1, 0, 0);
                }
            }
        }

        private int k = 0;
        private int step = 1;
      
        private double EpsilonHeight = 0.1;
        private double cutterDiameter = 1.6;

        double getHeight(int k, int i)
        {
            int bracketRadius = (int)(Math.Ceiling(cutterDiameter / delta) / 2); //podziałek na mm
            double tempHeight = 0;
            for (int j = 0; j < bracketRadius; j++)
            {

                for (int l = 0; l < bracketRadius; l++)
                {
                    if ((k + j) < (PointsArray.GetLength(0)-1) && (i + l) < (PointsArray.GetLength(1)-1))
                    {
                        tempHeight = Math.Max(tempHeight, PointsArray[k + j, i + l].Z);
                    }
                }

            }

            return tempHeight + EpsilonHeight;
        }

        public void GenerateInitialPathStep(List<Point> List)
        {
            double jump = 0.6;
            double size = 15;
            double epsilon = 0.5;

            double MoveY = 0;
            List.Add(new Point(-size / 2, -size / 2 + MoveY, 10));
            for (int i = 0; i < (int)Math.Ceiling(size / (2 * jump)) + 1; i++)
            {
                List.Add(new Point(-size / 2, -size / 2 + MoveY, Z_max + epsilon));
                List.Add(new Point(size / 2, -size / 2 + MoveY, Z_max + epsilon));
                MoveY += jump;
                List.Add(new Point(size / 2, -size / 2 + MoveY, Z_max + epsilon));
                List.Add(new Point(-size / 2, -size / 2 + MoveY, Z_max + epsilon));
                MoveY += jump;
            }
            List.Add(new Point(-size / 2, -size / 2 + MoveY - jump, 10));
        }
        public void GenerateInitialPathStep2(List<Point> List)
        {
            List.Add(new Point(delta * (-arraySize / 2) + delta / 2, delta * (-arraySize / 2) + delta / 2, 10));
            Tuple<int, int> nextPointCoordinates;
            for (int i = 0; i < PointsArray.GetLength(0); i++)
            {
                for (int j = 0; j < PointsArray.GetLength(1); j++)
                {
                    // List.Add(new Point(delta * (k - arraySize / 2)+delta/2, delta * (i - arraySize / 2)+delta/2, PointsArray[k, i].Z));
                    Point nextPoint;

                    //List.Add(new Point(delta * (k - arraySize / 2) + delta / 2, delta * (i - arraySize / 2) + delta / 2, PointsArray[k, i].Z + EpsilonHeight));
                    List.Add(new Point(delta * (k - arraySize / 2) + delta / 2, delta * (i - arraySize / 2) + delta / 2, getHeight(k, i)));
                    if (j != (PointsArray.GetLength(1) - 1))
                    {
                        nextPoint = PointsArray[k + step, i];
                        nextPointCoordinates = new Tuple<int, int>(k + step, i);
                    }
                    else // (j == (PointsArray.GetLength(1) - 1))
                    {
                        if (i == (PointsArray.GetLength(0) - 1))
                        {
                            nextPoint = PointsArray[k, i];
                            nextPointCoordinates = new Tuple<int, int>(k, i);
                        }

                        else
                        {
                            nextPoint = PointsArray[k, i + 1];
                            nextPointCoordinates = new Tuple<int, int>(k, i + 1);
                        }
                    }


                    if (nextPoint.Z >= PointsArray[k, i].Z)
                    {
                        List.Add(new Point(delta * (k - arraySize / 2) + delta / 2, delta * (i - arraySize / 2) + delta / 2, getHeight(nextPointCoordinates.Item1, nextPointCoordinates.Item2)));
                    }
                    if (nextPoint.Z < PointsArray[k, i].Z)
                    {
                        List.Add(new Point(delta * (nextPointCoordinates.Item1 - arraySize / 2) + delta / 2, delta * (nextPointCoordinates.Item2 - arraySize / 2) + delta / 2, getHeight(k, i) ));
                    }


                    k += step;
                }

                k -= step;
                step = -step;

            }

            List.Add(new Point(-delta * (PointsArray.GetLength(0) - 1 - arraySize / 2) + delta / 2, delta * (PointsArray.GetLength(1) - 1 - arraySize / 2) + delta / 2, 10));
        }

    }
}
