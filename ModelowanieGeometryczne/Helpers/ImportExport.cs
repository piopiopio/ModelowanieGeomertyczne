using ModelowanieGeometryczne.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using OpenTK;
using BezierCurve = ModelowanieGeometryczne.Model.BezierCurve;

namespace ModelowanieGeometryczne.Helpers
{
    public class PointExchange
    {
        public string name;
        public double x;
        public double y;
        public double z;

        public PointExchange(string _name, double _x, double _y, double _z)
        {
            name = _name;
            x = _x;
            y = _y;
            z = _z;
        }
        public PointExchange()
        { }
    }

    public class Surface
    {
        public bool cylinder;
        public int flakeU;
        public int flakeV;
        public string name;
        public int[][] points;
        public int u;
        public int v;

        public Surface()
        { }

        public Surface(
         bool cylinder1,
         int flakeU1,
         int flakeV1,
         string name1,
         int[][] points1,
         int u1,
         int v1)

        {
            cylinder = cylinder1;
            flakeU = flakeU1;
            flakeV = flakeV1;
            name = name1;
            points = points1;
            u = u1;
            v = v1;

        }
    }

    public class CurveExchange
    {
        // public bool isInterpolation;

        public string name;
        public int[] points;
        public int u;

        public CurveExchange()
        { }

        public CurveExchange(

         string name1,
         int[] points1
       )
        {
            name = name1;
            points = points1;

        }
    }

    public class TorusExchange
    {
        public double r1;
        public double R;
        public int u;
        public int v;
        public double[] center;
        public double[] rotation;
        public double[] scale;

        public TorusExchange()
        { }
        public TorusExchange(double r11, double R1, int u1, int v1, double[] center1, double[] rotation1, double[] scale1)
        {
            r1 = r11;
            R = R1;
            u = u1;
            v = v1;
            center = center1;
            rotation = rotation1;
            scale = scale1;
        }

    }
    public class AllCollections
    {
        public List<CurveExchange> curvesC0 = new List<CurveExchange>();
        public List<CurveExchange> curvesC2 = new List<CurveExchange>();
        public List<CurveExchange> curvesC2I = new List<CurveExchange>();
        public List<PointExchange> points = new List<PointExchange>();
        public List<Surface> surfacesC0 = new List<Surface>();
        public List<Surface> surfacesC2 = new List<Surface>();
        public List<TorusExchange> toruses = new List<TorusExchange>();
    }

    public class ImportExport
    {
        private ObservableCollection<Torus> TorusCollection1;
        ObservableCollection<BezierPatchC2> BezierPatchC2Collection1;
        ObservableCollection<Point> PointsCollection1;
        ObservableCollection<BezierPatch> BezierPatchCollection1;
        ObservableCollection<Curve> _bezierCurveCollection1;
        AllCollections result;


        public ImportExport(ObservableCollection<BezierPatchC2> BezierPatchC2Collection, ObservableCollection<BezierPatch> BezierPatchCollection, ObservableCollection<Point> PointsCollection, ObservableCollection<Curve> _bezierCurveCollection, ObservableCollection<Torus> TorusCollection)
        {
            TorusCollection1 = TorusCollection;
            BezierPatchC2Collection1 = BezierPatchC2Collection;
            PointsCollection1 = PointsCollection;
            BezierPatchCollection1 = BezierPatchCollection;
            _bezierCurveCollection1 = _bezierCurveCollection;
        }


        public void LoadJson(string path)
        {


            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();


                result = new JavaScriptSerializer().Deserialize<AllCollections>(json);

            }
            ParseLoadedObject();

        }


        public void SaveJson(string path)
        {

            var obj = PrepareToSave();

            var json = new JavaScriptSerializer().Serialize(obj);
            //TODO: Upięknianie JSON'a można to zakomentować w razie czego
            json = JSON_PrettyPrinter.Process(json);
            File.WriteAllText(path, json);

        }

        public AllCollections PrepareToSave()
        {
            result = new AllCollections();
            int PointsCounter = 0;


            foreach (var item in TorusCollection1)
            {
                TorusExchange temp = new TorusExchange(item.r, item.R, item._division_fi, item.Division_teta, item.Center, item.Center, item.Center);
                result.toruses.Add(temp);
            }

            foreach (var item in BezierPatchC2Collection1)
            {
                int[][] pointIndices = new int[item.VerticalPatches + 3][];
                for (int i = 0; i < item.PatchPoints.GetLength(0); i++)
                {
                    pointIndices[i] = new int[item.HorizontalPatches + 3];
                    for (int j = 0; j < item.PatchPoints.GetLength(1); j++)
                    {

                        result.points.Add(new PointExchange(item.PatchPoints[i, j].Name, item.PatchPoints[i, j].X, item.PatchPoints[i, j].Y, item.PatchPoints[i, j].Z));
                        pointIndices[i][j] = PointsCounter;
                        PointsCounter++;
                    }
                }

                Surface temp = new Surface(item.PatchesAreCylinder, item.HorizontalPatches, item.VerticalPatches, item.Name, pointIndices, item.u, item.v);
                result.surfacesC2.Add(temp);
            }

            List<Point> AllPoints = new List<Point>();

            foreach (var item in BezierPatchCollection1)
            {
                Point[,] PatchPoints;
                PatchPoints = item.GetAllPointsInOneArray();
                int[][] pointIndices = new int[PatchPoints.GetLength(0)][];



                for (int i = 0; i < PatchPoints.GetLength(0); i++)
                {
                    pointIndices[i] = new int[PatchPoints.GetLength(1)];

                    for (int j = 0; j < PatchPoints.GetLength(1); j++)
                    {
                        int a = AllPoints.IndexOf(PatchPoints[i, j]);

                        if (a == -1)
                        {
                            AllPoints.Add(PatchPoints[i, j]);
                            result.points.Add(new PointExchange(PatchPoints[i, j].Name, PatchPoints[i, j].X,
                            PatchPoints[i, j].Y, PatchPoints[i, j].Z));
                            pointIndices[i][j] = PointsCounter;
                            PointsCounter++;
                        }
                        else
                        {
                            pointIndices[i][j] = a;
                            //MessageBox.Show(a.ToString());
                        }
                    }
                }

                Surface temp = new Surface(item.PatchesAreCylinder, item.HorizontalPatches, item.VerticalPatches, item.Name, pointIndices, item._patchHorizontalDivision, item._patchVerticalDivision);
                result.surfacesC0.Add(temp);
            }

            foreach (var item in _bezierCurveCollection1)
            {
                int[] pointIndices = new int[item.PointsCollection.Count];

                for (int j = 0; j < item.PointsCollection.Count; j++)
                {
                    result.points.Add(new PointExchange(item.PointsCollection[j].Name, item.PointsCollection[j].X, item.PointsCollection[j].Y, item.PointsCollection[j].Z));
                    pointIndices[j] = PointsCounter;
                    PointsCounter++;
                }

                CurveExchange temp = new CurveExchange(item.Name, pointIndices);

                if (item.CurveType == "C0")
                {
                    result.curvesC0.Add(temp);
                }
                else if (item.CurveType == "C2")
                {
                    result.curvesC2.Add(temp);
                }
                else if (item.CurveType == "C2Interpolation")
                {
                    result.curvesC2I.Add(temp);
                }
                else
                {

                }


            }





            return result;
        }


        //double scale = 20;
        //Vector3d translate = new Vector3d(0, -75, 0);
        double scale = 1;
        Vector3d translate = new Vector3d(0, 0, 0);
        public void ParseLoadedObject()
        {

            List<Point> PointsCollection = new List<Point>();

            foreach (var item in result.toruses)
            {
                Vector4d temp = new Vector4d(item.center[0], item.center[1], item.center[2], 0);
                TorusCollection1.Add(new Torus(temp, item.r1, item.R, item.u, item.v));
            }

            foreach (var item in result.points)
            {
                PointsCollection.Add(new Point(item.x, item.y, item.z, item.name));
            }
            foreach (var item in result.surfacesC2)
            {
                Point[,] convertedPoints = new Point[item.points.GetLength(0), item.points[0].GetLength(0)];

                for (int i = 0; i < item.points.GetLength(0); i++)
                {
                    for (int j = 0; j < item.points[0].GetLength(0); j++)
                    {
                        convertedPoints[i, j] = new Point(translate.X+scale*result.points[(int)item.points[i][j]].x, translate.Y+scale*result.points[(int)item.points[i][j]].y, translate.Z+scale*result.points[(int)item.points[i][j]].z);
                  }
                }

                BezierPatchC2Collection1.Add(new BezierPatchC2(item.flakeU, item.flakeV, item.u, item.v, item.cylinder, convertedPoints, item.name));
            }



            foreach (var item in result.surfacesC0)
            {
                Point[,] convertedPoints = new Point[item.points.GetLength(0), item.points[0].GetLength(0)];

                for (int i = 0; i < item.points.GetLength(0); i++)
                {
                    for (int j = 0; j < item.points[0].GetLength(0); j++)
                    {
                        //convertedPoints[i, j] = new Point(result.points[(int)item.points[i][j]].x, result.points[(int)item.points[i][j]].y, result.points[(int)item.points[i][j]].z);
                        convertedPoints[i, j] =scale*PointsCollection[item.points[i][j]]+ translate;
                    }
                }

                BezierPatchCollection1.Add(new BezierPatch(item.flakeU, item.flakeV, item.u, item.v, item.cylinder, convertedPoints, item.name));
            }


            foreach (var item in result.curvesC0)
            {
                List<Point> convertedPoints = new List<Point>();

                for (int i = 0; i < item.points.GetLength(0); i++)
                {

                    convertedPoints.Add(new Point(result.points[(int)item.points[i]].x, result.points[(int)item.points[i]].y, result.points[(int)item.points[i]].z));
                    PointsCollection1.Add(convertedPoints.Last());


                }

                _bezierCurveCollection1.Add(new BezierCurve(convertedPoints));

            }

            foreach (var item in result.curvesC2)
            {
                List<Point> convertedPoints = new List<Point>();

                for (int i = 0; i < item.points.GetLength(0); i++)
                {

                    convertedPoints.Add(new Point(result.points[(int)item.points[i]].x, result.points[(int)item.points[i]].y, result.points[(int)item.points[i]].z));
                    PointsCollection1.Add(convertedPoints.Last());

                }

                _bezierCurveCollection1.Add(new BezierCurveC2(convertedPoints, false, item.name));

            }

            foreach (var item in result.curvesC2I)
            {
                List<Point> convertedPoints = new List<Point>();

                for (int i = 0; i < item.points.GetLength(0); i++)
                {

                    convertedPoints.Add(new Point(result.points[(int)item.points[i]].x, result.points[(int)item.points[i]].y, result.points[(int)item.points[i]].z));
                    PointsCollection1.Add(convertedPoints.Last());

                }

                _bezierCurveCollection1.Add(new BezierCurveC2(convertedPoints, true, item.name));

            }
        }
    }

}
