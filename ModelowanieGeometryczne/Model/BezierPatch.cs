using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ModelowanieGeometryczne.Helpers;

namespace ModelowanieGeometryczne.Model
{
    public class BezierPatch
    {

        public Patch[,] Surface;

        public int HorizontalPatches { get; set; }
        public int VerticalPatches { get; set; }
        public double PatchWidth { get; set; }
        public double PatchHeight { get; set; }
        public int PatchHorizontalDivision { get; set; }
        public int PatchVerticalDivision { get; set; }
        public bool PatchesAreCylinder { get; set; }
        public Vector4d StartPoint { get; set; }
        private ObservableCollection<Point> _vertices = new ObservableCollection<Point>();
        public ObservableCollection<Point> Vertices
        {
            get
            {
                _vertices.Clear();
                foreach (var item in Surface)
                {
                    foreach (var item2 in item.PatchPoints)
                    {
                        _vertices.Add(item2);
                    }
                }
                return _vertices;
            }

        }
        public bool Selected { get; set; }
        public string Name { get; set; }
        static int PatchNumber { get; set; }
        private bool _polylineEnabled = false;
        public bool PolylineEnabled
        {
            get
            {
                return _polylineEnabled;
            }
            set
            {
                _polylineEnabled = value;
            }
        }

        public BezierPatch(
            int horizontalPatches,
             int verticalPatches,
             double patchWidth,
             double patchHeight,
             int patchHorizontalDivision,
             int patchVerticalDivision,
             bool patchesAreCylinder,
             Vector4d startPoint)
        {



            // Vertices = new ObservableCollection<Point>();
            StartPoint = startPoint;
            HorizontalPatches = horizontalPatches;
            VerticalPatches = verticalPatches;
            PatchWidth = patchWidth;
            PatchHeight = patchHeight;
            PatchHorizontalDivision = patchHorizontalDivision;
            PatchVerticalDivision = patchVerticalDivision;
            PatchesAreCylinder = patchesAreCylinder;


            Surface = new Patch[HorizontalPatches, VerticalPatches];




            if (patchesAreCylinder)
            {
                SetUpVerticesCylinder();
            }

            else
            {
                SetUpPatchVertices();
            }

            PatchNumber++;
            Name = "Bezier patch number " + PatchNumber + " type: C0";


        }


        #region Public Methods

        private void SetUpPatchVertices()
        {
            double dx = PatchWidth / (3 * HorizontalPatches);
            double dy = PatchHeight / (3 * VerticalPatches);
            Point LocalStartPoint;
            var temp = new Point[4, 4];
            Patch[][] a = new Patch[3][];

            for (int i = 0; i < 3; i++)
            {
                a[i] = new Patch[3];
            }

            for (int i = 0; i < VerticalPatches; i++)
            {
                for (int j = 0; j < HorizontalPatches; j++)
                {
                    temp = new Point[4, 4];

                    LocalStartPoint = new Point(StartPoint.X + 3 * dx * j, StartPoint.Y + 3 * dy * i, StartPoint.Z);
                    for (int k = 0; k < 4; k++)
                    {
                        for (int l = 0; l < 4; l++)
                        {
                            temp[k, l] = new Point(LocalStartPoint.X + l * dx, LocalStartPoint.Y + k * dy, LocalStartPoint.Z);
                        }
                    }

                    Surface[j, i] = new Patch(temp);

                }
            }
        }



        public Matrix4d GetPatchMatrix(int i, int j)
        {
            int ho = 0;
            int ve = 0;
            Matrix4d PatchMatrix = new Matrix4d();

            return PatchMatrix;
        }

        private void SetUpVerticesCylinder()
        {
            //double dx = PatchWidth / HorizontalPatches;
            //double dy = PatchHeight / VerticalPatches;
            double alpha = (Math.PI * 2.0f) / (3 * HorizontalPatches + 1);
            //if (Vertices.Any())
            //{
            //    Vertices.Clear();
            //}
            //for (int i = 0; i < (3 * VerticalPatches + 1); i++)
            //{
            //    for (int j = 0; j < (3 * HorizontalPatches + 1); j++)
            //    {
            //        //patchwidth is radius when cylinder is set
            //        var point = new Point(PatchWidth * Math.Cos(alpha * j), StartPoint.Y + (i * dy), PatchWidth * Math.Sin(alpha * j));
            //        Vertices.Add(point);
            //    }
            //}

            double dx = PatchWidth / (3 * HorizontalPatches);
            double dy = PatchHeight / (3 * VerticalPatches);
            Point LocalStartPoint;
            var temp = new Point[4, 4];
            Patch[][] a = new Patch[3][];

            for (int i = 0; i < 3; i++)
            {
                a[i] = new Patch[3];
            }

            for (int i = 0; i < VerticalPatches; i++)
            {
                for (int j = 0; j < HorizontalPatches; j++)
                {
                    temp = new Point[4, 4];


                    LocalStartPoint = new Point(StartPoint.X + 3 * dx * j, StartPoint.Y + 3 * dy * i, StartPoint.Z);
                    for (int k = 0; k < 4; k++)
                    {
                        for (int l = 0; l < 4; l++)
                        {
                            temp[k, l] = new Point(LocalStartPoint.X + l * dx, LocalStartPoint.Y + k * dy, LocalStartPoint.Z);
                        }
                    }

                    Surface[j, i] = new Patch(temp);

                }
            }

        }




        public void Draw(Matrix4d transformacja)
        {

            for (int i = 0; i < HorizontalPatches; i++)
            {
                for (int j = 0; j < VerticalPatches; j++)
                {
                    Surface[i, j].DrawPoints(transformacja);
                    Surface[i, j].DrawPatch(transformacja);
                }
            }


            //foreach (var point in Vertices)
            //{
            //    point.Draw(transformacja);
            //}

            //if (PolylineEnabled)
            //{
            //    drawPolyline(transformacja);
            //}
        }

        private void drawPolyline(Matrix4d transformacja)
        {//Przed wywłoaniem tej motody musi zostac wywołane rysowanie punktów aby przeliczyc window coordinates
            //var projekcja = MatrixProvider.ProjectionMatrix();
            //double dx = PatchWidth / HorizontalPatches;
            //double dy = PatchHeight / VerticalPatches;
            //Vector4d windowCoordinates;

            //GL.Begin(BeginMode.Lines);
            //GL.Color3(1.0, 1.0, 1.0);
            //int j = 0;
            //for (int i = 0; i < (3 * VerticalPatches + 1); i++)
            //{


            //    for (j = 0; j < (3 * HorizontalPatches); j++)
            //    {//poziome
            //        windowCoordinates = projekcja.Multiply(transformacja.Multiply(Vertices[j + i * (3 * VerticalPatches + 1)].Coordinates));
            //        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
            //        windowCoordinates = projekcja.Multiply(transformacja.Multiply(Vertices[j + 1 + i * (3 * VerticalPatches + 1)].Coordinates));
            //        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);



            //    }

            //    if (PatchesAreCylinder)
            //    {
            //        windowCoordinates = projekcja.Multiply(transformacja.Multiply(Vertices[j + i * (3 * VerticalPatches + 1)].Coordinates));
            //        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
            //        windowCoordinates = projekcja.Multiply(transformacja.Multiply(Vertices[0 + i * (3 * VerticalPatches + 1)].Coordinates));
            //        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
            //    }

            //}

            //for (int i = 0; i < (3 * VerticalPatches); i++)
            //{
            //    for (j = 0; j < (3 * HorizontalPatches + 1); j++)
            //    {

            //        windowCoordinates = projekcja.Multiply(transformacja.Multiply(Vertices[j + i * (3 * VerticalPatches + 1)].Coordinates));
            //        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
            //        windowCoordinates = projekcja.Multiply(transformacja.Multiply(Vertices[j + (i + 1) * (3 * VerticalPatches + 1)].Coordinates));
            //        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);

            //    }
            //}
            //GL.End();
        }
        #endregion Public Methods
    }
}
