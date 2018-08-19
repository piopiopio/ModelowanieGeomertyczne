using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using OpenTK;

namespace ModelowanieGeometryczne.Model
{
    public class GregoryPatch
    {

        public List<Point[,]> BezierArraysBasedOnGregory = new List<Point[,]>();
        public List<Point> Q;
        public Point P0;
        public List<Point[]> P3 = new List<Point[]>();
        public List<Point> P1 = new List<Point>();
        public List<Point> P12 = new List<Point>();
        public List<Point> P13 = new List<Point>();
        public List<Point> P14 = new List<Point>();
        public List<Point> P8 = new List<Point>();


        public List<Point[,]> PointsOnBoundaryAndC1ConditionPoints = new List<Point[,]>();

        public Point[,] GetfiveInnerPoints(List<Point> selectedTwoPoints, ObservableCollection<BezierPatch> bezierPatchCollection)
        {
            foreach (var item in bezierPatchCollection)
            {
                var temp = item.GetMiddlePointBeetweenTwoPoints(selectedTwoPoints);
                if (temp != null)
                {
                    P3.Add(temp);
                }

                var fivePointsBeetweenTwoPointsTemporaryArray = item.GetFivePointsBeetweenTwoPoints(selectedTwoPoints);
                if (fivePointsBeetweenTwoPointsTemporaryArray != null)
                {
                    return fivePointsBeetweenTwoPointsTemporaryArray;
                }

            }

            return null;
        }

        public GregoryPatch(ObservableCollection<BezierPatch> bezierPatchCollection)
        {
            const double u = 0.5;
            const double v = 0.5;
            BezierArraysBasedOnGregory.Add(new Point[4, 4]);
            BezierArraysBasedOnGregory.Add(new Point[4, 4]);
            BezierArraysBasedOnGregory.Add(new Point[4, 4]);
            Q = new List<Point>();
            var selectedPoints = new List<Point>();
            

            foreach (var item in bezierPatchCollection)
            {
                var temp = item.GetAllPointsInOneArray();

                for (var i = 0; i < temp.GetLength(0); i++)

                    for (var j = 0; j < temp.GetLength(1); j++)
                    {
                        if (temp[i, j].Selected == true) selectedPoints.Add(temp[i, j]);
                    }
            }


            PointsOnBoundaryAndC1ConditionPoints.Add(GetfiveInnerPoints(new List<Point> {selectedPoints[0], selectedPoints[1]}, bezierPatchCollection));
            PointsOnBoundaryAndC1ConditionPoints.Add(GetfiveInnerPoints(new List<Point> { selectedPoints[1], selectedPoints[2] }, bezierPatchCollection));
            PointsOnBoundaryAndC1ConditionPoints.Add(GetfiveInnerPoints(new List<Point> { selectedPoints[2], selectedPoints[0] }, bezierPatchCollection));
           
            if (P3.Count != 3) { MessageBox.Show("To less P3 points, 3 point are required"); return; }

            for (var i = 0; i < 3; i++)
            {
                Q.Add(P3[i][0].Add((P3[i][1].Subtract(P3[i][0])).Multiply(3.0 / 2.0)));
            }
            P0 = P3[0][0].Add(P3[1][0].Add(P3[2][0])).Multiply(1.0 / 3.0);

            for (var i = 0; i < 3; i++)
            {
                P1.Add(((Q[i].Multiply(2)).Add(P0)).Multiply(1.0 / 3.0));
            }




            //TODO: Sprawdzać czy nie puste!!!

            for (var i = 0; i < 3; i++)
            {   List<Point> UsedToCalculateRestOfPoints = new List<Point>();
                UsedToCalculateRestOfPoints = new List<Point>();
                UsedToCalculateRestOfPoints.Add(PointsOnBoundaryAndC1ConditionPoints[i][1, 0]);//0  p5
                UsedToCalculateRestOfPoints.Add(PointsOnBoundaryAndC1ConditionPoints[i][1, 1]);//1  p11
                UsedToCalculateRestOfPoints.Add(PointsOnBoundaryAndC1ConditionPoints[i][1, 2]);//2  p17
                UsedToCalculateRestOfPoints.Add(PointsOnBoundaryAndC1ConditionPoints[i][1, 3]);//3  p7
                UsedToCalculateRestOfPoints.Add(PointsOnBoundaryAndC1ConditionPoints[i][1, 4]);//4  pxx
                UsedToCalculateRestOfPoints.Add(Q[i]);//5  p16
                UsedToCalculateRestOfPoints.Add(P1[i]);//6  p18

                P12.Add(UsedToCalculateRestOfPoints[2] + (UsedToCalculateRestOfPoints[0] - (UsedToCalculateRestOfPoints[2])).Multiply(0.5)); //p12
                P8.Add(UsedToCalculateRestOfPoints[2] - (UsedToCalculateRestOfPoints[0] - (UsedToCalculateRestOfPoints[2])).Multiply(0.5)); //p8
                P13.Add(UsedToCalculateRestOfPoints[6] + (UsedToCalculateRestOfPoints[3] - (UsedToCalculateRestOfPoints[6])).Multiply(1.0 / 3.0)); //p13
                P14.Add(UsedToCalculateRestOfPoints[6] - (UsedToCalculateRestOfPoints[3] - (UsedToCalculateRestOfPoints[6])).Multiply(1.0 / 3.0)); //p14
            }



           
            //UsedToCalculateRestOfPoints.Add(PointsOnBoundaryAndC1ConditionPoints[1][1, 0]);//0  p5
            //UsedToCalculateRestOfPoints.Add(PointsOnBoundaryAndC1ConditionPoints[1][1, 1]);//1  p11
            //UsedToCalculateRestOfPoints.Add(PointsOnBoundaryAndC1ConditionPoints[1][1, 2]);//2  p17
            //UsedToCalculateRestOfPoints.Add(PointsOnBoundaryAndC1ConditionPoints[1][1, 3]);//3  p7
            //UsedToCalculateRestOfPoints.Add(PointsOnBoundaryAndC1ConditionPoints[1][1, 4]);//4  pxx
            //UsedToCalculateRestOfPoints.Add(Q[1]);//5  p16
            //UsedToCalculateRestOfPoints.Add(P1[1]);//6  p18

            //P12.Add(UsedToCalculateRestOfPoints[2] + (UsedToCalculateRestOfPoints[0] - (UsedToCalculateRestOfPoints[2])).Multiply(0.5)); //p12
            //P8.Add(UsedToCalculateRestOfPoints[2] - (UsedToCalculateRestOfPoints[0] - (UsedToCalculateRestOfPoints[2])).Multiply(0.5)); //p8
            //P13.Add(UsedToCalculateRestOfPoints[6] + (UsedToCalculateRestOfPoints[3] - (UsedToCalculateRestOfPoints[6])).Multiply(1.0 / 3.0)); //p13
            //P14.Add(UsedToCalculateRestOfPoints[6] - (UsedToCalculateRestOfPoints[3] - (UsedToCalculateRestOfPoints[6])).Multiply(1.0 / 3.0)); //p14

            //UsedToCalculateRestOfPoints = new List<Point>();
            
            //UsedToCalculateRestOfPoints.Add(PointsOnBoundaryAndC1ConditionPoints[2][1, 0]);//0  p5
            //UsedToCalculateRestOfPoints.Add(PointsOnBoundaryAndC1ConditionPoints[2][1, 1]);//1  p11
            //UsedToCalculateRestOfPoints.Add(PointsOnBoundaryAndC1ConditionPoints[2][1, 2]);//2  p17
            //UsedToCalculateRestOfPoints.Add(PointsOnBoundaryAndC1ConditionPoints[2][1, 3]);//3  p7
            //UsedToCalculateRestOfPoints.Add(PointsOnBoundaryAndC1ConditionPoints[2][1, 4]);//4  pxx
            //UsedToCalculateRestOfPoints.Add(Q[2]);//5  p16
            //UsedToCalculateRestOfPoints.Add(P1[2]);//6  p18

            //P12.Add(UsedToCalculateRestOfPoints[2] + (UsedToCalculateRestOfPoints[0] - (UsedToCalculateRestOfPoints[2])).Multiply(0.5)); //p12
            //P8.Add(UsedToCalculateRestOfPoints[2] - (UsedToCalculateRestOfPoints[0] - (UsedToCalculateRestOfPoints[2])).Multiply(0.5)); //p8
            //P13.Add(UsedToCalculateRestOfPoints[6] + (UsedToCalculateRestOfPoints[3] - (UsedToCalculateRestOfPoints[6])).Multiply(1.0 / 3.0)); //p13
            //P14.Add(UsedToCalculateRestOfPoints[6] - (UsedToCalculateRestOfPoints[3] - (UsedToCalculateRestOfPoints[6])).Multiply(1.0 / 3.0)); //p14


            BezierArraysBasedOnGregory[0][0, 0] = selectedPoints[0];
            BezierArraysBasedOnGregory[0][0, 1] = PointsOnBoundaryAndC1ConditionPoints[0][0, 0];
            BezierArraysBasedOnGregory[0][0, 2] = PointsOnBoundaryAndC1ConditionPoints[0][0, 1];
            BezierArraysBasedOnGregory[0][0, 3] = PointsOnBoundaryAndC1ConditionPoints[0][0, 2];

            BezierArraysBasedOnGregory[0][1, 0] = PointsOnBoundaryAndC1ConditionPoints[2][0, 4];
            BezierArraysBasedOnGregory[0][1, 1] = u * PointsOnBoundaryAndC1ConditionPoints[2][1, 4] + v * PointsOnBoundaryAndC1ConditionPoints[0][1, 0];  //Test ok
            BezierArraysBasedOnGregory[0][1, 2] = u * P12[0] + v * PointsOnBoundaryAndC1ConditionPoints[0][1, 1]; //test ok
            BezierArraysBasedOnGregory[0][1, 3] = PointsOnBoundaryAndC1ConditionPoints[0][1, 2];
            BezierArraysBasedOnGregory[0][2, 0] = PointsOnBoundaryAndC1ConditionPoints[2][0, 3];
            BezierArraysBasedOnGregory[0][2, 1] = u * P8[2] + v * PointsOnBoundaryAndC1ConditionPoints[2][1, 3]; //Test ok
            BezierArraysBasedOnGregory[0][2, 2] = u * P14[0] + v * P13[2]; //Test ok
            BezierArraysBasedOnGregory[0][2, 3] = P1[0];
            BezierArraysBasedOnGregory[0][3, 0] = PointsOnBoundaryAndC1ConditionPoints[2][0, 2];
            BezierArraysBasedOnGregory[0][3, 1] = PointsOnBoundaryAndC1ConditionPoints[2][1, 2];
            BezierArraysBasedOnGregory[0][3, 2] = P1[2];
            BezierArraysBasedOnGregory[0][3, 3] = P0;


            BezierArraysBasedOnGregory[1][0, 0] = selectedPoints[1];
            BezierArraysBasedOnGregory[1][0, 1] = PointsOnBoundaryAndC1ConditionPoints[1][0, 0];
            BezierArraysBasedOnGregory[1][0, 2] = PointsOnBoundaryAndC1ConditionPoints[1][0, 1];
            BezierArraysBasedOnGregory[1][0, 3] = PointsOnBoundaryAndC1ConditionPoints[1][0, 2];
            BezierArraysBasedOnGregory[1][1, 0] = PointsOnBoundaryAndC1ConditionPoints[0][0, 4];
            BezierArraysBasedOnGregory[1][1, 1] = u * PointsOnBoundaryAndC1ConditionPoints[0][1, 4] + v * PointsOnBoundaryAndC1ConditionPoints[1][1, 0];  //Test
            BezierArraysBasedOnGregory[1][1, 2] = u * P12[1] + v * PointsOnBoundaryAndC1ConditionPoints[1][1, 1]; //Test
            BezierArraysBasedOnGregory[1][1, 3] = PointsOnBoundaryAndC1ConditionPoints[1][1, 2];
            BezierArraysBasedOnGregory[1][2, 0] = PointsOnBoundaryAndC1ConditionPoints[0][0, 3];
            BezierArraysBasedOnGregory[1][2, 1] = u * P8[0] + v * PointsOnBoundaryAndC1ConditionPoints[0][1, 3]; //Test
            BezierArraysBasedOnGregory[1][2, 2] = u * P14[1] + v * P13[0]; //Test
            BezierArraysBasedOnGregory[1][2, 3] = P1[1];
            BezierArraysBasedOnGregory[1][3, 0] = PointsOnBoundaryAndC1ConditionPoints[0][0, 2];
            BezierArraysBasedOnGregory[1][3, 1] = PointsOnBoundaryAndC1ConditionPoints[0][1, 2];
            BezierArraysBasedOnGregory[1][3, 2] = P1[0];
            BezierArraysBasedOnGregory[1][3, 3] = P0;


            BezierArraysBasedOnGregory[2][0, 0] = selectedPoints[2];
            BezierArraysBasedOnGregory[2][0, 1] = PointsOnBoundaryAndC1ConditionPoints[2][0, 0];
            BezierArraysBasedOnGregory[2][0, 2] = PointsOnBoundaryAndC1ConditionPoints[2][0, 1];
            BezierArraysBasedOnGregory[2][0, 3] = PointsOnBoundaryAndC1ConditionPoints[2][0, 2];
            BezierArraysBasedOnGregory[2][1, 0] = PointsOnBoundaryAndC1ConditionPoints[1][0, 4];
            BezierArraysBasedOnGregory[2][1, 1] = u * PointsOnBoundaryAndC1ConditionPoints[1][1, 4] + v * PointsOnBoundaryAndC1ConditionPoints[2][1, 0];  //Test
            BezierArraysBasedOnGregory[2][1, 2] = u * P12[2] + v * PointsOnBoundaryAndC1ConditionPoints[2][1, 1]; //Test
            BezierArraysBasedOnGregory[2][1, 3] = PointsOnBoundaryAndC1ConditionPoints[2][1, 2];
            BezierArraysBasedOnGregory[2][2, 0] = PointsOnBoundaryAndC1ConditionPoints[1][0, 3];
            BezierArraysBasedOnGregory[2][2, 1] = u * P8[1] + v * PointsOnBoundaryAndC1ConditionPoints[1][1, 3]; //Test
            BezierArraysBasedOnGregory[2][2, 2] = u * P14[2] + v * P13[1]; //Test
            BezierArraysBasedOnGregory[2][2, 3] = P1[2];
            BezierArraysBasedOnGregory[2][3, 0] = PointsOnBoundaryAndC1ConditionPoints[1][0, 2];
            BezierArraysBasedOnGregory[2][3, 1] = PointsOnBoundaryAndC1ConditionPoints[1][1, 2];
            BezierArraysBasedOnGregory[2][3, 2] = P1[1];
            BezierArraysBasedOnGregory[2][3, 3] = P0;

          
        }

        public GregoryPatch(BezierPatch P0, BezierPatch P1, BezierPatch P2)
        {

        }

        public static Point MergePoints(Point p0, Point p1)
        {
            return new Point((p0.X + p1.X) / 2, (p0.Y + p1.Y) / 2, (p0.Z + p1.Z) / 2);
        }

        public void Draw(Matrix4d transformacja)
        {
            int k = 0;
            if (Q != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        k += 2;
                        BezierArraysBasedOnGregory[0][i, j].Draw(transformacja, k, 1, 0, 0);
                        BezierArraysBasedOnGregory[1][i, j].Draw(transformacja, k, 0, 1, 0);
                        BezierArraysBasedOnGregory[2][i, j].Draw(transformacja, k, 0, 0, 1);
                    }


                }
            }
        }

        public static void MergePoints(ObservableCollection<BezierPatch> BezierPatchCollection)
        {
            List<Tuple<int, int, int>> BezierPatchIterator = new List<Tuple<int, int, int>>();

            int i = 0;

            foreach (var patch in BezierPatchCollection)
            {
                patch.PatchPoints = patch.GetAllPointsInOneArray();
                for (int j = 0; j < patch.PatchPoints.GetLength(0); j++)
                {
                    for (int k = 0; k < patch.PatchPoints.GetLength(1); k++)
                    {
                        if (patch.PatchPoints[j, k].Selected)
                        {
                            //Sprawdzanie czy punkt leży na rogu płatka
                            if (j == 0 || j == (patch.PatchPoints.GetLength(0) - 1))
                            {
                                if (k == 0 || k == (patch.PatchPoints.GetLength(1) - 1))
                                {
                                    BezierPatchIterator.Add(new Tuple<int, int, int>(i, j, k));

                                }
                            }
                        }
                    }
                }
                i++;
            }



            if (BezierPatchIterator.Count == 2)
            {
                var GregoryPatchMergedPoint = GregoryPatch.MergePoints(
                    BezierPatchCollection[BezierPatchIterator[0].Item1].PatchPoints[BezierPatchIterator[0].Item2, BezierPatchIterator[0].Item3],
                    BezierPatchCollection[BezierPatchIterator[1].Item1].PatchPoints[BezierPatchIterator[1].Item2, BezierPatchIterator[1].Item3]);

                //GregoryPatch1.P3.Add(GregoryPatchMergedPoint);

                BezierPatchCollection[BezierPatchIterator[0].Item1].PatchPoints[BezierPatchIterator[0].Item2, BezierPatchIterator[0].Item3] = GregoryPatchMergedPoint;
                BezierPatchCollection[BezierPatchIterator[1].Item1].PatchPoints[BezierPatchIterator[1].Item2, BezierPatchIterator[1].Item3] = GregoryPatchMergedPoint;

                BezierPatchCollection[BezierPatchIterator[0].Item1].PlaceVerticesToPatches4x4();
                BezierPatchCollection[BezierPatchIterator[1].Item1].PlaceVerticesToPatches4x4();

                foreach (var item in BezierPatchCollection)
                {
                    item.RecalculatePatches();
                }
                

            }
            else
            {
                MessageBox.Show("Not correct points");
            }

        }
    }
}
