using System;
using System.Collections.Generic;
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
        public void ConsiderList(List<Point> L)
        {
            foreach (var item1 in L)
            {
                ConsiderPoint(item1);
            }
        }
        public void ConsiderPoint(Point p)
        {
            int Xcoord = Math.Min((int) Math.Floor((p.X + dimension / 2) / factor), arraySize - 1);
            int Ycoord = Math.Min((int) Math.Floor((p.Y + dimension / 2) / factor), arraySize - 1);


            if (PointsArray[Xcoord, Ycoord].Z <= p.Z)
            {
                PointsArray[Xcoord, Ycoord]= p;
            }
        }

        public double Z_max
        {
            get
            {
                double z=0;
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
                }
            }
        }

        private int k = 0;
        private int step = 1;
        public void GenerateInitialPathStep2(List<Point>  List)
        {
            List.Add(new Point(delta * ( -arraySize / 2) + delta / 2, delta * ( -arraySize / 2) + delta / 2, 10));
            for (int i = 0; i < PointsArray.GetLength(0); i++)
            {
                for (int j = 0; j < PointsArray.GetLength(1); j++)
                {
                    List.Add(new Point(delta * (k - arraySize / 2)+delta/2, delta * (i - arraySize / 2)+delta/2, PointsArray[k, i].Z));
                    k += step;
                }

                k -=step;
               step = -step;

            }

            List.Add(new Point(-delta * (PointsArray.GetLength(0) -1- arraySize / 2) + delta / 2, delta * (PointsArray.GetLength(1) -1- arraySize / 2) + delta / 2, 10));
        }
    }
}
