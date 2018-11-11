using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using ModelowanieGeometryczne.Model;
using OpenTK;

namespace ModelowanieGeometryczne
{
    class FinishPathGenerator
    {
        private ObservableCollection<BezierPatchC2> BezierPatchC2Collection;
        private ObservableCollection<BezierPatch> BezierPatchCollection;
        private List<Tuple<Point, Vector3d>> List = new List<Tuple<Point, Vector3d>>();

        public FinishPathGenerator()
        {

        }
        public FinishPathGenerator(ObservableCollection<BezierPatchC2> _BezierPatchC2Collection, ObservableCollection<BezierPatch> _BezierPatchCollection)
        {
            BezierPatchC2Collection = _BezierPatchC2Collection;
            BezierPatchCollection = _BezierPatchCollection;
        }

        public void AddPointToList(List<Tuple<Point, Vector3d>> List, Tuple<Point, Vector3d> item, ref bool flag1, ref bool flag2, double safeHeight)
        {
            if (item.Item1.Z > 0)
            {
                if (flag2 == true)
                {
                    List.Add(new Tuple<Point, Vector3d>(
                        new Point(item.Item1.X, List.Last().Item1.Y, safeHeight),
                        item.Item2));
                    flag2 = false;
                }
                List.Add(item);
                flag1 = true;


            }

            else
            {
                if (flag1 == true)
                {
                    List.Add(new Tuple<Point, Vector3d>(
                        new Point(List.Last().Item1.X, List.Last().Item1.Y, safeHeight),
                        List.Last().Item2));
                }

                flag1 = false;
                flag2 = true;
            }
        }

        List<Point> ControlList = new List<Point>();
        public List<Point> Path = new List<Point>();
        public List<Point> GeneratePath()
        {
            ControlList.Clear();
               Path = new List<Point>();
            double safeHeight = 2.5;
            double r = 0.4;
            int divisions = 20;
            bool flag1 = false;
            bool flag2 = false;
            List.Clear();
            List<Tuple<Point, Vector3d>>[] temp = new List<Tuple<Point, Vector3d>>[5];
            temp[0] = BezierPatchC2Collection[0].GeneratePointsWithNormalVectorsForMilling(0, 1, 0, 1, 4 * divisions, 4 * divisions, -r);
            temp[1] = BezierPatchC2Collection[1].GeneratePointsWithNormalVectorsForMilling(0, 1, 0, 1, divisions, divisions, r);
            temp[2] = BezierPatchC2Collection[2].GeneratePointsWithNormalVectorsForMilling(0, 1, 0, 1, 3 * divisions, 3 * divisions, -r);
            temp[3] = BezierPatchC2Collection[3].GeneratePointsWithNormalVectorsForMilling(0, 1, 0, 1, 2 * divisions, 2 * divisions, -r);
            temp[4] = BezierPatchCollection[0].GeneratePointsWithNormalVectorsForMilling(0, 1, 0, 1, 2 * divisions, 2 * divisions, -r);

            for (int i = 0; i < temp.Length; i++)
            {
                Tuple<Point, Vector3d> PointWithOffset = new Tuple<Point, Vector3d>(temp[i][0].Item1 + temp[i][0].Item2, Vector3d.Zero);
                PointWithOffset.Item1.Z = safeHeight;
                List.Add(PointWithOffset);
                foreach (var item in temp[i])
                {
                    // if (item.Item1.Z > 0) { List.Add(item); }
                    AddPointToList(List, item, ref flag1, ref flag2, safeHeight);
                }
                PointWithOffset = new Tuple<Point, Vector3d>(List.Last().Item1 + List.Last().Item2, Vector3d.Zero);
                PointWithOffset.Item1.Z = safeHeight;
                List.Add(PointWithOffset);
            }



            //Tuple<Point, Vector3d> PointWithOffset = new Tuple<Point, Vector3d>(temp0[0].Item1 + temp0[0].Item2, Vector3d.Zero);
            //PointWithOffset.Item1.Z = safeHeight;
            //List.Add(PointWithOffset);
            //foreach (var item in temp0)
            //{
            //    // if (item.Item1.Z > 0) { List.Add(item); }
            //    AddPointToList(List, item, ref flag1, ref flag2, safeHeight);
            //}
            //PointWithOffset = new Tuple<Point, Vector3d>(List.Last().Item1 + List.Last().Item2, Vector3d.Zero);
            //PointWithOffset.Item1.Z = safeHeight;
            //List.Add(PointWithOffset);



            //PointWithOffset = new Tuple<Point, Vector3d>(temp1[0].Item1 + temp1[0].Item2, Vector3d.Zero);
            //PointWithOffset.Item1.Z = safeHeight;
            //List.Add(PointWithOffset);
            //foreach (var item in temp1)
            //{
            //    //if (item.Item1.Z > 0) { List.Add(item); }
            //    AddPointToList(List, item, ref flag1, ref flag2, safeHeight);
            //}
            //PointWithOffset = new Tuple<Point, Vector3d>(List.Last().Item1 + List.Last().Item2, Vector3d.Zero);
            //PointWithOffset.Item1.Z = safeHeight;
            //List.Add(PointWithOffset);


            //PointWithOffset = new Tuple<Point, Vector3d>(temp2[0].Item1 + temp2[0].Item2, Vector3d.Zero);
            //PointWithOffset.Item1.Z = safeHeight;
            //List.Add(PointWithOffset);
            //foreach (var item in temp2)
            //{
            //    //if (item.Item1.Z > 0) { List.Add(item); }
            //    AddPointToList(List, item, ref flag1, ref flag2, safeHeight);
            //}
            //PointWithOffset = new Tuple<Point, Vector3d>(List.Last().Item1 + List.Last().Item2, Vector3d.Zero);
            //PointWithOffset.Item1.Z = safeHeight;
            //List.Add(PointWithOffset);


            //PointWithOffset = new Tuple<Point, Vector3d>(temp3[0].Item1 + temp3[0].Item2, Vector3d.Zero);
            //PointWithOffset.Item1.Z = safeHeight;
            //List.Add(PointWithOffset);
            //foreach (var item in temp3)
            //{
            //    //    if (item.Item1.Z > 0)
            //    //    {
            //    //        if (flag2 == true)
            //    //        {
            //    //            List.Add(new Tuple<Point, Vector3d>(
            //    //                new Point(item.Item1.X, List.Last().Item1.Y, safeHeight),
            //    //                item.Item2));
            //    //            flag2 = false;
            //    //        }
            //    //        List.Add(item);
            //    //        flag1 = true;


            //    //    }

            //    //    else
            //    //    {
            //    //        if (flag1 == true)
            //    //        {
            //    //            List.Add(new Tuple<Point, Vector3d>(
            //    //                new Point(List.Last().Item1.X, List.Last().Item1.Y, safeHeight),
            //    //                List.Last().Item2));
            //    //        }

            //    //        flag1 = false;
            //    //        flag2 = true;
            //    AddPointToList(List, item, ref flag1, ref flag2, safeHeight);

            //}
            //PointWithOffset = new Tuple<Point, Vector3d>(List.Last().Item1 + List.Last().Item2, Vector3d.Zero);
            //PointWithOffset.Item1.Z = safeHeight;
            //List.Add(PointWithOffset);

            //List<Tuple<Point, Vector3d>> temp4 = BezierPatchCollection[0].GeneratePointsWithNormalVectorsForMilling(0, 1, 0, 1, 2 * divisions, 2 * divisions, -r);
            //PointWithOffset = new Tuple<Point, Vector3d>(temp4[0].Item1 + temp4[0].Item2, Vector3d.Zero);
            //PointWithOffset.Item1.Z = safeHeight;
            //List.Add(PointWithOffset);
            //foreach (var item in temp4)
            //{
            //    // if (item.Item1.Z > 0) { List.Add(item); }
            //    AddPointToList(List, item, ref flag1, ref flag2, safeHeight);
            //}
            //PointWithOffset = new Tuple<Point, Vector3d>(List.Last().Item1 + List.Last().Item2, Vector3d.Zero);
            //PointWithOffset.Item1.Z = safeHeight;
            //List.Add(PointWithOffset);


            for (int i = 0; i < BezierPatchC2Collection.Count-1; i++)
            {
                for (int j = 0; j <= 4; j++)
                {
                    ControlList.Add(BezierPatchC2Collection[i].GetPoint((double)j / 4.0, 0));
                    ControlList.Add(BezierPatchC2Collection[i].GetPoint((double)j / 4.0, 1));
                }
            }


            const double toleranceRadius = 1;
            foreach (var item in List)
            {
                Point PointWithOffset1 = item.Item1 + item.Item2;

                //if ((List.Where(a => (a.Item1 - PointWithOffset1).Length() < (r - 0.001)).Any()))
                //{
                //    PointWithOffset1.Z = safeHeight;
                //}


                if ((ControlList.Where(a => ((a.X - item.Item1.X)< toleranceRadius && (a.X - item.Item1.X) > -toleranceRadius) && ((a.Y - item.Item1.Y) < toleranceRadius && (a.Y - item.Item1.Y) > -toleranceRadius)).Any()))
                {
                    if ((List.Where(a => (a.Item1 - PointWithOffset1).Length() < (r - 0.001)).Any()))
                    {
                        PointWithOffset1.Z = safeHeight;
                    }
                }
                Path.Add(PointWithOffset1);
            }

            foreach (var item in Path)
            {
                item.Z -= r;
                if (item.Z < 0) item.Z = 0;
            }
            return Path;
        }

        private int UPPER = 1;
        public void Draw(Matrix4d M)
        {

            foreach (var item in ControlList)
            {
               item.Draw(M,20,1,1,1);
            }
            foreach (var item in List)
            {
                item.Item1.Draw(M, 10, 1, 0, 0);
                (item.Item1 + item.Item2).Draw(M, 10, 0, 1, 0);
            }

            foreach (var item in Path)
            {
                item.Draw(M, 10, 0, 0, 1);
            }
            //if (List.Count < 10) return;
            //for (int i = 0; i < UPPER; i++)
            //{
            //    List[i].Item1.Draw(M, 10, 1, 0, 0);
            //    (List[i].Item1 + List[i].Item2).Draw(M, 10, 0, 1, 0);

            //}
            //UPPER += 1;
        }
    }
}
