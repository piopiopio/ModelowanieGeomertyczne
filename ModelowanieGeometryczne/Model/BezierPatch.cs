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
        private int _patchHorizontalDivision = 4;
        private int _patchVerticalDivision = 4;
        public bool PatchesAreCylinder { get; set; }
        public Vector4d StartPoint { get; set; }
        private ObservableCollection<Point> _vertices = new ObservableCollection<Point>();
        bool _polylineEnabled = true;
        public bool Selected { get; set; }
        public string Name { get; set; }
        static int PatchNumber { get; set; }

        public int PatchHorizontalDivision
        {
            get
            {
                return _patchHorizontalDivision;
            }
            set
            {
                if ((value >= 3) && (value<20))
                {
                    _patchHorizontalDivision = value;

                    if (Surface != null)
                    {
                        foreach (var item in Surface)
                        {
                            item.v = _patchHorizontalDivision;

                        }
                    }
                }
            }
        }
        public int PatchVerticalDivision
        {
            get
            {
                return _patchVerticalDivision;
            }
            set
            {
                if ((value >= 3) && (value < 20))
                {
                    _patchVerticalDivision = value;
                    if (Surface != null)
                    {
                        foreach (var item in Surface)
                        {
                            item.u = _patchVerticalDivision;
                        }
                    }
                }
            }
        }


        public bool PolylineEnabled
        {
            get { return _polylineEnabled; }
            set
            {
                _polylineEnabled = value;
                // OnPropertyChanged("PolylineEnabled");     
            }
        }
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


            for (int i = 0; i < VerticalPatches; i++)
            {//i pionowe płatki :
                for (int j = 0; j < HorizontalPatches; j++)
                {//j iteracja pozioma, poziome płatki ..
                    temp = new Point[4, 4];

                    LocalStartPoint = new Point(StartPoint.X + 3 * dx * j, StartPoint.Y + 3 * dy * i, StartPoint.Z);
                    for (int k = 0; k < 4; k++)
                    {
                        for (int l = 0; l < 4; l++)
                        {



                            //Wstawianie tych samych punktów na łączeniach
                            if ((i != 0) && (k == 0))
                            {
                                temp[k, l] = Surface[j, i - 1].PatchPoints[3, l];
                            }

                            else if ((j != 0) && (l == 0))
                            {
                                temp[k, l] = Surface[j - 1, i].PatchPoints[k, 3];
                            }
                            else
                            {
                                temp[k, l] = new Point(LocalStartPoint.X + l * dx, LocalStartPoint.Y + k * dy, LocalStartPoint.Z);
                            }
                        }
                    }

                    Surface[j, i] = new Patch(temp, _patchVerticalDivision, _patchHorizontalDivision);

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
            //double alpha = (Math.PI * 2.0f) / (3 * HorizontalPatches + 1);
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



            double alpha = (Math.PI * 2.0f) / (3 * HorizontalPatches);
            double dx = PatchWidth / (3 * HorizontalPatches);

            double dy = PatchHeight / (3 * VerticalPatches);



            var temp = new Point[4, 4];


            for (int i = 0; i < VerticalPatches; i++)
            {//i pionowe płatki :
                for (int j = 0; j < HorizontalPatches; j++)
                {//j iteracja pozioma, poziome płatki ..
                    temp = new Point[4, 4];




                    for (int k = 0; k < 4; k++)
                    {
                        for (int l = 0; l < 4; l++)
                        {



                            //Wstawianie tych samych punktów na łączeniach
                            if ((i != 0) && (k == 0))
                            {
                                temp[k, l] = Surface[j, i - 1].PatchPoints[3, l];
                            }

                            else if ((j != 0) && (l == 0))
                            {
                                temp[k, l] = Surface[j - 1, i].PatchPoints[k, 3];
                            }
                            else
                            {
                                temp[k, l] = new Point(StartPoint.X + PatchWidth * Math.Cos(alpha * (3 * j + l)), StartPoint.Y + (k + i * 3) * dy, StartPoint.Z + PatchWidth * Math.Sin(alpha * (3 * j + l)));
                                // temp[k, l] = new Point(LocalStartPoint.X + l * dx, LocalStartPoint.Y + k * dy, LocalStartPoint.Z);
                            }


                        }
                    }

                    Surface[j, i] = new Patch(temp, _patchVerticalDivision, _patchHorizontalDivision);

                }
            }
        }



        public void DrawPolyline(Matrix4d transformacja)
        {
            if (_polylineEnabled)
            {
                for (int i = 0; i < HorizontalPatches; i++)
                {
                    for (int j = 0; j < VerticalPatches; j++)
                    {
                        Surface[i, j].DrawPolyline(transformacja);
                    }
                }
            }
        }

        public void DrawSurface(Matrix4d transformacja)
        {

            for (int i = 0; i < HorizontalPatches; i++)
            {
                for (int j = 0; j < VerticalPatches; j++)
                {
                    Surface[i, j].DrawPoints(transformacja);
                    Surface[i, j].DrawPatch(transformacja);
                }
            }
        }

        #endregion Public Methods
    }
}
