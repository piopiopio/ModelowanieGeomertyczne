using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using ModelowanieGeometryczne.Helpers;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ModelowanieGeometryczne.Model
{
    public class GregoryPatch
    {
        public bool Selected { get; set; }
        private static int iterator;
        public List<Patch> PatchCollection = new List<Patch>();
        public List<Point[,]> BezierArraysBasedOnGregory;
        public List<Point> Q;
        public Point P0;
        public List<Point[]> P3;
        public List<Point> P1;
        public List<Point> P12;
        public List<Point> P13;
        public List<Point> P14;
        public List<Point> P8;
        public List<Point> selectedPoints = new List<Point>();
        ObservableCollection<BezierPatch> BezierPatchCollection = new ObservableCollection<BezierPatch>();
        public List<Point[][]> EgdePoints = new List<Point[][]>();
        public List<Point[][]> ControlArrayC1 = new List<Point[][]>();
        private const int MaxDivisionValue = 20;
        private int _patchHorizontalDivision;
        public int PatchHorizontalDivision
        {
            get
            { return _patchHorizontalDivision; }
            set
            {
                var a = value;

                if (a < 1) _patchHorizontalDivision = 1;
                else if (a > MaxDivisionValue) _patchHorizontalDivision = MaxDivisionValue;
                else _patchHorizontalDivision = a;
                foreach (var patch in PatchCollection)
                {
                    patch.u = _patchHorizontalDivision;
                }
            }
        }

        private int _patchVerticalDivision;
        public int PatchVerticalDivision
        {
            get
            { return _patchVerticalDivision; }
            set
            {
                var a = value;


                if (a < 1) _patchVerticalDivision = 1;
                else if (a > MaxDivisionValue) _patchVerticalDivision = MaxDivisionValue;
                else
                {
                    _patchVerticalDivision = a;
                }
                foreach (var patch in PatchCollection)
                {
                    patch.v = _patchVerticalDivision;
                }
            }
        }
        public bool VectorsVisibility { get; set; }
        public string Name { get; set; }
        public List<Point[,]> PointsOnBoundaryAndC1ConditionPoints;

        public Point[][] GetFourEgdePoints(List<Point> selectedTwoPoints, ObservableCollection<BezierPatch> bezierPatchCollection)
        {
            foreach (var item in bezierPatchCollection)
            {
                Point[][] fourEdgeControlPoints = new Point[2][];
                fourEdgeControlPoints = item.GetFourEdgeControlPoints(selectedTwoPoints);



                if (fourEdgeControlPoints != null)
                {
                    return fourEdgeControlPoints;
                }

            }

            return null;
        }


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

        public Point[,] CalculateBezierBasedOnGregory(List<Point> selectedPoints, int i, int j, List<Point[][]> cp)
        {
            const double u = 0.5;
            const double v = 0.5;
            Point[,] bezierArrayBasedOnGregory = new Point[4, 4];
            //bezierArrayBasedOnGregory[0, 0] = selectedPoints[i];
            //bezierArrayBasedOnGregory[0, 1] = PointsOnBoundaryAndC1ConditionPoints[i][0, 0];
            //bezierArrayBasedOnGregory[0, 2] = PointsOnBoundaryAndC1ConditionPoints[i][0, 1];
            //bezierArrayBasedOnGregory[0, 3] = PointsOnBoundaryAndC1ConditionPoints[i][0, 2];
            //bezierArrayBasedOnGregory[1, 0] = PointsOnBoundaryAndC1ConditionPoints[j][0, 4];
            //// bezierArrayBasedOnGregory[1, 1] = u * PointsOnBoundaryAndC1ConditionPoints[j][1, 4] + v * PointsOnBoundaryAndC1ConditionPoints[i][1, 0];  //Test ok
            //bezierArrayBasedOnGregory[1, 1] = PointsOnBoundaryAndC1ConditionPoints[i][1, 0];  //Test ok
            ////bezierArrayBasedOnGregory[1, 2] = u * P12[i] + v * PointsOnBoundaryAndC1ConditionPoints[i][1, 1]; //test ok
            //bezierArrayBasedOnGregory[1, 2] = PointsOnBoundaryAndC1ConditionPoints[i][1, 1]; //test ok
            //bezierArrayBasedOnGregory[1, 3] = PointsOnBoundaryAndC1ConditionPoints[i][1, 2];
            //bezierArrayBasedOnGregory[2, 0] = PointsOnBoundaryAndC1ConditionPoints[j][0, 3];
            ////bezierArrayBasedOnGregory[2, 1] = u * P8[j] + v * PointsOnBoundaryAndC1ConditionPoints[j][1, 3]; //Test ok
            //bezierArrayBasedOnGregory[2, 1] = PointsOnBoundaryAndC1ConditionPoints[j][1, 3]; //Test ok
            //bezierArrayBasedOnGregory[2, 2] = u * P14[i] + v * P13[j]; //Test ok
            //bezierArrayBasedOnGregory[2, 3] = P1[i];
            //bezierArrayBasedOnGregory[3, 0] = PointsOnBoundaryAndC1ConditionPoints[j][0, 2];
            //bezierArrayBasedOnGregory[3, 1] = PointsOnBoundaryAndC1ConditionPoints[j][1, 2];
            //bezierArrayBasedOnGregory[3, 2] = P1[j];
            //bezierArrayBasedOnGregory[3, 3] = P0;

            bezierArrayBasedOnGregory[0, 0] = cp[i][0][0];
            bezierArrayBasedOnGregory[0, 1] = cp[i][0][1];
            bezierArrayBasedOnGregory[0, 2] = cp[i][0][2];
            bezierArrayBasedOnGregory[0, 3] = cp[i][0][3];
            bezierArrayBasedOnGregory[1, 0] = cp[j][0][5];
            bezierArrayBasedOnGregory[1, 1] = cp[i][1][1];  //Test 
            bezierArrayBasedOnGregory[1, 2] = cp[i][1][2]; //test ok
            bezierArrayBasedOnGregory[1, 3] = cp[i][1][3];
            bezierArrayBasedOnGregory[2, 0] = cp[j][0][4];
            bezierArrayBasedOnGregory[2, 1] = cp[j][1][4]; //Test ok
            bezierArrayBasedOnGregory[2, 2] = u * P14[i] + v * P13[j]; //Test ok
            bezierArrayBasedOnGregory[2, 3] = P1[i];
            bezierArrayBasedOnGregory[3, 0] = cp[j][0][3];
            bezierArrayBasedOnGregory[3, 1] = cp[j][1][3];
            bezierArrayBasedOnGregory[3, 2] = P1[j];
            bezierArrayBasedOnGregory[3, 3] = P0;
            return bezierArrayBasedOnGregory;


        }



        public void CalculateGregoryPatch()
        {

            Q = new List<Point>();
            P3 = new List<Point[]>();
            P1 = new List<Point>();
            P12 = new List<Point>();
            P13 = new List<Point>();
            P14 = new List<Point>();
            P8 = new List<Point>();
            PatchCollection = new List<Patch>();
            BezierArraysBasedOnGregory = new List<Point[,]>();
            PointsOnBoundaryAndC1ConditionPoints = new List<Point[,]>();

            EgdePoints.Add(GetFourEgdePoints(new List<Point> { selectedPoints[0], selectedPoints[1] }, BezierPatchCollection));
            EgdePoints.Add(GetFourEgdePoints(new List<Point> { selectedPoints[1], selectedPoints[2] }, BezierPatchCollection));
            EgdePoints.Add(GetFourEgdePoints(new List<Point> { selectedPoints[2], selectedPoints[0] }, BezierPatchCollection));

            PointsOnBoundaryAndC1ConditionPoints.Add(GetfiveInnerPoints(new List<Point> { selectedPoints[0], selectedPoints[1] }, BezierPatchCollection));
            PointsOnBoundaryAndC1ConditionPoints.Add(GetfiveInnerPoints(new List<Point> { selectedPoints[1], selectedPoints[2] }, BezierPatchCollection));
            PointsOnBoundaryAndC1ConditionPoints.Add(GetfiveInnerPoints(new List<Point> { selectedPoints[2], selectedPoints[0] }, BezierPatchCollection));

            if (P3.Count != 3)
            {
                MessageBox.Show("3 corner points are required");
                throw new System.ArgumentException("To less corner points, 3 corner points are required", "original");

                return;
            }

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
            {
                List<Point> usedToCalculateRestOfPoints = new List<Point>
                {
                    PointsOnBoundaryAndC1ConditionPoints[i][1, 0], //0  p5
                    PointsOnBoundaryAndC1ConditionPoints[i][1, 1], //1  p11
                    PointsOnBoundaryAndC1ConditionPoints[i][1, 2], //2  p17
                    PointsOnBoundaryAndC1ConditionPoints[i][1, 3], //3  p7
                    PointsOnBoundaryAndC1ConditionPoints[i][1, 4], //4  pxx
                    Q[i],                    //5  p16
                    P1[i]                    //6  p18    
                };


                P12.Add(usedToCalculateRestOfPoints[2] + (usedToCalculateRestOfPoints[0] - (usedToCalculateRestOfPoints[2])).Multiply(0.5)); //p12
                P8.Add(usedToCalculateRestOfPoints[2] - (usedToCalculateRestOfPoints[0] - (usedToCalculateRestOfPoints[2])).Multiply(0.5)); //p8
                P13.Add(usedToCalculateRestOfPoints[6] + (usedToCalculateRestOfPoints[3] - (usedToCalculateRestOfPoints[6])).Multiply(1.0 / 3.0)); //p13
                P14.Add(usedToCalculateRestOfPoints[6] - (usedToCalculateRestOfPoints[3] - (usedToCalculateRestOfPoints[6])).Multiply(1.0 / 3.0)); //p14
            }





            ControlArrayC1.Clear();
            ControlArrayC1.Add(GetFiveMiddleControlPoints(EgdePoints[0]));
            ControlArrayC1.Add(GetFiveMiddleControlPoints(EgdePoints[1]));
            ControlArrayC1.Add(GetFiveMiddleControlPoints(EgdePoints[2]));

            BezierArraysBasedOnGregory.Add(CalculateBezierBasedOnGregory(selectedPoints, 0, 2, ControlArrayC1));
            BezierArraysBasedOnGregory.Add(CalculateBezierBasedOnGregory(selectedPoints, 1, 0, ControlArrayC1));
            BezierArraysBasedOnGregory.Add(CalculateBezierBasedOnGregory(selectedPoints, 2, 1, ControlArrayC1));
            PatchCollection.Add(new Patch(BezierArraysBasedOnGregory[0], _patchHorizontalDivision, _patchVerticalDivision));
            PatchCollection.Add(new Patch(BezierArraysBasedOnGregory[1], _patchHorizontalDivision, _patchVerticalDivision));
            PatchCollection.Add(new Patch(BezierArraysBasedOnGregory[2], _patchHorizontalDivision, _patchVerticalDivision));
        }

        public void DrawVectors(Matrix4d transformacja)
        {
            if (VectorsVisibility)
            {
                GL.Begin(BeginMode.Lines);
                GL.Color3(1.0, 1.0, 0);
                foreach (var item in ControlArrayC1)
                {
                    for (int i = 1; i < 6; i++)
                    {
                        Matrix4d projection = MatrixProvider.ProjectionMatrix();
                        var a = projection.Multiply(transformacja.Multiply(item[0][i]));
                        var b = projection.Multiply(transformacja.Multiply(item[1][i]));
                        var c = projection.Multiply(transformacja.Multiply(2 * item[0][i] - item[1][i]));

                        //GL.Vertex2(new Vector2d(a.X / 720, a.Y / 375));
                        //GL.Vertex2(new Vector2d(b.X / 720, b.Y / 375));
                        //GL.Vertex2(new Vector2d((2*(item[0][i].X_Window / (1440.0 / 2.0) )- item[1][i].X_Window /(1440.0 / 2.0)), (2* (item[0][i].Y_Window / (1440.0 / 2.0) )- item[1][i].Y_Window / (1440.0 / 2.0))));
                        //GL.Vertex2(new Vector2d(item[1][i].X_Window/ (1440.0 / 2.0) , item[1][i].Y_Window/(750.0 / 2.0)));
                        GL.Vertex2(new Vector2d(c.X, c.Y));
                        GL.Vertex2(new Vector2d(b.X, b.Y));
                    }
                }

                GL.End();

            }
        }


        public GregoryPatch(ObservableCollection<BezierPatch> bezierPatchCollection)
        {
            PatchHorizontalDivision = 4;
            PatchVerticalDivision = 4;
            VectorsVisibility = false;
            iterator++;
            Name = "Gregory patches number:" + iterator;
            BezierPatchCollection = bezierPatchCollection;

            foreach (var item in BezierPatchCollection)
            {
                var temp = item.GetAllPointsInOneArray();

                for (var i = 0; i < temp.GetLength(0); i++)

                    for (var j = 0; j < temp.GetLength(1); j++)
                    {
                        if (temp[i, j].Selected == true)
                            selectedPoints.Add(temp[i, j]);
                    }
            }

            selectedPoints = selectedPoints.Distinct().ToList();
                CalculateGregoryPatch();



            PatchHorizontalDivision = 4;
            PatchVerticalDivision = 4;
            VectorsVisibility = true;

        }


        public static Point MergePoints(Point p0, Point p1)
        {
            return new Point((p0.X + p1.X) / 2, (p0.Y + p1.Y) / 2, (p0.Z + p1.Z) / 2);
        }

        //double B0(double t)
        //{
        //    return (1 -3*t+3*t*t-t*t*t);
        //}

        //double B1(double t)
        //{
        //    return 3*t-6*t*t+3*t*t*t;
        //}

        //double B2(double t)
        //{
        //    return 3 * t * t - 3 * t * t * t;
        //}

        //double B3(double t)
        //{
        //    return t * t * t;
        //}


        public Point[][] GetFiveMiddleControlPoints(Point[][] P)
        {
            //P - 4 punkty konterolne krzywej beziera (pierwszy wiersz i drugi wiersz)
            Point[][] controlArray = new Point[2][];
            controlArray[0] = new Point[7];
            controlArray[1] = new Point[7];

            controlArray[0][0] = P[0][0];
            controlArray[0][6] = P[0][3];
            controlArray[0][1] = (P[0][0] + P[0][1]) / 2;
            controlArray[0][2] = (controlArray[0][1] + (P[0][1] + P[0][2]) / 2) / 2;
            controlArray[0][5] = (P[0][2] + P[0][3]) / 2;
            controlArray[0][4] = (controlArray[0][5] + (P[0][1] + P[0][2]) / 2) / 2;
            controlArray[0][3] = (controlArray[0][2] + controlArray[0][4]) / 2;


            controlArray[1][0] = null;
            controlArray[1][6] = null;
            //controlArray[1][1] = 2 * controlArray[0][1] - ((P[1][0] + P[1][1]) / 2);
            //controlArray[1][2] = 2 * controlArray[0][2] - ((controlArray[1][1] + (P[1][1] + P[1][2]) / 2) / 2);
            //controlArray[1][5] = 2 * controlArray[0][5] - ((P[1][2] + P[1][3]) / 2);
            //controlArray[1][4] = 2 * controlArray[0][4] - ((controlArray[1][5] + (P[1][1] + P[1][2]) / 2) / 2);
            //controlArray[1][3] = 2 * controlArray[0][3] - ((controlArray[1][2] + controlArray[1][4]) / 2);
            controlArray[1][1] = ((P[1][0] + P[1][1]) / 2);
            controlArray[1][2] = ((controlArray[1][1] + (P[1][1] + P[1][2]) / 2) / 2);
            controlArray[1][5] = ((P[1][2] + P[1][3]) / 2);
            controlArray[1][4] = ((controlArray[1][5] + (P[1][1] + P[1][2]) / 2) / 2);
            controlArray[1][3] = ((controlArray[1][2] + controlArray[1][4]) / 2);

            double a = 0.5;
            controlArray[1][1] = controlArray[0][1] + a * (controlArray[0][1] - controlArray[1][1]);
            controlArray[1][2] = controlArray[0][2] + a * (controlArray[0][2] - controlArray[1][2]);
            controlArray[1][5] = controlArray[0][5] + a * (controlArray[0][5] - controlArray[1][5]);
            controlArray[1][4] = controlArray[0][4] + a * (controlArray[0][4] - controlArray[1][4]);
            controlArray[1][3] = controlArray[0][3] + a * (controlArray[0][3] - controlArray[1][3]);

            return controlArray;
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
                        //k += 2;
                        //BezierArraysBasedOnGregory[0][i, j].Draw(transformacja, k, 1, 0, 0);
                        //BezierArraysBasedOnGregory[1][i, j].Draw(transformacja, k, 0, 1, 0);
                        //BezierArraysBasedOnGregory[2][i, j].Draw(transformacja, k, 0, 0, 1);

                    }


                }
            }

            //foreach (var item in ControlArrayC1)
            //{
            //    for (int i = 0; i < 7; i++)
            //    {
            //        item[i].Draw(transformacja, 12, 1, 1, 1);
            //    }
            //}

            foreach (var patch in PatchCollection)
            {
                patch.DrawPatch(transformacja);
            }


            //foreach (var point in EgdePoints)
            //{
            //    for (int i = 0; i < 4; i++)
            //    {
            //        point[0][i].Draw(transformacja, 10, 1, 1, 0);
            //        point[1][i].Draw(transformacja, 10, 1, 1, 1);
            //    }
            //}




            if (VectorsVisibility) DrawVectors(transformacja);

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

        public bool CheckStatus()
        {
            throw new NotImplementedException();
        }
    }
}
