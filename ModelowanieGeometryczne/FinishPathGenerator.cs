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
        List<Point> hatchingList = new List<Point>();
        public List<Point> Path = new List<Point>();

        public List<Point> GenerateControlStar()
        {
            List<Point> ControlStarPoints=new List<Point>();
            //Point Origin=new Point(0,0,0);
            foreach (var item in List)
            {Point temp=new Point(item.Item1.X, item.Item1.Y,0);
                ControlStarPoints.Add(temp);
                ControlStarPoints.Add(item.Item1);
                ControlStarPoints.Add(temp);
            }

            return ControlStarPoints;

        }
        public List<Point> GeneratePath()
        {
            if (BezierPatchC2Collection.Count < 5 || BezierPatchCollection.Count < 1)
            {
                return null;
            }

            hatchingList = new List<Point>();
            Path = new List<Point>();
            double safeHeight = 2.5;
            double r = 0.4;
            int divisions = 20;
            bool flag1 = false;
            bool flag2 = false;
            List.Clear();

            List<Tuple<Point, Vector3d>>[] temp = new List<Tuple<Point, Vector3d>>[6];
            temp[0] = BezierPatchC2Collection[0].GeneratePointsWithNormalVectorsForMilling(0, 1, 0, 1, 4 * divisions, 4 * divisions, -r);
            temp[1] = BezierPatchC2Collection[1].GeneratePointsWithNormalVectorsForMilling(0, 1, 0, 1, divisions, divisions, r);
            temp[2] = BezierPatchC2Collection[2].GeneratePointsWithNormalVectorsForMilling(0, 1, 0, 1, 3 * divisions, 3 * divisions, -r);
            temp[3] = BezierPatchC2Collection[3].GeneratePointsWithNormalVectorsForMilling(0, 1, 0, 1, 2 * divisions, 2 * divisions, -r);
            temp[4] = BezierPatchCollection[0].GeneratePointsWithNormalVectorsForMilling(0.0, 0.24999, 0, 1, 2 * divisions, 2 * divisions, -r);
            temp[5] = BezierPatchCollection[0].GeneratePointsWithNormalVectorsForMilling(0.25001, 1, 0, 1, 2 * divisions, 2 * divisions, -r);

            for (int i = 0; i < temp.Length; i++)
            {
                Tuple<Point, Vector3d> PointWithOffset = new Tuple<Point, Vector3d>(temp[i][0].Item1 + temp[i][0].Item2, Vector3d.Zero);
                PointWithOffset.Item1.Z = safeHeight;
                List.Add(PointWithOffset);
                foreach (var item in temp[i])
                {
                    AddPointToList(List, item, ref flag1, ref flag2, safeHeight);
                }
                PointWithOffset = new Tuple<Point, Vector3d>(List.Last().Item1 + List.Last().Item2, Vector3d.Zero);
                PointWithOffset.Item1.Z = safeHeight;
                List.Add(PointWithOffset);
            }


            List<Point> ControlList = new List<Point>();
            for (int i = 0; i < BezierPatchC2Collection.Count - 1; i++)
            {
                for (int j = 0; j <= 4; j++)
                {
                    ControlList.Add(BezierPatchC2Collection[i].GetPoint((double)j / 4.0, 0));
                    ControlList.Add(BezierPatchC2Collection[i].GetPoint((double)j / 4.0, 1));
                }
            }



            const double toleranceRadius = 0.7;
            foreach (var item in List)
            {
                Point PointWithOffset1 = item.Item1 + item.Item2;



                if ((ControlList.Any(a => ((a.X - item.Item1.X) < toleranceRadius && (a.X - item.Item1.X) > -toleranceRadius) && ((a.Y - item.Item1.Y) < toleranceRadius && (a.Y - item.Item1.Y) > -toleranceRadius))))
                {
                    if ((List.Any(a => (a.Item1 - PointWithOffset1).Length() < (r - 0.001))))
                    {
                        PointWithOffset1.Z = safeHeight;
                    }
                }
                PointWithOffset1.Z -= r;
                if (PointWithOffset1.Z < 0) PointWithOffset1.Z = 0;
                Path.Add(PointWithOffset1);
            }



            var p1 = BezierPatchCollection[0].GetPoint(0.25, 0);
            p1.Z = safeHeight;
            Path.Add(p1);

            for (int i = 0; i < 41; i++)
            {
                p1 = BezierPatchCollection[0].GetPoint(0.25, (double)i / 40);

                Path.Add(p1);
            }

            p1 = BezierPatchCollection[0].GetPoint(0.25, 1);
            p1.Z = safeHeight;
            Path.Add(p1);





            List<Point> ListToAdd = new List<Point>();
            hatchingList = Path.Where(a => a.Z < 0.05).ToList();

            Point startPoint = new Point(-2.4, -0.9, 0);
            Point hatchingPointTemp = new Point(startPoint);


            double hatchingEpsilon = 0.08;
            double jump = 0.03;

            int n = 50;



            ListToAdd.Add(new Point(startPoint.X, startPoint.Y, safeHeight));
            for (int i = 0; i < (n + 1); i++)
            {
                ListToAdd.Add(startPoint);

                var aa = hatchingList.Where(a => (a - hatchingPointTemp).Length() < (hatchingEpsilon)).ToList();
                while (!(hatchingList.Where(a => (a - hatchingPointTemp).Length() < (hatchingEpsilon))).Any())
                {
                    hatchingPointTemp.X += Math.Sin((2 * Math.PI / n) * i) * jump;
                    hatchingPointTemp.Y += Math.Cos((2 * Math.PI / n) * i) * jump;

                }
                ListToAdd.Add(hatchingPointTemp);
                hatchingPointTemp = new Point(startPoint);
            }

            Path = Path.Concat(ListToAdd).ToList();
            Path.Add(new Point(Path.Last().X, Path.Last().Y, safeHeight));

            //////debug
            ////Path.Clear();
            ////foreach (var item in BezierPatchCollection[0].GeneratePointsWithNormalVectorsForMilling(0.250, 0.250, 0, 1, 1, 100, r))
            ////{
            ////    Path.Add(item.Item1);
            ////}

            ////foreach (var item in BezierPatchCollection[0].GeneratePointsWithNormalVectorsForMilling(0.250001, 0.25001, 0, 1, 1, 100, -r))
            ////{
            ////    Path.Add(item.Item1 + item.Item2);
            ////}

            ////foreach (var item in BezierPatchCollection[0].GeneratePointsWithNormalVectorsForMilling(0.24999, 0.24999, 0, 1, 1, 100, -r))
            ////{
            ////    Path.Add(item.Item1 + item.Item2);
            ////}
            return Path;
        }

        //private int UPPER = 1;
        public void Draw(Matrix4d M)
        {
            //foreach (var item in hatchingList)
            //{
            //    item.Draw(M,20,0,1,0);
            //}
            foreach (var item in List)
            {
                item.Item1.Draw(M, 10, 1, 0, 0);
                (item.Item1 + item.Item2).Draw(M, 10, 0, 1, 0);
            }

            foreach (var item in Path)
            {
                item.Draw(M, 10, 0, 0, 1);
            }
        }
    }
}
