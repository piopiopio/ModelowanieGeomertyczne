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
        public int HorizontalPatches { get; set; }
        public int VerticalPatches { get; set; }
        public double PatchWidth { get; set; }
        public double PatchHeight { get; set; }
        public int PatchHorizontalDivision { get; set; }
        public int PatchVerticalDivision { get; set; }
        public bool PatchesAreCylinder { get; set; }
        public Vector4d StartPoint { get; set; }
        public ObservableCollection<Point> Vertices;
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
            Vertices = new ObservableCollection<Point>();
            StartPoint = startPoint;
            HorizontalPatches = horizontalPatches;
            VerticalPatches = verticalPatches;
            PatchWidth = patchWidth;
            PatchHeight = patchHeight;
            PatchHorizontalDivision = patchHorizontalDivision;
            PatchVerticalDivision = patchVerticalDivision;
            PatchesAreCylinder = patchesAreCylinder;
            if (patchesAreCylinder)
            {
                SetUpVerticesCylinder();
            }
            else
            {
                SetUpVertices();
            }

            PatchNumber++;
            Name = "Bezier patch number " + PatchNumber + " type: C0";
        }


        #region Public Methods

        private void SetUpVertices()
        {
            double dx = PatchWidth / HorizontalPatches;
            double dy = PatchHeight / VerticalPatches;

            if (Vertices.Any())
            {
                Vertices.Clear();
            }
            for (int i = 0; i < (3 * VerticalPatches + 1); i++)
            {
                for (int j = 0; j < (3 * HorizontalPatches + 1); j++)
                {

                    var point = new Point(StartPoint.X + j * dx, StartPoint.Y + i * dy, StartPoint.Z);
                    Vertices.Add(point);
                }

            }
        }

        //    protected void DrawSinglePatch(Bitmap bmp, Graphics g, int patchIndex, int patchDivisions, Matrix3D matX, Matrix3D matY, Matrix3D matZ
        //, int divisions, bool isHorizontal)
        //    {
        //        double step = 1.0f / (patchDivisions - 1);
        //        double drawingStep = 1.0f / (divisions - 1);
        //        double u = patchIndex == 0 ? 0 : step;
        //        Vector4 pointX = null, pointY = null;

        //        for (int m = (patchIndex == 0 ? 0 : 1); m < patchDivisions; m++, u += step)
        //        {
        //            if (isHorizontal)
        //                pointY = u.GetBezierPoint();
        //            else
        //                pointX = u.GetBezierPoint();

        //            for (int n = 0; n < divisions; n++)
        //            {
        //                var v = n * drawingStep;
        //                if (isHorizontal)
        //                    pointX = v.GetBezierPoint();
        //                else
        //                    pointY = v.GetBezierPoint();

        //                var point = CalculatePatchPoint(matX, matY, matZ, pointX, pointY);
        //                SceneManager.DrawPoint(bmp, g, point, Thickness, Color);
        //            }
        //        }
        //    }

        public Matrix4d GetPatchMatrix(int i, int j)
        {
            int ho = 0;
            int ve = 0;
            Matrix4d PatchMatrix = new Matrix4d();
 
            return PatchMatrix;
        }

        private void SetUpVerticesCylinder()
        {
            double dx = PatchWidth / HorizontalPatches;
            double dy = PatchHeight / VerticalPatches;
            double alpha = (Math.PI * 2.0f) / (3 * HorizontalPatches + 1);
            if (Vertices.Any())
            {
                Vertices.Clear();
            }
            for (int i = 0; i < (3 * VerticalPatches + 1); i++)
            {
                for (int j = 0; j < (3 * HorizontalPatches + 1); j++)
                {
                    //patchwidth is radius when cylinder is set
                    var point = new Point(PatchWidth * Math.Cos(alpha * j), StartPoint.Y + (i * dy), PatchWidth * Math.Sin(alpha * j));
                    Vertices.Add(point);
                }

            }
        }




        public void Draw(Matrix4d transformacja)
        {
            foreach (var point in Vertices)
            {
                point.Draw(transformacja);
            }

            if (PolylineEnabled)
            {
                drawPolyline(transformacja);
            }
        }

        private void drawPolyline(Matrix4d transformacja)
        {//Przed wywłoaniem tej motody musi zostac wywołane rysowanie punktów aby przeliczyc window coordinates
            var projekcja = MatrixProvider.ProjectionMatrix();
            double dx = PatchWidth / HorizontalPatches;
            double dy = PatchHeight / VerticalPatches;
            Vector4d windowCoordinates;

            GL.Begin(BeginMode.Lines);
            GL.Color3(1.0, 1.0, 1.0);
            int j = 0;
            for (int i = 0; i < (3 * VerticalPatches + 1); i++)
            {


                for (j = 0; j < (3 * HorizontalPatches); j++)
                {//poziome
                    windowCoordinates = projekcja.Multiply(transformacja.Multiply(Vertices[j + i * (3 * VerticalPatches + 1)].Coordinates));
                    GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                    windowCoordinates = projekcja.Multiply(transformacja.Multiply(Vertices[j + 1 + i * (3 * VerticalPatches + 1)].Coordinates));
                    GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);



                }

                if (PatchesAreCylinder)
                {
                    windowCoordinates = projekcja.Multiply(transformacja.Multiply(Vertices[j + i * (3 * VerticalPatches + 1)].Coordinates));
                    GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                    windowCoordinates = projekcja.Multiply(transformacja.Multiply(Vertices[0 + i * (3 * VerticalPatches + 1)].Coordinates));
                    GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                }

            }

            for (int i = 0; i < (3 * VerticalPatches); i++)
            {
                for (j = 0; j < (3 * HorizontalPatches + 1); j++)
                {

                    windowCoordinates = projekcja.Multiply(transformacja.Multiply(Vertices[j + i * (3 * VerticalPatches + 1)].Coordinates));
                    GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                    windowCoordinates = projekcja.Multiply(transformacja.Multiply(Vertices[j + (i + 1) * (3 * VerticalPatches + 1)].Coordinates));
                    GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);

                }
            }
            GL.End();
        }
        #endregion Public Methods
    }
}
