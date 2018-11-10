using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModelowanieGeometryczne.Helpers;
using System.Windows.Forms;
using NativeWindow = OpenTK.NativeWindow;

namespace ModelowanieGeometryczne.Model
{
    public class BezierPatch : IPatch
    {

        public Patch[,] Surface;

        public int HorizontalPatches { get; set; }
        public int VerticalPatches { get; set; }
        public double PatchWidth { get; set; }
        public double PatchHeight { get; set; }
        public int _patchHorizontalDivision = 4;
        public int _patchVerticalDivision = 4;
        public bool PatchesAreCylinder { get; set; }
        public Vector4d StartPoint { get; set; }
        private ObservableCollection<Point> _vertices = new ObservableCollection<Point>();
        bool _polylineEnabled = false;
        private bool _selected = false;

        public bool Selected
        {
            get
            {
                return _selected;

            }
            set
            {
                _selected = value;
                foreach (var VARIABLE in Surface)
                {
                    foreach (var item in VARIABLE.PatchPoints)
                    {
                        item.Selected = _selected;
                    }

                   
                }
            }
        }
        public string Name { get; set; }
        static int PatchNumber { get; set; }
        Point[,] AllPointsArray;

        public double[] GetPatchNumber(double u, double v)
        {
            double borderX = 1.0 / HorizontalPatches;
            double borderY = 1.0 / VerticalPatches;


            double[] surfaceCoordinates = new double[4];

            surfaceCoordinates[0] = (int)(u / borderX);
            surfaceCoordinates[1] = (int)(v / borderY);

            if ((int)surfaceCoordinates[0] == HorizontalPatches) surfaceCoordinates[0] = surfaceCoordinates[0] - 1;
            if ((int)surfaceCoordinates[1] == VerticalPatches) surfaceCoordinates[1] = surfaceCoordinates[1] - 1;

            surfaceCoordinates[2] = (u - borderX * surfaceCoordinates[0]) / borderX;
            surfaceCoordinates[3] = (v - borderY * surfaceCoordinates[1]) / borderY;

            return surfaceCoordinates;
        }

        public Point GetPoint(double u, double v)
        {
            double[] coord = GetPatchNumber(u, v);

            // return Surface[(int)coord[0], (int)coord[1]].GetPoint(coord[3], coord[2]);
            //return Surface[(int)coord[1], (int)coord[0]].GetPoint(coord[2], coord[3]);
            return Surface[(int)coord[0], (int)coord[1]].GetPoint(coord[2], coord[3]);
        }

        public Point GetPointDerivativeU(double u, double v)
        {

            double[] coord = GetPatchNumber(u, v);

            // return Surface[(int)coord[0], (int)coord[1]].GetPointDerrivativeU(coord[3], coord[2]);
           // return Surface[(int)coord[1], (int)coord[0]].GetPointDerrivativeU(coord[2], coord[3]);
            return Surface[(int)coord[0], (int)coord[1]].GetPointDerrivativeU(coord[2], coord[3]);
        }

        public Point GetPointDerivativeV(double u, double v)
        {

            double[] coord = GetPatchNumber(u, v);

            //return Surface[(int)coord[0], (int)coord[1]].GetPointDerrivativeV(coord[3], coord[2]);
            return Surface[(int)coord[0], (int)coord[1]].GetPointDerrivativeV(coord[2], coord[3]);
        }




        public Point[] GetMiddlePointBeetweenTwoPoints(List<Point> p)
        { //MiddlePoint[0] punkt na środku krawędzi
            //MiddlePoint[1] punkt wyznaczony z warunku C1
            if (HorizontalPatches > 1 || VerticalPatches > 1)
            {
                MessageBox.Show("Too big surface");
                return null;
            }


            AllPointsArray = GetAllPointsInOneArray();

            Point[] MiddlePoint = new Point[2];
            int CaseNumber = 0;
            List<Tuple<int, int>> pIndices = new List<Tuple<int, int>>();



            for (int i = 0; i < AllPointsArray.GetLength(0); i++)
            {
                for (int j = 0; j < AllPointsArray.GetLength(1); j++)
                {
                    foreach (var item in p)
                    {
                        if (AllPointsArray[i, j] == item)
                        {
                            pIndices.Add(new Tuple<int, int>(i, j));
                            break;
                        }
                    }
                }
            }

            if (pIndices.Count > 2)
            {
                MessageBox.Show("Too many points selected");
                return null;
            }
            if (pIndices.Count < 2)
            {
                //MessageBox.Show("Too less points selected");
                return null;
            }

            if ((pIndices[0].Item1 == 0 && pIndices[0].Item2 == 0) && (pIndices[1].Item1 == 0 && pIndices[1].Item2 == 3)) CaseNumber = 1;
            if ((pIndices[1].Item1 == 0 && pIndices[1].Item2 == 0) && (pIndices[0].Item1 == 0 && pIndices[0].Item2 == 3)) CaseNumber = 1;

            if ((pIndices[0].Item1 == 0 && pIndices[0].Item2 == 3) && (pIndices[1].Item1 == 3 && pIndices[1].Item2 == 3)) CaseNumber = 2;
            if ((pIndices[1].Item1 == 0 && pIndices[1].Item2 == 3) && (pIndices[0].Item1 == 3 && pIndices[0].Item2 == 3)) CaseNumber = 2;

            if ((pIndices[0].Item1 == 3 && pIndices[0].Item2 == 0) && (pIndices[1].Item1 == 3 && pIndices[1].Item2 == 3)) CaseNumber = 3;
            if ((pIndices[1].Item1 == 3 && pIndices[1].Item2 == 0) && (pIndices[0].Item1 == 3 && pIndices[0].Item2 == 3)) CaseNumber = 3;

            if ((pIndices[0].Item1 == 0 && pIndices[0].Item2 == 0) && (pIndices[1].Item1 == 3 && pIndices[1].Item2 == 0)) CaseNumber = 4;
            if ((pIndices[1].Item1 == 0 && pIndices[1].Item2 == 0) && (pIndices[0].Item1 == 3 && pIndices[0].Item2 == 0)) CaseNumber = 4;

            switch (CaseNumber)
            {
                case 1:
                    MiddlePoint[0] = Surface[0, 0].GetPointGregory(0, 0.5);
                    MiddlePoint[1] = Surface[0, 0].GetPointGregory(0, 0.5).Add(Surface[0, 0].GetPointGregory(0, 0.5).Subtract(Surface[0, 0].GetPointGregory(0.1, 0.5)));
                    break;
                case 2:
                    MiddlePoint[0] = Surface[0, 0].GetPointGregory(0.5, 1);
                    MiddlePoint[1] = Surface[0, 0].GetPointGregory(0.5, 1).Add(Surface[0, 0].GetPointGregory(0.5, 1).Subtract(Surface[0, 0].GetPointGregory(0.5, 0.9)));
                    break;
                case 3:
                    MiddlePoint[0] = Surface[0, 0].GetPointGregory(1, 0.5);
                    MiddlePoint[1] = Surface[0, 0].GetPointGregory(1, 0.5).Add(Surface[0, 0].GetPointGregory(1, 0.5).Subtract(Surface[0, 0].GetPointGregory(0.9, 0.5)));
                    break;
                case 4:
                    MiddlePoint[0] = Surface[0, 0].GetPointGregory(0.5, 0);
                    MiddlePoint[1] = Surface[0, 0].GetPointGregory(0.5, 0).Add(Surface[0, 0].GetPointGregory(0.5, 0).Subtract(Surface[0, 0].GetPointGregory(0.5, 0.1)));
                    break;
                default:
                    MiddlePoint[0] = null;
                    MiddlePoint[1] = null;
                    break;
            }

            //if(MiddlePoint!= null) MessageBox.Show("X: " + MiddlePoint.X.ToString()+ " Y: " + MiddlePoint.Y.ToString()+ " Z: "+ MiddlePoint.Z.ToString());

            return MiddlePoint;
        }

        public Point[][] GetFourEdgeControlPoints(List<Point> p)
        {
            if (HorizontalPatches > 1 || VerticalPatches > 1)
            {
                MessageBox.Show("Too big surface");
                return null;
            }


            AllPointsArray = GetAllPointsInOneArray();

            Point[][] egdePoints = new Point[2][];
            egdePoints[0] = new Point[4];
            egdePoints[1] = new Point[4];

            int CaseNumber = 0;
            List<Tuple<int, int>> pIndices = new List<Tuple<int, int>>();



            for (int i = 0; i < AllPointsArray.GetLength(0); i++)
            {
                for (int j = 0; j < AllPointsArray.GetLength(1); j++)
                {
                    if (AllPointsArray[i, j] == p[0])
                    {
                        pIndices.Add(new Tuple<int, int>(i, j));
                        break;
                    }

                }
            }
            for (int i = 0; i < AllPointsArray.GetLength(0); i++)
            {
                for (int j = 0; j < AllPointsArray.GetLength(1); j++)
                {
                    if (AllPointsArray[i, j] == p[1])
                    {
                        pIndices.Add(new Tuple<int, int>(i, j));
                        break;
                    }

                }
            }
            if (pIndices.Count > 2)
            {
                MessageBox.Show("Too many points selected");
                return null;
            }

            if (pIndices.Count < 2)
            {
                // MessageBox.Show("Too less points selected");
                return null;
            }

            egdePoints[0][0] = p[0];
            egdePoints[0][1] = AllPointsArray[pIndices[0].Item1 + (pIndices[1].Item1 - pIndices[0].Item1) / 3, pIndices[0].Item2 + (pIndices[1].Item2 - pIndices[0].Item2) / 3];
            egdePoints[0][2] = AllPointsArray[pIndices[0].Item1 + 2 * (pIndices[1].Item1 - pIndices[0].Item1) / 3, pIndices[0].Item2 + 2 * (pIndices[1].Item2 - pIndices[0].Item2) / 3];
            egdePoints[0][3] = p[1];

            int a = 99, b = 99;
            if (pIndices[0].Item1 == pIndices[1].Item1 && pIndices[0].Item1 == 0)
            {
                a = 1;
                b = 0;
            }
            if (pIndices[0].Item1 == pIndices[1].Item1 && pIndices[0].Item1 == 3)
            {
                a = -1;
                b = 0;
            }
            if (pIndices[0].Item2 == pIndices[1].Item2 && pIndices[0].Item2 == 0)
            {
                a = 0;
                b = 1;
            }
            if (pIndices[0].Item2 == pIndices[1].Item2 && pIndices[0].Item2 == 3)
            {
                a = 0;
                b = -1;
            }
            egdePoints[1][0] = AllPointsArray[a + pIndices[0].Item1, b + pIndices[0].Item2];
            egdePoints[1][1] = AllPointsArray[a + pIndices[0].Item1 + (pIndices[1].Item1 - pIndices[0].Item1) / 3, b + pIndices[0].Item2 + (pIndices[1].Item2 - pIndices[0].Item2) / 3];
            egdePoints[1][2] = AllPointsArray[a + pIndices[0].Item1 + 2 * (pIndices[1].Item1 - pIndices[0].Item1) / 3, b + pIndices[0].Item2 + 2 * (pIndices[1].Item2 - pIndices[0].Item2) / 3];
            egdePoints[1][3] = AllPointsArray[a + pIndices[0].Item1 + (pIndices[1].Item1 - pIndices[0].Item1), b + pIndices[0].Item2 + (pIndices[1].Item2 - pIndices[0].Item2)];
            return egdePoints;
        }

        public Point[,] GetFivePointsBeetweenTwoPoints(List<Point> p)
        {
            //MiddlePoint[0] punkt na środku krawędzi
            //MiddlePoint[1] punkt wyznaczony z warunku C1
            if (HorizontalPatches > 1 || VerticalPatches > 1)
            {
                MessageBox.Show("Too big surface");
                return null;
            }


            AllPointsArray = GetAllPointsInOneArray();

            Point[,] MiddlePoints = new Point[2, 5];
            int CaseNumber = 0;
            List<Tuple<int, int>> pIndices = new List<Tuple<int, int>>();



            for (int i = 0; i < AllPointsArray.GetLength(0); i++)
            {
                for (int j = 0; j < AllPointsArray.GetLength(1); j++)
                {
                    if (AllPointsArray[i, j] == p[0])
                    {
                        pIndices.Add(new Tuple<int, int>(i, j));
                        break;
                    }

                }
            }
            for (int i = 0; i < AllPointsArray.GetLength(0); i++)
            {
                for (int j = 0; j < AllPointsArray.GetLength(1); j++)
                {
                    if (AllPointsArray[i, j] == p[1])
                    {
                        pIndices.Add(new Tuple<int, int>(i, j));
                        break;
                    }

                }
            }
            if (pIndices.Count > 2)
            {
                MessageBox.Show("Too many points selected");
                return null;
            }

            if (pIndices.Count < 2)
            {
                // MessageBox.Show("Too less points selected");
                return null;
            }

            //if ((pIndices[0].Item1 == 0 && pIndices[0].Item2 == 0) && (pIndices[1].Item1 == 0 && pIndices[1].Item2 == 3)) CaseNumber = 1;
            //if ((pIndices[1].Item1 == 0 && pIndices[1].Item2 == 0) && (pIndices[0].Item1 == 0 && pIndices[0].Item2 == 3)) CaseNumber = 1;

            //if ((pIndices[0].Item1 == 0 && pIndices[0].Item2 == 3) && (pIndices[1].Item1 == 3 && pIndices[1].Item2 == 3)) CaseNumber = 2;
            //if ((pIndices[1].Item1 == 0 && pIndices[1].Item2 == 3) && (pIndices[0].Item1 == 3 && pIndices[0].Item2 == 3)) CaseNumber = 2;

            //if ((pIndices[0].Item1 == 3 && pIndices[0].Item2 == 0) && (pIndices[1].Item1 == 3 && pIndices[1].Item2 == 3)) CaseNumber = 3;
            //if ((pIndices[1].Item1 == 3 && pIndices[1].Item2 == 0) && (pIndices[0].Item1 == 3 && pIndices[0].Item2 == 3)) CaseNumber = 3;

            //if ((pIndices[0].Item1 == 0 && pIndices[0].Item2 == 0) && (pIndices[1].Item1 == 3 && pIndices[1].Item2 == 0)) CaseNumber = 4;
            //if ((pIndices[1].Item1 == 0 && pIndices[1].Item2 == 0) && (pIndices[0].Item1 == 3 && pIndices[0].Item2 == 0)) CaseNumber = 4;

            //double delta = 1.0 / 6.0;
            //for (int i = 1; i < 6; i++)
            //{
            //    switch (CaseNumber)
            //    {
            //        case 1:
            //            MiddlePoints[0, i-1] = Surface[0, 0].GetPoint(0, delta*i);
            //            MiddlePoints[1, i-1] = Surface[0, 0].GetPoint(0, delta * i).Add(Surface[0, 0].GetPoint(0, delta * i).Subtract(Surface[0, 0].GetPoint(0.1, delta * i)));
            //            break;
            //        case 2:
            //            MiddlePoints[0, i-1] = Surface[0, 0].GetPoint(delta * i, 1);
            //            MiddlePoints[1, i-1] = Surface[0, 0].GetPoint(delta * i, 1).Add(Surface[0, 0].GetPoint(delta * i, 1).Subtract(Surface[0, 0].GetPoint(delta * i, 0.9)));
            //            break;
            //        case 3:
            //            MiddlePoints[0, i-1] = Surface[0, 0].GetPoint(1, delta * i);
            //            MiddlePoints[1, i-1] = Surface[0, 0].GetPoint(1, delta * i).Add(Surface[0, 0].GetPoint(1, delta * i).Subtract(Surface[0, 0].GetPoint(0.9, delta * i)));
            //            break;
            //        case 4:
            //            MiddlePoints[0, i-1] = Surface[0, 0].GetPoint(delta * i, 0);
            //            MiddlePoints[1, i-1] = Surface[0, 0].GetPoint(delta * i, 0).Add(Surface[0, 0].GetPoint(delta * i, 0).Subtract(Surface[0, 0].GetPoint(delta * i, 0.1)));
            //            break;
            //        default:
            //            MiddlePoints[0, i-1] = null;
            //            MiddlePoints[1, i-1] = null;
            //            break;
            //    }
            //}
            //if(MiddlePoint!= null) MessageBox.Show("X: " + MiddlePoint.X.ToString()+ " Y: " + MiddlePoint.Y.ToString()+ " Z: "+ MiddlePoint.Z.ToString());

            double a = pIndices[0].Item1 / 18.0;
            double b = pIndices[1].Item1 / 18.0;
            double c = pIndices[0].Item2 / 18.0;
            double d = pIndices[1].Item2 / 18.0;
            double e, f;
            double g, h;
            if (((b - a) < 0) || (pIndices[0].Item1 == 3 && pIndices[1].Item1 == 3)) e = 1;
            else e = 0;

            if (((d - c) < 0) || (pIndices[0].Item2 == 3 && pIndices[1].Item2 == 3)) f = 1;
            else f = 0;

            if (pIndices[0].Item1 == 0 && pIndices[1].Item1 == 0)
            {
                g = 0.1;
                h = 0;
            }
            else if (pIndices[0].Item1 == 3 && pIndices[1].Item1 == 3)
            {
                g = -0.1;
                h = 0;
            }

            else if (pIndices[0].Item2 == 0 && pIndices[1].Item2 == 0)
            {
                g = 0;
                h = 0.1;
            }
            else if (pIndices[0].Item2 == 3 && pIndices[1].Item2 == 3)
            {
                g = 0;
                h = -0.1;
            }
            else
            {
                //Bład 
                h = 99;
                g = 99;
            }
            for (int i = 1; i < 6; i++)
            {
                MiddlePoints[0, i - 1] = Surface[0, 0].GetPointGregory(e + (b - a) * i, f + (d - c) * i);
                //            MiddlePoints[1, i-1] = Surface[0, 0].GetPoint(0, delta * i).Add(Surface[0, 0].GetPoint(0, delta * i).Subtract(Surface[0, 0].GetPoint(0.1, delta * i)));
                MiddlePoints[1, i - 1] = 2 * Surface[0, 0].GetPointGregory(e + (b - a) * i, f + (d - c) * i) - Surface[0, 0].GetPointGregory(g + e + (b - a) * i, h + f + (d - c) * i); ;
            }

            return MiddlePoints;
        }






        public Point[,] GetAllPointsInOneArray()
        {
            Point[,] PointsArray = new Point[VerticalPatches * 3 + 1, HorizontalPatches * 3 + 1];



            for (int i = 0; i < VerticalPatches; i++)
            {
                for (int j = 0; j < HorizontalPatches; j++)
                {


                    for (int ip = 0; ip < 4; ip++)
                    {
                        for (int jp = 0; jp < 4; jp++)
                        {
                            PointsArray[ip + i * 3, jp + j * 3] = Surface[j, i].PatchPoints[ip, jp];
                        }

                    }


                }

            }
            return PointsArray;
        }

        public void PlacePoints(Point[,] PointsArray = null)
        {
            if (PointsArray == null)
            {
                PointsArray = GetAllPointsInOneArray();
            }
            Point[,] temp;
            if (PatchesAreCylinder)
            {
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
                                    temp[k, l] = PointsArray[3 * i + k, 3 * j + l];
                                    // temp[k, l] = new Point(LocalStartPoint.X + l * dx, LocalStartPoint.Y + k * dy, LocalStartPoint.Z);
                                }


                            }
                        }

                        Surface[j, i] = new Patch(temp, _patchVerticalDivision, _patchHorizontalDivision);

                    }
                }
            }
            else
            {

            }
        }


        public int PatchHorizontalDivision
        {
            get
            {
                return _patchHorizontalDivision;
            }
            set
            {
                if ((value >= 1) && (value <= 20))
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

        double _rotationZ = 0;
        double _rotationZOld = 0;
        public double RotationZ
        {
            get { return _rotationZ; }
            set
            {
                _rotationZ = value;
                ModelowanieGeometryczne.Model.Point[,] PointsTemporaryCollection = GetAllPointsInOneArray();
                for (int i = 0; i < PointsTemporaryCollection.GetLength(0); i++)
                {
                    for (int j = 0; j < PointsTemporaryCollection.GetLength(1); j++)
                    {
                        PointsTemporaryCollection[i, j] = MatrixProvider.MultiplyP(MatrixProvider.RotateZMatrix((Math.PI / 180) * (_rotationZ - _rotationZOld)), PointsTemporaryCollection[i, j]);
                    }
                }


                PatchPoints = PointsTemporaryCollection;
                PlaceVerticesToPatches4x4();
                RecalculatePatches();
                _rotationZOld = RotationZ;
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
                if ((value >= 1) && (value <= 20))
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

        public Point[,] _patchPoints;
        public Point[,] PatchPoints
        {
            get
            {
                //_patchPoints = GetAllPointsInOneArray();
                return _patchPoints;
            }
            set
            {
                _patchPoints = value;
                PlaceVerticesToPatches4x4();
                RecalculatePatches();
            }
        }


        public void RecalculatePatches()
        {
            foreach (var item in Surface)
            {
                item.CalculateParametrizationVectors();
                item.CalculateCurvesPatchPoints();
            }

        }

        public BezierPatch(
            int horizontalPatches,
            int verticalPatches,
            int patchHorizontalDivision,
            int patchVerticalDivision,
            bool cylinder,
            Point[,] pointsToAdd,
             string name1
            )
        {
            HorizontalPatches = horizontalPatches;
            VerticalPatches = verticalPatches;
            _patchHorizontalDivision = patchHorizontalDivision;
            _patchVerticalDivision = patchVerticalDivision;
            PatchesAreCylinder = cylinder;
            _patchPoints = pointsToAdd;
            Name = name1;
            Surface = new Patch[HorizontalPatches, VerticalPatches];
            AllPointsArray = pointsToAdd;

            PlaceVerticesToPatches4x4();
            // CalculateParametrizationVectors();
            //CalculateBezierPoints();
        }

        public void PlaceVerticesToPatches4x4()
        {//TODO: ujednolic placeverticestoPatches i placepoints-> tu jest połączone wersja teraz



            if (PatchesAreCylinder)
            {
                var PointsArray = PatchPoints;
                Point[,] temp;
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
                                    temp[k, l] = PointsArray[3 * i + k, 3 * j + l];
                                    // temp[k, l] = new Point(LocalStartPoint.X + l * dx, LocalStartPoint.Y + k * dy, LocalStartPoint.Z);
                                }


                                //Sklejanie końców
                                if ((j == (HorizontalPatches - 1)) && (l == 3) && (HorizontalPatches > 1))
                                {
                                    temp[k, l] = Surface[0, i].PatchPoints[k, 0];
                                }
                                if ((j == (HorizontalPatches - 1)) && (l == 3) && (HorizontalPatches == 1))
                                {
                                    temp[k, l] = temp[k, 0];
                                }

                            }
                        }

                        Surface[j, i] = new Patch(temp, _patchVerticalDivision, _patchHorizontalDivision);

                    }
                }
            }
            else
            {

                var temp = new Point[4, 4];
                int k = 0;
                int h = 0;
                for (int PatchesI = 0; PatchesI < VerticalPatches; PatchesI++)
                {
                    h = 0;
                    for (int PatchesJ = 0; PatchesJ < HorizontalPatches; PatchesJ++)
                    {
                        temp = new Point[4, 4];
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {

                                if ((PatchesI != 0) && (i == 0))
                                {

                                    temp[i, j] = PatchPoints[i + 4 * PatchesI - k, j + 4 * PatchesJ - h];
                                }

                                else if ((PatchesJ != 0) && (j == 0))
                                {

                                    temp[i, j] = PatchPoints[i + 4 * PatchesI - k, j + 4 * PatchesJ - h];
                                }
                                else
                                {
                                    temp[i, j] = PatchPoints[i + 4 * PatchesI - k, j + 4 * PatchesJ - h];
                                }

                            }

                        }
                        h++;

                        Surface[PatchesJ, PatchesI] = new Patch(temp, _patchVerticalDivision, _patchHorizontalDivision);
                    }
                    k++;
                }

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
            AllPointsArray = new Point[HorizontalPatches * 3 + 1, VerticalPatches * 3 + 1];



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


                            if ((j == (HorizontalPatches - 1)) && (l == 3) && (HorizontalPatches > 1))
                            {
                                temp[k, l] = Surface[0, i].PatchPoints[k, 0];
                            }
                            if ((j == (HorizontalPatches - 1)) && (l == 3) && (HorizontalPatches == 1))
                            {
                                temp[k, l] = temp[k, 0];
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

        List <Point> list = new List<Point>();
        public List<Point> GeneratePointsForMilling()
        {

            for (int i = 0; i < HorizontalPatches; i++)
            {
                for (int j = 0; j < VerticalPatches; j++)
                {
                    Surface[i, j].GeneratePointsForMilling(list);


                }
            }

            return list;
        }

        private bool changeDirection = false;
        private int insertIndex = 0;
        public List<Tuple<Point, Vector3d>> GeneratePointsWithNormalVectorsForMilling(double umin, double umax, double vmin, double vmax, int nu, int nv, double radius)
        {
            double deltaU = (umax - umin) / nu;
            double deltaV = (vmax - vmin) / nv;
            double tempU = umin;
            double tempV = vmin;
            List<Tuple<Point, Vector3d>> List = new List<Tuple<Point, Vector3d>>();
            for (int j = 0; j <= nu; j++)
            {

                tempV = vmin;
                insertIndex = List.Count;

                for (int i = 0; i <= nv; i++)
                {
                    Point a = GetPoint(tempU, tempV);
                    Vector3d b = Point.CrossProduct(GetPointDerivativeU(tempU, tempV),
                        GetPointDerivativeV(tempU, tempV));

                    b.Normalize();
                    b = b * radius;
                    if (changeDirection)
                    {
                        List.Insert(insertIndex, new Tuple<Point, Vector3d>(a, b));
                    }
                    else
                    {
                        List.Add(new Tuple<Point, Vector3d>(a, b));
                    }

                    tempV += deltaV;
                }

                changeDirection = !changeDirection;
                tempU += deltaV;
            }

            return List;

        }


        #endregion Public Methods
    }
}
