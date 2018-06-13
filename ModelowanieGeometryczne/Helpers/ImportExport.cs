using ModelowanieGeometryczne.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

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


    public class AllCollections
    {
        public List<int> curvesC0 = new List<int>();
        public List<int> curvesC2 = new List<int>();
        public List<int> curvesC2I = new List<int>();
        public List<PointExchange> points = new List<PointExchange>();
        public List<Surface> surfacesC0 = new List<Surface>();
        public List<Surface> surfacesC2 = new List<Surface>();
        public List<Surface> toruses = new List<Surface>();
    }

    public class ImportExport
    {
        ObservableCollection<BezierPatchC2> BezierPatchC2Collection1;
        ObservableCollection<Point> PointsCollection1;
        ObservableCollection<BezierPatch> BezierPatchCollection1;
        ObservableCollection<Curve> _bezierCurveCollection1;
        AllCollections result;


        public ImportExport(ObservableCollection<BezierPatchC2> BezierPatchC2Collection, ObservableCollection<BezierPatch> BezierPatchCollection, ObservableCollection<Point> PointsCollection, ObservableCollection<Curve> _bezierCurveCollection)
        {
            BezierPatchC2Collection1 = BezierPatchC2Collection;
            PointsCollection1 = PointsCollection;
            BezierPatchCollection1 = BezierPatchCollection;
            _bezierCurveCollection1 = _bezierCurveCollection;
        }

        public void SaveJson(string path)
        {

            var obj = PrepareToSave();

            var json = new JavaScriptSerializer().Serialize(obj);
            File.WriteAllText(path, json);

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

        public AllCollections PrepareToSave()
        {
            result = new AllCollections();
            int PointsCounter=0;
           
            foreach (var item in BezierPatchC2Collection1)
            {
                int[][] pointIndices = new int[item.VerticalPatches+3][];
                for (int i = 0; i < item.PatchPoints.GetLength(0); i++)
                {
                    pointIndices[i] = new int[item.HorizontalPatches + 3];
                    for (int j = 0; j < item.PatchPoints.GetLength(1); j++)
                    {
                        result.points.Add(new PointExchange(item.PatchPoints[i, j].Name, item.PatchPoints[i, j].X, item.PatchPoints[i, j].Y, item.PatchPoints[i, j].Z) );
                        pointIndices[i][j] = PointsCounter; 
                        PointsCounter++;
                    }
                }

                Surface temp = new Surface(item.PatchesAreCylinder, item.HorizontalPatches, item.VerticalPatches, item.name, pointIndices, item.u, item.v);
                result.surfacesC2.Add(temp);
            }


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
                        result.points.Add(new PointExchange(PatchPoints[i, j].Name, PatchPoints[i, j].X, PatchPoints[i, j].Y, PatchPoints[i, j].Z));
                        pointIndices[i][j] = PointsCounter;
                        PointsCounter++;
                    }
                }

                Surface temp = new Surface(item.PatchesAreCylinder, item.HorizontalPatches, item.VerticalPatches, item.Name, pointIndices, item._patchHorizontalDivision, item._patchVerticalDivision);
                result.surfacesC0.Add(temp);
            }



                return result;
        }

        public void ParseLoadedObject()
        {
            foreach (var item in result.surfacesC2)
            {
                Point[,] convertedPoints = new Point[item.points.GetLength(0), item.points[0].GetLength(0)];

                for (int i = 0; i < item.points.GetLength(0); i++)
                {
                    for (int j = 0; j < item.points[0].GetLength(0); j++)
                    {
                        convertedPoints[i, j] = new Point(result.points[(int)item.points[i][j]].x, result.points[(int)item.points[i][j]].y, result.points[(int)item.points[i][j]].z);
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
                        convertedPoints[i, j] = new Point(result.points[(int)item.points[i][j]].x, result.points[(int)item.points[i][j]].y, result.points[(int)item.points[i][j]].z);
                    }
                }

                BezierPatchCollection1.Add(new BezierPatch(item.flakeU, item.flakeV, item.u, item.v, item.cylinder, convertedPoints, item.name));
            }

        }
    }

}
