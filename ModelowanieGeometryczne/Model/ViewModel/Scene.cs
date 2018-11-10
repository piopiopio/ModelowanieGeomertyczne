using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Navigation;
using ModelowanieGeometryczne.Helpers;
using ModelowanieGeometryczne.Model;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Cursor = ModelowanieGeometryczne.Model.Cursor;
using BezierCurve = ModelowanieGeometryczne.Model.BezierCurve;
using Boolean = System.Boolean;
using Point = ModelowanieGeometryczne.Model.Point;

namespace ModelowanieGeometryczne.ViewModel
{
    public class Scene : ViewModelBase
    {

        double torus_r = 1;
        double torus_R = 2;
        int torus_division_fi = 20;
        int torus_division_teta = 20;

        public double Torus_r
        {
            get { return torus_r; }
            set
            {
                torus_r = value;
                OnPropertyChanged("Torus_r");
            }
        }


        public double Torus_R
        {
            get { return torus_R; }
            set
            {
                torus_R = value;
                OnPropertyChanged("Torus_R");
            }
        }

        public int Torus_division_fi
        {
            get { return torus_division_fi; }
            set
            {
                torus_division_fi = value;
                OnPropertyChanged("Torus_division_fi ");
            }
        }


        public int Torus_division_teta
        {
            get { return torus_division_teta; }
            set
            {
                torus_division_teta = value;
                OnPropertyChanged("Torus_division_teta ");
            }
        }

        public event PropertyChangedEventHandler RefreshScene;

        #region Private Fields

        public int EllipseCounter = 1;


        private Cursor _cursor;
        // private Torus _torus;
        private double _height = 750;
        private double _width = 1440;
        private double _x, _y, _x0, _y0, _alphaX, _alphaY, _alphaZ, _fi, _teta, _fi0, _teta0;
        private double _scale;
        private Matrix4d _m;
        private Matrix4d M
        {
            get
            { return _m; }
            set
            {
                _m = value;

            }

        }


        private Matrix4 _projection;
        private bool _stereoscopy;
        Tuple<int, int> _mouseCoordinates;
        private ObservableCollection<Point> _pointsCollection;//
        private ObservableCollection<Curve> _bezierCurveCollection;//
        private ObservableCollection<BezierPatch> _bezierPatchCollection;
        private ObservableCollection<BezierPatchC2> _bezierPatchC2Collection;
        private ObservableCollection<GregoryPatch> _gregoryPatchCollection;
        private ObservableCollection<TrimCurve> _trimCurvesCollection;
        private ObservableCollection<Torus> _torusCollection;
        public Ellipse YellowEllipse = new Ellipse();

        private bool _moveSelectedPointsWithCoursor = false;
        private bool _torusEnabled = false;
        private ICommand _gregoryMergePoints;
        private ICommand _clearScene;
        private ICommand _addBezierPatch;
        private ICommand _addGregoryPatch;
        private ICommand _addBezierPatchC2;
        private ICommand _trimPatches;
        private ICommand _addTorus;
        private ICommand _convertToInterpolation;
        private ICommand _addBezierCurve;
        private ICommand _addBezierCurveC2;
        private ICommand _addBezierCurveC2Interpolation;
        private ICommand _addPoints;
        private ICommand _undoAllTransformation;
        private ICommand _showCurvesUV;
        private ICommand _unselectAllItems;
        private ICommand _generateZigZagPath;
        private ICommand _initialPath;
        private ICommand _generateContourPath;
        private ICommand _generateFinishPath;




        private bool _showDescentGradientsSteps = false;
        public bool ShowDescentGradientsSteps
        {
            get { return _showDescentGradientsSteps; }
            set
            {
                _showDescentGradientsSteps = value;
                Refresh();
            }
        }
        private bool _showTrimmingCurves = false;
        public bool ShowTrimmingCurves
        {
            get { return _showTrimmingCurves; }
            set
            {
                _showTrimmingCurves = value;
                Refresh();
            }
        }

        private ICommand _drawEllipse;
        //Bezier patches
        int _horizontalPatches;
        int _verticalPatches;
        double _patchWidth;
        double _patchHeight;
        int _patchHorizontalDivision;
        int _patchVerticalDivision;
        bool _patchesAreCylinder;
        #endregion Private Fields

        #region Public Properties
        private Thread _workerThread;
        private double _ellipseA;
        private double _ellipseB;
        private double _ellipseC;
        private double _ellipseM;

        public double EllipseA
        {
            get { return _ellipseA; }
            set
            {
                _ellipseA = value;
                Refresh();
                OnPropertyChanged("EllipseA");
                EllipseCounter = 1;

            }
        }
        public double EllipseB
        {
            get { return _ellipseB; }
            set
            {
                _ellipseB = value;
                Refresh();
                OnPropertyChanged("EllipseB");
                EllipseCounter = 1;
            }
        }
        public double EllipseC
        {
            get { return _ellipseC; }
            set
            {
                _ellipseC = value;
                Refresh();
                OnPropertyChanged("EllipseC");
                EllipseCounter = 1;
            }
        }
        public double EllipseM
        {
            get { return _ellipseM; }
            set
            {
                _ellipseM = value;
                Refresh();
                OnPropertyChanged("EllipseM");
                EllipseCounter = 1;
            }
        }


        public ICommand AddPointsCommand { get { return _addPoints ?? (_addPoints = new ActionCommand(AddSelectedPointsExecuted)); } }

        public ICommand UndoAllTransformation { get { return _undoAllTransformation ?? (_undoAllTransformation = new ActionCommand(UndoAllTransformationExecuted)); } }

        public ICommand AddBezierPatch { get { return _addBezierPatch ?? (_addBezierPatch = new ActionCommand(AddBezierPatchExecuted)); } }

        public ICommand AddGregoryPatch { get { return _addGregoryPatch ?? (_addBezierPatch = new ActionCommand(AddGregoryPatchExecuted)); } }
        public ICommand AddBezierPatchC2 { get { return _addBezierPatchC2 ?? (_addBezierPatchC2 = new ActionCommand(AddBezierPatchC2Executed)); } }

        public ICommand TrimPatches { get { return _trimPatches ?? (_trimPatches = new ActionCommand(TrimPatchesExecuted)); } }
        public ICommand AddTorus { get { return _addTorus ?? (_addTorus = new ActionCommand(AddTorusExecuted)); } }
        public ICommand ConvertToInterpolation { get { return _convertToInterpolation ?? (_convertToInterpolation = new ActionCommand(ConvertToInterpolationExecuted)); } }
        public ICommand ShowCurvesUV { get { return _showCurvesUV ?? (_showCurvesUV = new ActionCommand(ShowCurvesUvExecuted)); } }
        public ICommand UnselectAllItems { get { return _unselectAllItems ?? (_unselectAllItems = new ActionCommand(UnselectAllItemsExecuted)); } }
        public ICommand InitialPath { get { return _initialPath ?? (_initialPath = new ActionCommand(InitialPathExecuted)); } }
        public ICommand GenerateZigZagPath { get { return _generateZigZagPath ?? (_generateZigZagPath = new ActionCommand(GenerateZigZagPathExecuted)); } }
        public ICommand GenerateContourPath { get { return _generateContourPath ?? (_generateContourPath = new ActionCommand(GenerateContourPathExecuted)); } }
        public ICommand GenerateFinishPath { get { return _generateFinishPath ?? (_generateFinishPath = new ActionCommand(GenerateFinishPathExecuted)); } }

        public List<Point> TrimedCurvesPointsListWithOffset = new List<Point>();
        HeightArray _heightArray = new HeightArray();
        private FinishPathGenerator _finishPathGenerator=new FinishPathGenerator();
        private void GenerateFinishPathExecuted()
        {//Sprawdzac czy ustawiona, wyłączyć możliwość klikniecia w przycisk przed wykonaniem poprzednih krokow.

            _finishPathGenerator=new FinishPathGenerator(BezierPatchC2Collection, BezierPatchCollection);
            SavePath(_finishPathGenerator.GeneratePath(), "4.k8");
        }


        private void InitialPathExecuted()
        {
            _heightArray = new HeightArray(BezierPatchC2Collection, BezierPatchCollection);
            List<Point> movesList=new List<Point>();
            _heightArray.GenerateInitialPathStep(movesList);
            _heightArray.GenerateInitialPathStep2(movesList);
            SavePath(movesList, "1.k16");
        }


        private void GenerateContourPathExecuted()
        {
            double r = 0.5; //Zmiana tego paramatru wymusi koniecznosc ręcznegoo usunięcia innych dwóch punktów niż te usuwane w tej chwili!! Default: r=0.5;
            Point a;
            Vector3d b = new Vector3d(0, 0, 0);
            List<Vector3d> OffsetVectorsHistory = new List<Vector3d>();
            TrimedCurvesPointsListWithOffset.Clear();
            Point tempPoint;
            for (int i = 0; i < TrimedCurvesPointsList.Count; i++)
            {
                bool addFlag = true;
                if (i == TrimedCurvesPointsList.Count - 1)
                {
                    a = TrimedCurvesPointsList[0] - TrimedCurvesPointsList[i];

                }
                else
                {
                    a = TrimedCurvesPointsList[i + 1] - TrimedCurvesPointsList[i];
                }

                if (a.X != 0)
                {
                    b.Y = Math.Sign(a.X) * Math.Sqrt(r * r * a.X * a.X / (a.X * a.X + a.Y * a.Y));
                    b.X = -a.Y * b.Y / a.X;
                }
                else
                {
                    a.X = 0;
                    b.X = -Math.Sign(a.Y);
                }

                b.Normalize();
                OffsetVectorsHistory.Add(b);
                tempPoint = TrimedCurvesPointsList[i] + (b * r);
                double epsilon = 0.0001;
                foreach (var item in TrimedCurvesPointsList)
                {
                    if ((item.X - tempPoint.X) * (item.X - tempPoint.X) +
                        (item.Y - tempPoint.Y) * (item.Y - tempPoint.Y) <= (r - epsilon) * (r - epsilon))
                    {
                        addFlag = false;
                        break;
                    }

                    if (!addFlag) break;
                }

                if (addFlag) TrimedCurvesPointsListWithOffset.Add(tempPoint);
            }

            double epsilonSpace = 0.2;
            //TrimedCurvesPointsListWithOffset.RemoveAt(2298);
           // TrimedCurvesPointsListWithOffset.RemoveAt(3138);
            //TrimedCurvesPointsListWithOffset.RemoveAt(3324);
            //TrimedCurvesPointsListWithOffset.RemoveAt(2483);
            List<Tuple<Point, Point, Point, Point>> freeSpacePoints = new List<Tuple<Point, Point, Point, Point>>();

            if ((TrimedCurvesPointsListWithOffset[1] - TrimedCurvesPointsListWithOffset[0]).Length() > epsilonSpace)
            {
                freeSpacePoints.Add(new Tuple<Point, Point, Point, Point>(TrimedCurvesPointsListWithOffset[TrimedCurvesPointsListWithOffset.Count - 1], TrimedCurvesPointsListWithOffset[0], TrimedCurvesPointsListWithOffset[1], TrimedCurvesPointsListWithOffset[2]));
            };


            for (int i = 1; i < TrimedCurvesPointsListWithOffset.Count - 2; i++)
            {
                if ((TrimedCurvesPointsListWithOffset[i + 1] - TrimedCurvesPointsListWithOffset[i]).Length() > epsilonSpace)
                {
                    freeSpacePoints.Add(new Tuple<Point, Point, Point, Point>(TrimedCurvesPointsListWithOffset[i - 1], TrimedCurvesPointsListWithOffset[i], TrimedCurvesPointsListWithOffset[i + 1], TrimedCurvesPointsListWithOffset[i + 2]));
                };
            }



            if ((TrimedCurvesPointsListWithOffset[TrimedCurvesPointsListWithOffset.Count - 1] - TrimedCurvesPointsListWithOffset[TrimedCurvesPointsListWithOffset.Count - 2]).Length() > epsilonSpace)
            {
                freeSpacePoints.Add(new Tuple<Point, Point, Point, Point>(TrimedCurvesPointsListWithOffset[TrimedCurvesPointsListWithOffset.Count - 3], TrimedCurvesPointsListWithOffset[TrimedCurvesPointsListWithOffset.Count - 2], TrimedCurvesPointsListWithOffset[TrimedCurvesPointsListWithOffset.Count - 1], TrimedCurvesPointsListWithOffset[0]));
            };

            if ((TrimedCurvesPointsListWithOffset[0] - TrimedCurvesPointsListWithOffset[TrimedCurvesPointsListWithOffset.Count - 1]).Length() > epsilonSpace)
            {
                freeSpacePoints.Add(new Tuple<Point, Point, Point, Point>(TrimedCurvesPointsListWithOffset[TrimedCurvesPointsListWithOffset.Count - 2], TrimedCurvesPointsListWithOffset[TrimedCurvesPointsListWithOffset.Count - 1], TrimedCurvesPointsListWithOffset[0], TrimedCurvesPointsListWithOffset[1]));
            };






            foreach (var item in freeSpacePoints)
            {
                Point a1 = item.Item2 - item.Item1;
                Point a2 = item.Item3 - item.Item4;

                double b1 = item.Item2.Y - item.Item2.X * a1.Y / a1.X;
                double b2 = item.Item3.Y - item.Item3.X * a2.Y / a2.X;

                double X = (b2 - b1) / ((a1.Y / a1.X) - (a2.Y / a2.X));
                double Y = (a1.Y / a1.X) * X + b1;

                TrimedCurvesPointsListWithOffset.Insert(TrimedCurvesPointsListWithOffset.IndexOf(item.Item3), new Point(X, Y, 0));

            }

            foreach (var item in TrimedCurvesPointsListWithOffset)
            {
                item.Z = 0;
            }
            TrimedCurvesPointsListWithOffset.Insert(0, new Point(TrimedCurvesPointsListWithOffset[0].X, TrimedCurvesPointsListWithOffset[0].Y - 4, 10));
            TrimedCurvesPointsListWithOffset.Insert(1, new Point(TrimedCurvesPointsListWithOffset[0].X, TrimedCurvesPointsListWithOffset[0].Y, 0));
            TrimedCurvesPointsListWithOffset.Add(new Point(TrimedCurvesPointsListWithOffset.Last().X, TrimedCurvesPointsListWithOffset.Last().Y, 10));


            SavePath(TrimedCurvesPointsListWithOffset, "3.f10");
        }


        public void ConnectTwoPoints(Point p1, Point p2, List<Point> list)
        {
            Point delta = (p2 - p1);
            int n = (int)Math.Floor(Math.Abs(delta.Length() / NewtonForwardStep));
            delta = delta / n;


            for (int i = 1; i < n; i++)
            {

                list.Add(p1 + i * delta);

            }
        }

        public void AppendTrimmedCurvesPoints(List<Point[]> t, List<Point> l)
        {

            foreach (var item in t)
            {
                l.Add(item[0]);
            }
        }

        public List<Point> TrimedCurvesPointsList = new List<Point>();

        public List<Point> MaxValues = new List<Point>();
        public List<Point> MinValues = new List<Point>();

        public List<Point> MaxValues1 = new List<Point>();
        public List<Point> MinValues1 = new List<Point>();

        public void SavePath(List<Point> pointsList, string fileName)
        {
            double scale = 10;
            double z_offset = 20;
            int LineNumberIterator = 1;
            SaveFileDialog sa = new SaveFileDialog();
            sa.Title = "Save";
            //sa.Filter = "Save users|*.f10; *.f10";
            //ExchangeObject.PrepareToSave();
            StringBuilder myStringBuilder = new StringBuilder();

            myStringBuilder.Append("N0" + "G01Z120.000\n");
            //   if (sa.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                for (int i = 0; i < pointsList.Count; i++)
                {
                    myStringBuilder.Append(pointsList[i].ToString(LineNumberIterator, scale, 20));
                    LineNumberIterator++;
                }

                //   File.WriteAllText(sa.FileName, myStringBuilder.ToString());
                File.WriteAllText("D:\\Studia\\Informatyka MGR\\Semestr 1\\ModelowanieGeometryczne\\MG testowe\\New folder\\" + fileName, myStringBuilder.ToString());
            }

        }

        public void GnerateMinMaxXLists(List<Point> list)
        {

            double jump = 0.05;
            double min = -7.5;
            double current = min;
            double max = 7.5;
            double rawDiameter = 1;
            double diameterOffset = 0.2;
            double radius = (rawDiameter + diameterOffset) / 2;
            MaxValues = new List<Point>();
            MinValues = new List<Point>();
            MaxValues1 = new List<Point>();
            MinValues1 = new List<Point>();

            for (int i = 0; i < (max - min) / jump; i++)
            {
                var temp = list.Where(o => (o.Y >= current && o.Y < (current + jump))).ToList();

                double maxValue = min;
                double minValue = max;
                foreach (var item in temp)
                {
                    maxValue = Math.Max(maxValue, item.X);
                }
                MaxValues.Add(new Point(maxValue, current, 0));

                foreach (var item in temp)
                {
                    minValue = Math.Min(minValue, item.X);
                }

                MinValues.Add(new Point(minValue, current, 0));
                current += jump;
            }

            for (int i = 0; i < MaxValues.Count; i++)
            {

                for (int j = 0; j < list.Count; j++)
                {
                    while ((list[j].X - MaxValues[i].X) * (list[j].X - MaxValues[i].X) +
                           (list[j].Y - MaxValues[i].Y) * (list[j].Y - MaxValues[i].Y) <= radius * radius)
                    {
                        MaxValues[i].X += jump / 30;
                        j = 0;


                    }

                }
                if (MaxValues[i].X > 0.5 && MaxValues[i].X < 1.5 && MaxValues[i].Y > 2.8 && MaxValues[i].Y < (3.0 + radius))
                {
                    //Kasowanie nadmiarowych punktów kolizja po y przy smigle
                    MinValues1.Insert(0, MaxValues[i]);
                    MaxValues.RemoveAt(i);
                    i -= 1;
                    //     break;
                }
            }


            for (int i = 0; i < MinValues.Count; i++)
            {

                for (int j = 0; j < list.Count; j++)
                {
                    while ((list[j].X - MinValues[i].X) * (list[j].X - MinValues[i].X) +
                       (list[j].Y - MinValues[i].Y) * (list[j].Y - MinValues[i].Y) <= radius * radius)
                    {
                        MinValues[i].X -= jump / 30;
                        j = 0;
                    }
                }

            }

            current = MinValues1.Last().Y - jump;
            for (int i = 0; i < (max - min) / jump; i++)
            {//x=1.5

                var temp = list.Where(o => (o.Y >= current && o.Y < (current + jump))).ToList();

                double maxValue = max;
                double minValue = min;
                foreach (var item in temp)
                {
                    if (item.X > 1.5)
                    {
                        maxValue = Math.Min(maxValue, item.X);
                    }
                }

                bool addFlag = true;
                foreach (var item in list)
                {
                    addFlag = true;
                    while ((item.X - maxValue) * (item.X - maxValue) +
                           (item.Y - current) * (item.Y - current) <= radius * radius)
                    {
                        maxValue -= jump / 30;


                    }

                    if (maxValue < 0.9)
                    {
                        addFlag = false;
                        // break;
                    }
                }

                foreach (var item in list)
                {
                    addFlag = true;
                    while ((item.X - maxValue) * (item.X - maxValue) +
                           (item.Y - current) * (item.Y - current) <= radius * radius)
                    {
                        maxValue -= jump / 30;


                    }

                    if (maxValue < 0.9)
                    {
                        addFlag = false;
                        // break;
                    }
                }


                if (addFlag)
                {
                    MaxValues1.Add(new Point(maxValue, current, 0));
                }








                foreach (var item in temp)
                {
                    if (item.X < 1.5)
                    {
                        minValue = Math.Max(minValue, item.X);
                    }

                }

                addFlag = true;
                foreach (var item in list)
                {

                    while ((item.X - minValue) * (item.X - minValue) +
                           (item.Y - current) * (item.Y - current) <= radius * radius)
                    {
                        minValue += jump / 30;


                    }
                    if (minValue > 1.4 || minValue < 0)
                    {
                        addFlag = false;
                        //break;
                    }
                }

                if (addFlag)
                {
                    MinValues1.Add(new Point(minValue, current, 0));
                }

                current -= jump;


            }

            for (int i = 265; i < 277; i++)
            {
                MinValues[i] = new Point(MinValues[265].X, MinValues[i].Y, MinValues[265].Z);
            }


            //Prawy górny róg wirnika tylnego
            for (int i = 252; i < 264; i++)
            {
                MaxValues[i] = new Point(MaxValues[252].X, MaxValues[i].Y, MaxValues[252].Z);
            }

            for (int i = 38; i < 50; i++)
            {
                MinValues[i] = new Point(MinValues[50].X, MinValues[i].Y, MinValues[50].Z);
            }

            for (int i = 38; i < 50; i++)
            {
                MaxValues[i] = new Point(MaxValues[50].X, MaxValues[i].Y, MaxValues[50].Z);
            }

            for (int i = 210; i < 225; i++)
            {
                //MaxValues[i] = new Point(MaxValues[50].X, MaxValues[i].Y, MaxValues[50].Z);
                MaxValues.Insert(i, new Point(MaxValues[209].X, MaxValues[i - 1].Y + jump, MaxValues[209].Z));
            }


            //minValues ma 59 elementow a i zaczyna sie od 51 moze byc nie tak

            for (int i = 0; i < 7; i++)
            {
                MinValues1.RemoveAt(34);
            }

            double delta;
            double deltaX = MinValues1[51].X - MinValues1[50].X;
            double deltaY = MaxValues1[51].Y - MaxValues1[50].Y;
            for (int i = 52; i < 78; i++)
            {
                //MinValues1[i] = new Point(MinValues1[i - 1].X + delta, MinValues1[i].Y, MinValues1[i].Z);
                MinValues1.Add(new Point(MinValues1[i - 1].X + deltaX, MinValues1[i - 1].Y + deltaY, MinValues1[i - 1].Z));

            }

            double u = MaxValues1[0].Y;
            deltaX = MaxValues1[0].X - MaxValues1[1].X;
            deltaY = MaxValues1[0].Y - MaxValues1[1].Y;
            while (u < 3.58)
            {

                MaxValues1.Insert(0, new Point(MaxValues1[0].X + deltaX, MaxValues1[0].Y + jump, MaxValues1[0].Z));
                u += jump;
            }
            MaxValues1.RemoveAt(33);
            for (int i = MinValues1.Count; i > 78; i--)
            {
                MinValues1.RemoveAt(78);
            }

            delta = MinValues[77].Y - MinValues[76].Y;
            double temp1 = delta * 12;
            for (int i = 64; i < 76; i++)
            {

                MinValues[i] = new Point(MinValues[76].X + temp1, MinValues[i].Y, MinValues[76].Z);
                temp1 -= delta;
            }

            while (MaxValues1.Count > 78)
            {
                MaxValues1.RemoveAt(78);
            }
            List<Point> OutputList = new List<Point>();


            OutputList.Add(new Point(MinValues[0].X + 0.8, MinValues[0].Y, 10));
            OutputList.Add(new Point(MinValues[0].X + 0.8, MinValues[0].Y, MinValues[0].Z));
            for (int i = 0; i < MinValues.Count; i += 10)
            {
                OutputList.Add(new Point(min, MinValues[i].Y, MinValues[i].Z));
                OutputList.Add(MinValues[i]);
                OutputList.Add(new Point(min, MinValues[i].Y, MinValues[i].Z));


            }
            OutputList.Add(new Point(min, MinValues.Last().Y, MinValues.Last().Z));
            OutputList.Add(MinValues.Last());
            OutputList.Add(new Point(min, MinValues.Last().Y, MinValues.Last().Z));
            OutputList.Add(new Point(min, MinValues.Last().Y, 10));




            OutputList.Add(new Point(MaxValues[0].X - 0.8, MaxValues[0].Y, 10));
            OutputList.Add(new Point(MaxValues[0].X - 0.8, MaxValues[0].Y, MaxValues[0].Z));
            for (int i = 0; i < MaxValues.Count; i += 10)
            {
                OutputList.Add(new Point(max, MaxValues[i].Y, MaxValues[i].Z));
                OutputList.Add(MaxValues[i]);
                OutputList.Add(new Point(max, MaxValues[i].Y, MaxValues[i].Z));


            }
            OutputList.Add(new Point(max, MaxValues.Last().Y, MaxValues.Last().Z));
            OutputList.Add(MaxValues.Last());
            OutputList.Add(new Point(max, MaxValues.Last().Y, MaxValues.Last().Z));
            OutputList.Add(new Point(max, MaxValues.Last().Y, 10));




            OutputList.Add(new Point(MinValues1[0].X, MinValues1[0].Y + 0.8, 10));
            OutputList.Add(new Point(MinValues1[0].X, MinValues1[0].Y + 0.8, MinValues1[0].Z));

            // for (int i = 0; i < 3; i += 10)
            //  {
            //OutputList.Add(new Point(min, MinValues[i].Y, MinValues[i].Z));
            //OutputList.Add(MinValues[i]);
            //OutputList.Add(new Point(min, MinValues[i].Y, MinValues[i].Z));
            //OutputList.Add(new Point(min, MinValues[i].Y, MinValues[i].Z));
            OutputList.Add(MinValues1[0]);
            OutputList.Add(MaxValues1[0]);
            OutputList.Add(MaxValues1[10]);
            OutputList.Add(MinValues1[10]);



            OutputList.Add(MinValues1[20]);
            OutputList.Add(MaxValues1[20]);
            OutputList.Add(MaxValues1[30]);

            OutputList.Add(MinValues1[30]);
            OutputList.Add(new Point(MinValues1[30].X, MinValues1[30].Y, 10));


            ////OutputList.Add(new Point(MinValues1[77].X, MinValues1[77].Y - 0.8, 10));
            ////OutputList.Add(new Point(MinValues1[77].X, MinValues1[77].Y - 0.8, MinValues1[77].Z));

            //int st = 77;
            //int d = 10;
            //for (int i = 2 * d; i < 41; i += 2 * d)
            //{


            //    OutputList.Add(MinValues1[st]);
            //    OutputList.Add(MaxValues1[st-14]);
            //    OutputList.Add(MaxValues1[st -14-d]);
            //    OutputList.Add(MinValues1[st -d]);

            //    st -= i;
            //    //break;
            //}


            ////OutputList.Add(MinValues1[77]);
            ////OutputList.Add(MaxValues1[63]);
            ////OutputList.Add(MaxValues1[55]);
            ////OutputList.Add(MinValues1[60]);
            ////OutputList.Add(MinValues1[60]);
            ////OutputList.Add(MaxValues1[42]);

            
            ////OutputList.Add(new Point(MaxValues1[42].X, MaxValues1[42].Y, 10));
        //}






            SavePath(OutputList, "2.f10");

        }


        private bool DebugZigZagfirstrun = true;
        private void GenerateZigZagPathExecuted()
        {
            double scale = 2;
            if (DebugZigZagfirstrun)
            {
                _trimCurvesCollection.Clear();
                TrimedCurvesPointsList.Clear();
                GL.Enable(EnableCap.PointSmooth);

                TrimedCurvesPointsList = new List<Point>();


                //Korpus
                _cursor.Coordinates = new Vector4d(scale * -0.9, scale * (3.8 - 3.6), scale * -0.1, scale * 0.0);
                UnselectAllItemsExecuted();
                _bezierPatchC2Collection[4].Selected = true;
                _bezierPatchC2Collection[0].Selected = true;
                TrimPatchesExecuted();
                List<Point[]> tempKorpus = new List<Point[]>();

                tempKorpus = _trimCurvesCollection.Last().NewtonOuputPoint.ToList();

                for (int i = _trimCurvesCollection.Last().NewtonOuputPoint.Count; i > 168; i--)
                {
                    _trimCurvesCollection.Last().NewtonOuputPoint.RemoveAt(168);
                }

                for (int i = 0; i < 523; i++)
                {
                    tempKorpus.RemoveAt(0);
                }
                //for (int i = 0; i < 14; i++)
                //{
                //    tempKorpus.RemoveAt(tempKorpus.Count - 1);
                //}

                AppendTrimmedCurvesPoints(_trimCurvesCollection.Last().NewtonOuputPoint, TrimedCurvesPointsList);
            

                //Ploza
                //_cursor.Coordinates = new Vector4d(scale * -0.9, scale * (3.8 - 3.6), scale * -0.1, scale * 0.0);
                _cursor.Coordinates = new Vector4d(-2.6,0.5,-0.2,0);
                UnselectAllItemsExecuted();
                _bezierPatchC2Collection[4].Selected = true;
                _bezierPatchC2Collection[2].Selected = true;
                TrimPatchesExecuted();
                //for (int i = 0; i < 37; i++)
                //{
                //    _trimCurvesCollection.Last().NewtonOuputPoint.RemoveAt(0);
                //}


                //for (int i = 0; i < 19; i++)
                //{
                //    _trimCurvesCollection.Last().NewtonOuputPoint.RemoveAt(_trimCurvesCollection.Last().NewtonOuputPoint.Count() - 1);
                //}

                AppendTrimmedCurvesPoints(_trimCurvesCollection.Last().NewtonOuputPoint, TrimedCurvesPointsList);
                AppendTrimmedCurvesPoints(tempKorpus, TrimedCurvesPointsList);

                //Wirnik tyl
                _cursor.Coordinates = new Vector4d(scale * 0.8, scale * (6.1 - 3.6), scale * -0.1, scale * 0.0);
                UnselectAllItemsExecuted();
                _bezierPatchC2Collection[4].Selected = true;
                _bezierPatchC2Collection[3].Selected = true;
                TrimPatchesExecuted();
                List<Point[]> WirnikTempCollection = new List<Point[]>();
                for (int i = 0; i < 196; i++)
                {
                    WirnikTempCollection.Add(_trimCurvesCollection.Last().NewtonOuputPoint.First());
                    _trimCurvesCollection.Last().NewtonOuputPoint.RemoveAt(0);
                }

                for (int i = 0; i < 39; i++)
                {
                    _trimCurvesCollection.Last().NewtonOuputPoint.RemoveAt(0);
                }
                AppendTrimmedCurvesPoints(_trimCurvesCollection.Last().NewtonOuputPoint, TrimedCurvesPointsList);



                //Wirnik tył
                _cursor.Coordinates = new Vector4d(scale * -0.1, scale * (6.6 - 3.6), scale * -0.1, scale * 0.0);
                UnselectAllItemsExecuted();
                _bezierPatchC2Collection[4].Selected = true;
                _bezierPatchC2Collection[3].Selected = true;
                TrimPatchesExecuted();


                ConnectTwoPoints(_trimCurvesCollection[2].NewtonOuputPoint.Last()[0],
                _trimCurvesCollection[3].NewtonOuputPoint.First()[0], TrimedCurvesPointsList);
                AppendTrimmedCurvesPoints(_trimCurvesCollection.Last().NewtonOuputPoint, TrimedCurvesPointsList);

                ConnectTwoPoints(_trimCurvesCollection[3].NewtonOuputPoint.Last()[0],
                    WirnikTempCollection.First()[0], TrimedCurvesPointsList);

                AppendTrimmedCurvesPoints(WirnikTempCollection, TrimedCurvesPointsList);

                //Korpus
                _cursor.Coordinates = new Vector4d(scale * 0.5, scale * (5.2 - 3.6), scale * -0.1, scale * 0.0);
                UnselectAllItemsExecuted();
                _bezierPatchC2Collection[4].Selected = true;
                _bezierPatchC2Collection[0].Selected = true;
                TrimPatchesExecuted();
                //ConnectTwoPoints(_trimCurvesCollection[1].NewtonOuputPoint.Last()[0],
                //    _trimCurvesCollection[2].NewtonOuputPoint.First()[0], TrimedCurvesPointsList);
                //  ConnectTwoPoints(_trimCurvesCollection[1].NewtonOuputPoint.First()[0],
                //       _trimCurvesCollection[2].NewtonOuputPoint.Last()[0], TrimedCurvesPointsList);

                tempKorpus.Clear();
                for (int i = _trimCurvesCollection.Last().NewtonOuputPoint.Count; i > 624; i--)
                {
                    tempKorpus.Add(_trimCurvesCollection.Last().NewtonOuputPoint[624]);
                    _trimCurvesCollection.Last().NewtonOuputPoint.RemoveAt(624);
                }

                AppendTrimmedCurvesPoints(_trimCurvesCollection.Last().NewtonOuputPoint, TrimedCurvesPointsList);

                //Wirnik
                _cursor.Coordinates = new Vector4d(scale * 0.7, scale * (3.4 - 3.6), scale * -0.1, scale * 0.0);
                UnselectAllItemsExecuted();
                _bezierPatchC2Collection[4].Selected = true;
                _bezierPatchC2Collection[1].Selected = true;
                TrimPatchesExecuted();
                _trimCurvesCollection.Last().NewtonOuputPoint.Reverse();
                //for (int i = 0; i < 49; i++)
                //{
                //    _trimCurvesCollection.Last().NewtonOuputPoint.RemoveAt(0);
                //}
                //for (int i = 0; i < 18; i++)
                //{
                //    _trimCurvesCollection.Last().NewtonOuputPoint.RemoveAt(_trimCurvesCollection.Last().NewtonOuputPoint.Count - 1);
                //}
                AppendTrimmedCurvesPoints(_trimCurvesCollection.Last().NewtonOuputPoint, TrimedCurvesPointsList);

                //Smiglo
                _cursor.Coordinates = new Vector4d(scale * 1.2, scale * (3.8 - 3.6),scale * -0.1, scale * 0.0);
                UnselectAllItemsExecuted();
                _bezierPatchCollection[0].Selected = true;
                _bezierPatchC2Collection[4].Selected = true;
                TrimPatchesExecuted();

                List<Point[]> SmigloTemp = new List<Point[]>();

                for (int i = 0; i < 393; i++)
                {
                    SmigloTemp.Add(_trimCurvesCollection.Last().NewtonOuputPoint[0]);
                    _trimCurvesCollection.Last().NewtonOuputPoint.RemoveAt(0);
                }

                for (int i = 0; i < 70; i++)
                {
                    _trimCurvesCollection.Last().NewtonOuputPoint.RemoveAt(0);
                }

                AppendTrimmedCurvesPoints(_trimCurvesCollection.Last().NewtonOuputPoint, TrimedCurvesPointsList);


                //Smiglo
                TrimedCurvesPointsList.Add(_bezierPatchCollection[0].GetPoint(1, 1));
                _cursor.Coordinates = new Vector4d(3.0, 0.4, scale * -0.1, scale * 0.0);
                UnselectAllItemsExecuted();
                _bezierPatchCollection[0].Selected = true;
                _bezierPatchC2Collection[4].Selected = true;
                TrimPatchesExecuted();
                AppendTrimmedCurvesPoints(_trimCurvesCollection.Last().NewtonOuputPoint, TrimedCurvesPointsList);

                TrimedCurvesPointsList.Add(_bezierPatchCollection[0].GetPoint(1, 0));
                AppendTrimmedCurvesPoints(SmigloTemp, TrimedCurvesPointsList);

                //Wirnik
                _cursor.Coordinates = new Vector4d(scale * 0.8, scale * (2.9 - 3.6), scale * -0.1, scale * 0.0);
                UnselectAllItemsExecuted();
                _bezierPatchC2Collection[4].Selected = true;
                _bezierPatchC2Collection[1].Selected = true;
                TrimPatchesExecuted();
                _trimCurvesCollection.Last().NewtonOuputPoint.Reverse();
                //for (int i = 0; i < 18; i++)
                //{
                //    _trimCurvesCollection.Last().NewtonOuputPoint.RemoveAt(0);
                //}
                //for (int i = 0; i < 36; i++)
                //{
                //    _trimCurvesCollection.Last().NewtonOuputPoint.RemoveAt(_trimCurvesCollection.Last().NewtonOuputPoint.Count - 1);
                //}
                AppendTrimmedCurvesPoints(_trimCurvesCollection.Last().NewtonOuputPoint, TrimedCurvesPointsList);
                for (int i = 0; i < 71; i++)
                {
                    tempKorpus.RemoveAt(0);
                }
                AppendTrimmedCurvesPoints(tempKorpus, TrimedCurvesPointsList);

                ConnectTwoPoints(TrimedCurvesPointsList.Last(),
                    TrimedCurvesPointsList.First(), TrimedCurvesPointsList);







                _trimCurvesCollection.Clear();
                //List<Point> SortedList = TrimedCurvesPointsList.OrderBy(o => o.Coordinates.Y).ToList();


                DebugZigZagfirstrun = false;
            }
          
            SavePath(TrimedCurvesPointsList, "CzystyKontur.f999");
            GnerateMinMaxXLists(TrimedCurvesPointsList);

        }


        private void UnselectAllItemsExecuted()
        {
            foreach (var item in TrimCurvesCollection)
            {
                item.Selected = false;
            }

            foreach (var point in _pointsCollection)
            {
                point.Selected = false;
            }

            foreach (var curve in _bezierCurveCollection)
            {
                curve.Selected = false;
            }

            foreach (var patch in BezierPatchCollection)
            {
                patch.Selected = false;
            }

            foreach (var patch in BezierPatchC2Collection)
            {
                patch.Selected = false;
            }

            foreach (var torus in TorusCollection)
            {
                torus.Selected = false;
            }
        }

        private void ShowCurvesUvExecuted()
        {
            foreach (var item in TrimCurvesCollection.Where(c => c.Selected))
            {
                //  var item = TrimCurvesCollection[0];
                BitmapsWindow win2 = new BitmapsWindow(item.StartPointHistory, item.StartPointForGradientDescentMethod);
                win2.Show();
            }




        }

        private void ConvertToInterpolationExecuted()
        {
            foreach (var item in TrimCurvesCollection)
            {
                if (item.Selected)
                {

                    ObservableCollection<Point> Points0 = new ObservableCollection<Point>();
                    ObservableCollection<Point> Points1 = new ObservableCollection<Point>();

                    foreach (var p in item.NewtonOuputPoint)
                    {
                        Points0.Add(p[0]);
                        Points1.Add(p[1]);
                    }
                    if (Points0.Count > 0)
                    {
                        var curve = new BezierCurveC2(Points0, true);
                        curve.RefreshScene += Refresh;
                        BezierCurveCollection.Add(curve);
                        Refresh();
                    }

                    if (Points1.Count > 0)
                    {
                        var curve = new BezierCurveC2(Points1, true);
                        curve.RefreshScene += Refresh;
                        BezierCurveCollection.Add(curve);
                        Refresh();
                    }
                }

            }

        }

        private void AddTorusExecuted()
        {
            TorusCollection.Add(new Torus(Cursor.Coordinates, Torus_r, Torus_R, Torus_division_fi, Torus_division_teta));
            Refresh();
        }


        public ImportExport ExchangeObject;

        public double[] StartingParametrization;

        public void TrimPatchesExecuted()
        {
            TrimCurvesCollection.Add(new TrimCurve(GradientDescentethodStepLength, GradientDescentStopCondition, GradientDescentethodStopStepsNumber, NewtonForwardStep, NewtonStopCondition, NewtonStepNumberCondition));
            double[] t = new double[4] { 0.1, 0.1, 0.1, 0.1 };
            IPatch patch1, patch2;
            List<IPatch> BPList = new List<IPatch>();

            foreach (var item in BezierPatchCollection)
            {
                if (item.Selected) BPList.Add(item);
                //BPList.Add(item);
            }

            foreach (var item in BezierPatchC2Collection)
            {
                if (item.Selected) BPList.Add(item);
                //BPList.Add(item);
            }

            foreach (var item in TorusCollection)
            {
                if (item.Selected) BPList.Add(item);
                //BPList.Add(item);
            }

            if (BPList.Count == 2)
            {
                Point cursorCenterPoint = new Point(Cursor.Coordinates.X, Cursor.Coordinates.Y, Cursor.Coordinates.Z);
                // StartingParametrization = TrimCurvesCollection[0].SearchStartingPointsForGradientDescentMethod(cursorCenterPoint, BPList[0], BPList[1]);
                TrimCurvesCollection.Last().CalclulateTrimmedCurve(cursorCenterPoint, BPList[0], BPList[1]);


                Refresh();
            }
            else if (BPList.Count == 1)
            {
                //MessageBox.Show("Za mało wybranych powierzcni");
                Point cursorCenterPoint = new Point(Cursor.Coordinates.X, Cursor.Coordinates.Y, Cursor.Coordinates.Z);
                TrimCurvesCollection.Last().CalclulateTrimmedCurve(cursorCenterPoint, BPList[0], BPList[0]);
                Refresh();
            }
            else if (BPList.Count == 0)
            {
                MessageBox.Show("Too less surface selected");
            }

            else
            {
                MessageBox.Show("Too many surface selected");
            }


        }



        public void AddBezierPatchC2Executed()
        {
            var patch = new BezierPatchC2(HorizontalPatches, VerticalPatches, PatchWidth, PatchHeight, PatchHorizontalDivision, PatchVerticalDivision, PatchesAreCylinder, Cursor.Coordinates);
            BezierPatchC2Collection.Add(patch);
            Refresh();
        }

        public void AddBezierPatchExecuted()
        {
            var patch = new BezierPatch(HorizontalPatches, VerticalPatches, PatchWidth, PatchHeight, PatchHorizontalDivision, PatchVerticalDivision, PatchesAreCylinder, Cursor.Coordinates);
            BezierPatchCollection.Add(patch);
            Refresh();
        }



        public Point[,] GregoryPointsCollection = new Point[2, 5];







        public List<Point[,]> GeneratedBezierPatchFromGregory = new List<Point[,]>();

        public void AddGregoryPatchExecuted()
        {

            try
            {
                var a = new GregoryPatch(BezierPatchCollection);
                GregoryPatchCollection.Add(a);
            }
            catch
            {
                MessageBox.Show("Creation error");
            }



            Refresh();
        }

        private void UndoAllTransformationExecuted()
        {
            M = Matrix4d.Identity;
            _scale = 0.1;
            Refresh();
            EllipseCounter = 1;
        }

        public bool PatchesAreCylinder
        {
            get { return _patchesAreCylinder; }
            set { _patchesAreCylinder = value; }
        }

        internal void DeleteSelectedGregoryPatches()
        {
            var temp = _gregoryPatchCollection.Where(c => c.Selected).ToList();

            foreach (var patch in temp)
            {
                _gregoryPatchCollection.Remove(patch);
            }
        }

        public int PatchHorizontalDivision
        {
            get { return _patchHorizontalDivision; }
            set
            {
                _patchHorizontalDivision = value;

                foreach (var item in _bezierPatchCollection)
                {
                    item.PatchHorizontalDivision = _patchHorizontalDivision;
                }
            }

        }

        public int PatchVerticalDivision
        {
            get { return _patchVerticalDivision; }
            set
            {
                _patchVerticalDivision = value;
                foreach (var item in _bezierPatchCollection)
                {
                    item.PatchVerticalDivision = _patchVerticalDivision;
                }
            }

        }

        public double PatchWidth
        {
            get { return _patchWidth; }
            set { _patchWidth = value; }
        }

        public double PatchHeight
        {
            get { return _patchHeight; }
            set { _patchHeight = value; }
        }

        public int HorizontalPatches
        {
            get { return _horizontalPatches; }
            set { _horizontalPatches = value; }
        }


        public int VerticalPatches
        {
            get { return _verticalPatches; }
            set { _verticalPatches = value; }
        }

        public void AddSelectedPointsExecuted()
        {
            foreach (var curve in _bezierCurveCollection.Where(p => p.Selected))
            {
                foreach (var point in _pointsCollection.Where(p => p.Selected))
                {
                    curve.AddPoint(point);
                    Refresh();
                }

            }
            Refresh();
        }
        public ObservableCollection<Curve> BezierCurveCollection
        {
            get { return _bezierCurveCollection; }
            set
            {
                _bezierCurveCollection = value;

                OnPropertyChanged("BezierCurveCollectiont");
                Refresh();
            }
        }

        //public ObservableCollection<BezierCurveC2> BezierCurveC2Collection
        //{
        //    get { return _bezierCurveC2Collection; }
        //    set
        //    {
        //        _bezierCurveC2Collection = value;
        //        OnPropertyChanged("BezierCurveC2Collectiont");
        //        Refresh();
        //    }
        //}
        public bool TorusEnabled
        {
            get
            {
                return _torusEnabled;
            }
            set
            {
                _torusEnabled = value;
                OnPropertyChanged("TorusEnabled");
                Refresh();

            }
        }

        public Tuple<int, int> MouseCoordinates
        {
            get { return _mouseCoordinates; }
            set { _mouseCoordinates = value; }
        }

        public bool MoveSelectedPointsWithCoursor
        {
            get { return _moveSelectedPointsWithCoursor; }
            set
            {
                _moveSelectedPointsWithCoursor = value;
                OnPropertyChanged("MoveSelectedPointsWithCoursor");
            }
        }

        public Cursor Cursor
        {
            get
            {
                return _cursor;

            }
            private set
            {
                _cursor = value;
                OnPropertyChanged("Cursor");
            }
        }

        public ObservableCollection<BezierPatch> BezierPatchCollection
        {
            get { return _bezierPatchCollection; }
            set
            {
                _bezierPatchCollection = value;
                OnPropertyChanged("BezierPAthCollection");
                Refresh();
            }
        }

        public ObservableCollection<BezierPatchC2> BezierPatchC2Collection
        {
            get { return _bezierPatchC2Collection; }
            set
            {
                _bezierPatchC2Collection = value;
                OnPropertyChanged("BezierPathC2Collection");
                Refresh();
            }
        }

        public ObservableCollection<TrimCurve> TrimCurvesCollection
        {
            get { return _trimCurvesCollection; }
            set
            {
                _trimCurvesCollection = value;
                OnPropertyChanged("TrimCurvesCollection");
                Refresh();
            }
        }

        public ObservableCollection<Torus> TorusCollection
        {
            get
            {
                return _torusCollection;

            }
            set
            {
                _torusCollection = value;
                OnPropertyChanged("TorusCollection");
                Refresh();
            }
        }
        public ObservableCollection<GregoryPatch> GregoryPatchCollection
        {
            get { return _gregoryPatchCollection; }
            set
            {
                _gregoryPatchCollection = value;
                OnPropertyChanged("GregoryPatchCollection");
                Refresh();
            }
        }
        public ObservableCollection<Point> PointsCollection
        {
            get { return _pointsCollection; }
            set
            {
                _pointsCollection = value;
                OnPropertyChanged("PointsCollection");
            }
        }

        public bool Stereoscopy
        {
            get { return _stereoscopy; }
            set
            {
                _stereoscopy = value;
                OnPropertyChanged("Stereoscopy");
                Refresh();
            }
        }
        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                SetViewPort();
            }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                SetViewPort();
            }
        }

        public double AlphaX
        {
            get { return _alphaX; }
            set
            {
                _alphaX = value;
                Refresh();
            }
        }

        public double AlphaY
        {
            get { return _alphaY; }
            set
            {
                _alphaY = value;
                Refresh();
            }
        }



        public double AlphaZ
        {
            get { return _alphaZ; }
            set
            {
                _alphaZ = value;
                Refresh();

            }
        }



        public double Scale
        {
            get { return _scale; }
            set
            {
                _scale = Math.Max(value, 0.01);
                EllipseCounter = 1;
            }
        }

        //public Torus Torus
        //{
        //    get
        //    {
        //        return _torus;
        //    }
        //    set
        //    {
        //        _torus = value;
        //        OnPropertyChanged("Torus");
        //    }
        //}


        public ICommand GregoryMergePoints { get { return _gregoryMergePoints ?? (_gregoryMergePoints = new ActionCommand(GregoryMergePointsExecuted)); } }
        public ICommand ClearSceneICommand { get { return _clearScene ?? (_clearScene = new ActionCommand(ClearSceneExecuted)); } }
        public ICommand AddBezierCurve { get { return _addBezierCurve ?? (_addBezierCurve = new ActionCommand(AddBezierCurveExecuted)); } }
        public ICommand AddBezierCurveC2 { get { return _addBezierCurveC2 ?? (_addBezierCurveC2 = new ActionCommand(AddBezierCurveC2Executed)); } }
        public ICommand AddBezierCurveC2Interpolation { get { return _addBezierCurveC2Interpolation ?? (_addBezierCurveC2Interpolation = new ActionCommand(AddBezierCurveC2InterpolationExecuted)); } }

        private void GregoryMergePointsExecuted()
        {

            GregoryPatch.MergePoints(BezierPatchCollection);
            Refresh();
        }

        private void ClearSceneExecuted()
        {
            ClearScene();
            Refresh();
        }

        private void AddBezierCurveC2Executed()
        {
            if (_pointsCollection.Any(point => point.Selected))
            {
                var curve = new BezierCurveC2(_pointsCollection.Where(point => point.Selected), false);
                curve.RefreshScene += Refresh;
                BezierCurveCollection.Add(curve);
                Refresh();
            }
        }

        private void AddBezierCurveC2InterpolationExecuted()
        {
            if (_pointsCollection.Any(point => point.Selected))
            {
                var curve = new BezierCurveC2(_pointsCollection.Where(point => point.Selected), true);
                curve.RefreshScene += Refresh;
                BezierCurveCollection.Add(curve);
                Refresh();
            }
        }

        public double NewtonForwardStep { get; set; }
        public double NewtonStopCondition { get; set; }
        public int NewtonStepNumberCondition { get; set; }
        public double GradientDescentethodStepLength { get; set; }
        public double GradientDescentStopCondition { get; set; }
        public int GradientDescentethodStopStepsNumber { get; set; }

        public Scene()
        {

            _scale = 0.1;
            _x = 0;
            _y = 0;
            _x0 = 0;
            _y0 = 0;
            _alphaX = 0;
            _alphaY = 0;
            _alphaZ = 0;
            _fi = 0;
            _teta = 0;
            _fi0 = 0;
            _teta0 = 0;
            _horizontalPatches = 1;
            _verticalPatches = 1;
            _patchWidth = 1;
            _patchHeight = 1;
            _patchHorizontalDivision = 4;
            _patchVerticalDivision = 4;
            _patchesAreCylinder = false;
            M = Matrix4d.Identity;
            //Torus = new Torus();
            PointsCollection = new ObservableCollection<Point>();
            _bezierCurveCollection = new ObservableCollection<Curve>();
            BezierPatchCollection = new ObservableCollection<BezierPatch>();
            BezierPatchC2Collection = new ObservableCollection<BezierPatchC2>();
            GregoryPatchCollection = new ObservableCollection<GregoryPatch>();
            TrimCurvesCollection = new ObservableCollection<TrimCurve>();
            TorusCollection = new ObservableCollection<Torus>();

            ExchangeObject = new ImportExport(BezierPatchC2Collection, BezierPatchCollection, PointsCollection, _bezierCurveCollection, TorusCollection);
            //_bezierCurveC2Collection = new ObservableCollection<BezierCurveC2>();
            //PointsCollection.Add(new Point(1, 1, 0));
            //PointsCollection.Add(new Point(-5, 1, 0));
            //PointsCollection.Add(new Point(-5, 2, 0));
            //PointsCollection.Add(new Point(1, 2, 0));

            //PointsCollection.Add(new Point(0, 0, 0));
            //PointsCollection.Add(new Point(1, 1, 0));
            //PointsCollection.Add(new Point(2, 1, 0));
            //PointsCollection.Add(new Point(3, 0, 0));
            Cursor = new Cursor();
            DrawEllipseFlag = false;
            EllipseA = 0.4;
            EllipseB = 0.1;
            EllipseC = 0.1;
            EllipseM = 0.8;
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dispatcherTimer.Start();


            NewtonForwardStep = -0.01;
            NewtonStopCondition = 0.001;
            NewtonStepNumberCondition = 20;
            GradientDescentethodStepLength = 0.001;
            GradientDescentStopCondition = 0.1;
            GradientDescentethodStopStepsNumber = 10000;


        }
        #endregion Public Properties

        private bool _drawEllipseFlag;

        public bool DrawEllipseFlag
        {
            get { return _drawEllipseFlag; }
            set
            {
                _drawEllipseFlag = value;
                Refresh();
            }
        }

        #region Private Methods
        public void ClearScene()
        {
            PointsCollection.Clear();
            BezierPatchC2Collection.Clear();
            BezierPatchCollection.Clear();
            BezierCurveCollection.Clear();
            GregoryPatchCollection.Clear();
            TrimCurvesCollection.Clear();
            TorusCollection.Clear();
            TrimedCurvesPointsList.Clear();
            MaxValues.Clear();
            MinValues.Clear();
            MaxValues1.Clear();
            MinValues1.Clear();
            DebugZigZagfirstrun = true;
            _heightArray.Clear();
        }
        public void LoadScene()
        {

            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Open";
            op.Filter = "Load users|*.json; *.json";
            // if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                //try
                //{
                // DeserializeDataSet(op.FileName);
                ClearScene();
                //ExchangeObject.LoadJson(op.FileName);
               // ExchangeObject.LoadJson("D:\\Studia\\Informatyka MGR\\Semestr 1\\ModelowanieGeometryczne\\MG testowe\\New folder\\Piotrek14+plaszczyzna symetriiC2_Final.json");
                ExchangeObject.LoadJson("D:\\Studia\\Informatyka MGR\\Semestr 1\\ModelowanieGeometryczne\\MG testowe\\New folder\\tt.json");
                //}
                //catch
                //{
                //    System.Windows.MessageBox.Show("File read error");
                //}
            }

            //GenerateEnvelopePathExecuted();
            Render();
        }

        public void SaveScene()
        {

            SaveFileDialog sa = new SaveFileDialog();
            sa.Title = "Save";
            sa.Filter = "Save users|*.json; *.json";
            //ExchangeObject.PrepareToSave();

            if (sa.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ExchangeObject.SaveJson(sa.FileName);
            }

        }
        void Refresh(object sender, PropertyChangedEventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            //BusyEllipseLed = 1;
            if (RefreshScene != null)
                RefreshScene(this, new PropertyChangedEventArgs("RefreshScene"));
            //BusyEllipseLed = 0;
        }
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        int MaxDivisionsNumber = 720;


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (EllipseCounter < MaxDivisionsNumber)
            {

                Refresh();
                EllipseCounter = EllipseCounter * 2;


            }
        }

        //private Thread elipseDrawer = new Thread(dispatcherTimer_Tick);

        //public void ResetWorker()
        //{
        //  //  elipseDrawer.
        //}

        //private void dispatcherTimer_Tick()
        //{
        //    if (EllipseCounter < 700)
        //    {
        //        Refresh();
        //        EllipseCounter = EllipseCounter * 2;

        //    }
        //}
        private int UPPER = 0;
        internal void Render()
        {


            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadIdentity();
            var scaleMatrix = MatrixProvider.ScaleMatrix(_scale);
            var translateMatrix = MatrixProvider.TranslateMatrix(_x / 720, -_y / 450, 0);
            var rotate = MatrixProvider.RotateXMatrix(_alphaX) * MatrixProvider.RotateYMatrix(_alphaY) * MatrixProvider.RotateZMatrix(_alphaZ);

            //Matrix multiplication
            M = scaleMatrix * translateMatrix * rotate * M;

            _scale = 1;
            _alphaX = 0;
            _alphaY = 0;
            _alphaZ = 0;
            _x = 0;
            _y = 0;
            _fi = 0;
            _teta = 0;

            if (_finishPathHelpers)
            {
                _finishPathGenerator.Draw(M);
            }
            if (_initialHelpers)
            {
                _heightArray.Draw(M);
            }

            if (_contourHelpers)
            {
                foreach (var item in TrimedCurvesPointsListWithOffset)
                {
                    item.Draw(M, 1, 0, 1, 0);
                }

            }

            if (_zigZagHelpers)
            {
                foreach (var item in MaxValues)
                {
                    item.Draw(M, 100, 0, 0, 1);
                }

                foreach (var item in MinValues)
                {
                    item.Draw(M, 100, 0, 1, 1);
                }

                foreach (var item in MaxValues1)
                {
                    item.Draw(M, 100, 1, 0, 1);
                }

                foreach (var item in MinValues1)
                {
                    item.Draw(M, 100, 1, 1, 1);
                }

                ////  UPPER = 0;
                foreach (var item in TrimedCurvesPointsList)
                {
                    //  UPPER++;
                    //for (int i = 0; i < UPPER; i++)
                    //{
                    // UPPER = 663;

                    item.Draw(M, 10, 1, 0, 0);
                    // TrimedCurvesPointsList[i].Draw(M, 10, 1, 0, 0);
                    //        if (UPPER == 607)
                    //        {
                    //            break;
                    //       }
                }
            }


            if (ShowDescentGradientsSteps)
            {

                foreach (var item2 in TrimCurvesCollection)
                {
                    double k = 1.0 / item2.PointsHistoryGradientDescent.Count;
                    double k1 = 0;
                    foreach (var item in item2.PointsHistoryGradientDescent)
                    {
                        item[0].Draw(M, 10, k1, 1, 0);
                        item[1].Draw(M, 10, 0, 1, k1);
                    }

                }
            }

            if (_showTrimmingCurves)
            {
                foreach (var item in TrimCurvesCollection)
                {
                    foreach (var item2 in item.NewtonOuputPoint)
                    {
                        item2[0].Draw(M, 20, 0, 1, 1);
                        item2[1].Draw(M, 20, 1, 0, 1);
                    }
                }
            }

            if (DrawEllipseFlag)
            {
                // BusyEllipseLed = 1.0;
                YellowEllipse.Draw(M, EllipseA, EllipseB, EllipseC, EllipseM, EllipseCounter);

            }
            //BusyEllipseLed = 0.0;
            //wywoływanie rysowania torusa    

            //TODO: Rysowanie torusa
            //if (TorusEnabled)
            //{
            //    if (Stereoscopy)
            //    {
            //        _torus.DrawStereoscopy(M);
            //    }
            //    else
            //    {
            //        _torus.Draw(M);
            //    }
            //}

            if (Stereoscopy)
            {
                Cursor.DrawStereoscopy(M);
            }
            else
            {
                Cursor.Draw(M);

            }

            foreach (var i in _pointsCollection)
            {
                if (Stereoscopy)
                {
                    i.DrawStereoscopy(M);
                }
                else
                {
                    i.Draw(M);
                }
            }

            foreach (var curve in _bezierCurveCollection)
            {
                if (Stereoscopy)
                {
                    curve.DrawCurveStereoscopy(M);
                    curve.DrawPolylineStereoscopy(M);

                }
                else
                {
                    curve.DrawCurve(M);
                    curve.DrawPolyline(M);

                }
            }
            foreach (var patch in BezierPatchCollection)
            {
                patch.DrawSurface(M);
                patch.DrawPolyline(M);
            }

            foreach (var patch in BezierPatchC2Collection)
            {
                patch.DrawPoints(M);
                patch.DrawPolyline(M);
                patch.DrawPatch(M);

            }

            foreach (var patch in GregoryPatchCollection)
            {
                patch.Draw(M);
                ////punkty kontrolne
                //foreach (var item in patch.ControlArrayC1)
                //{
                //    for (int i = 1; i < 6; i++)
                //    {

                //        item[0][i].Draw(M, 5, 1, 0, 0);
                //        item[1][i].Draw(M, 5, 1, 0, 0);
                //    }
                //}



            }

            foreach (var torus in TorusCollection)
            {
                torus.Draw(M);
            }

            //// Draw GregoryPatch
            //foreach (var item in P3)
            //{
            //    item[0].Draw(M, 4, 0.6, 0, 0.3);
            //    item[1].Draw(M, 4, 0, 0.6, 0.3);
            //}

            ////foreach (var item in Q)
            ////{
            ////    item.Draw(M, 4, 0.3, 0, 0.6);

            ////}

            //if (P0 != null)
            //{
            //    P0.Draw(M, 4, 0, 1, 0);
            //}

            //foreach (var item in P1)
            //{
            //    item.Draw(M, 4, 0, 0, 1);

            //}
            //foreach (var item in PointsOnBoundaryAndC1ConditionPoints)
            //{
            //    for (int i = 0; i < item.GetLength(0); i++)
            //    {
            //        for (int j = 0; j < item.GetLength(1); j++)
            //        {
            //            item[i, j].Draw(M, 4, 1, 0, 0);
            //        }

            //    }

            //}
            //foreach (var item in UsedToCalculateRestOfPoints)
            //{
            //    item.Draw(M, 0, 1, 0);
            //}

            //foreach (var item in RestOfPoints)
            //{
            //    item.Draw(M, 0, 0, 1);
            //}
            //GL.Begin(BeginMode.Quads);
            //GL.Color3(Color.Aqua);
            //GL.Vertex2(-0.5f, -0.5f);
            //GL.Vertex2(0.5f, -0.5f);
            //GL.Vertex2(0.5f, 0.5f);
            //GL.Vertex2(-0.5f, 0.5f);
            ////GL.Color3(Color.Bisque);
            ////GL.Vertex2(-1f, -1f);
            ////GL.Vertex2(0.5f, -0.1f);
            ////GL.Vertex2(0.5f, 0.5f);
            ////GL.Vertex2(-0.5f, 0.5f);
            //GL.End();




            GL.Flush();


        }

        private void SetViewPort()
        {
            GL.Viewport(0, 0, (int)Width, (int)Height);
        }

        private void AddBezierCurveExecuted()
        {
            if (_pointsCollection.Any(point => point.Selected))
            {
                var curve = new BezierCurve(_pointsCollection.Where(point => point.Selected));
                curve.RefreshScene += Refresh;
                BezierCurveCollection.Add(curve);
                Refresh();
            }
        }

        internal void SetCurrentCoordinate(int x, int y)
        {
            _x0 = x;
            _y0 = y;
            EllipseCounter = 1;
        }

        internal void SetCurrentRotation(int fi, int teta)
        {
            _fi0 = fi;
            _teta0 = teta;
            EllipseCounter = 1;
        }

        #endregion Private Methods
        #region Public Methods

        public void MoveCursor(double dx, double dy, double dz)
        {

            _cursor.Coordinates += new Vector4d(dx, dy, dz, 0);
            if (_moveSelectedPointsWithCoursor)
            {
                MoveSelectedPoints(dx, dy, dz);
            }
        }

        public void MoveSelectedPoints(double dx, double dy, double dz)
        {
            foreach (var p in _pointsCollection.Where(point => point.Selected))
            {
                p.X += dx;
                p.Y += dy;
                p.Z += dz;
            }

            foreach (var patch in BezierPatchCollection)
            {


                //foreach (var p in patch.Vertices.Where(point => point.Selected))
                //{
                //    p.X += dx;
                //    p.Y += dy;
                //    p.Z += dz;

                //}
                patch._patchPoints = patch.GetAllPointsInOneArray();
                if (patch.PatchesAreCylinder)
                {
                    for (int i = 0; i < patch.PatchPoints.GetLength(0); i++)
                    {
                        for (int j = 0; j < patch.PatchPoints.GetLength(1) - 1; j++)
                        {
                            if (patch.PatchPoints[i, j].Selected)
                            {
                                patch.PatchPoints[i, j].X += dx;
                                patch.PatchPoints[i, j].Y += dy;
                                patch.PatchPoints[i, j].Z += dz;
                            }


                        }
                    }
                }
                else
                {
                    for (int i = 0; i < patch.PatchPoints.GetLength(0); i++)
                    {
                        for (int j = 0; j < patch.PatchPoints.GetLength(1); j++)
                        {
                            if (patch.PatchPoints[i, j].Selected)
                            {
                                patch.PatchPoints[i, j].X += dx;
                                patch.PatchPoints[i, j].Y += dy;
                                patch.PatchPoints[i, j].Z += dz;
                            }


                        }
                    }
                }
            }
            foreach (var patch in BezierPatchCollection)
            {   //Odświeżanie płatów C0, zwiazane z tym że punkt na łączeniu obrabiany dwa razy
                patch.PlaceVerticesToPatches4x4();
                patch.RecalculatePatches();
            }


            foreach (var patch in BezierPatchC2Collection)
            {

                for (int i = 0; i < patch.PatchPoints.GetLength(0); i++)
                {
                    for (int j = 0; j < patch.PatchPoints.GetLength(1); j++)
                    {
                        if (patch.PatchPoints[i, j].Selected == true)
                        {
                            patch.PatchPoints[i, j].X += dx;
                            patch.PatchPoints[i, j].Y += dy;
                            patch.PatchPoints[i, j].Z += dz;
                            patch.CalculateBezierPoints();
                        }
                    }
                }
            }



            for (int i = 0; i < GregoryPatchCollection.Count; i++)
            {
                try
                {
                    GregoryPatchCollection[i].CalculateGregoryPatch();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    GregoryPatchCollection.RemoveAt(i);
                    i--;
                }
            }
        }



        public void SelectPointByCursor()
        {
            const double epsilon = 0.2;
            var c = _cursor.Coordinates;
            var temp = c;
            foreach (var p in _pointsCollection)
            {
                temp = new Vector4d(c.X - p.X, c.Y - p.Y, c.Z - p.Z, 0);

                if (temp.Length < epsilon)
                {
                    p.Selected = !p.Selected;
                }
            }
        }

        private double _clickX = 0, _clickY = 0;
        public double ClickX
        {
            get { return _clickX; }
            set
            {
                _clickX = value;
                OnPropertyChanged("ClickX");
            }
        }
        public double ClickY
        {
            get { return _clickY; }
            set
            {
                _clickY = value;
                OnPropertyChanged("ClickY");
            }
        }

        private bool _zigZagHelpers = false;
        public bool ZigZagHelpers
        {
            get { return _zigZagHelpers; }
            set
            {
                _zigZagHelpers = value;
                OnPropertyChanged(nameof(ZigZagHelpers));
            }
        }
        private bool _initialHelpers = false;
        public bool InitialHelpers
        {
            get { return _initialHelpers; }
            set
            {
                _initialHelpers = value;
                OnPropertyChanged(nameof(InitialHelpers));
            }
        }

        private bool _contourHelpers = false;
        public bool ContourHelpers
        {
            get { return _contourHelpers; }
            set
            {
                _contourHelpers = value;
                OnPropertyChanged(nameof(ContourHelpers));
            }
        }

        private bool _finishPathHelpers = true;
        public bool FinishPathHelpers
        {
            get { return _finishPathHelpers; }
            set
            {
                _finishPathHelpers = value;
                OnPropertyChanged(nameof(FinishPathHelpers));
            }
        }

        //private double _busyColor = 0;
        //public double BusyEllipseLed
        //{
        //    get { return _busyColor; }
        //    set
        //    {
        //        _busyColor = value;
        //        OnPropertyChanged("BusyEllipseLed");

        //    }
        //}

        public void SelectPointByMouse()
        {
            const double epsilon = 15;
            Vector4d c = new Vector4d((_x0 - Width / 2.0) / (Width / 1440), (_y0 - Height / 2.0) / (Height / 750), 0, 0);
            //Vector4d c = new Vector4d((_x0 - Width / 2.0), (_y0 - Height / 2.0) , 0, 0);

            ClickX = c.X;
            ClickY = c.Y;
            var temp = c;
            foreach (var p in _pointsCollection)
            {
                temp = new Vector4d(c.X - p.X_Window, -c.Y - p.Y_Window, 0, 0);

                if (temp.Length < epsilon)
                {
                    p.Selected = !p.Selected;
                }
            }

            foreach (var patch in BezierPatchCollection)
            {
                foreach (var point in patch.Vertices)
                {
                    temp = new Vector4d(c.X - point.X_Window, -c.Y - point.Y_Window, 0, 0);

                    if (temp.Length < epsilon)
                    {
                        point.Selected = !point.Selected;
                        return;
                    }
                }
            }

            foreach (var patch in BezierPatchC2Collection)
            {
                foreach (var point in patch.PatchPoints)
                {
                    temp = new Vector4d(c.X - point.X_Window, -c.Y - point.Y_Window, 0, 0);

                    if (temp.Length < epsilon)
                    {
                        point.Selected = !point.Selected;
                        return;
                    }
                }
            }

            foreach (var patch in GregoryPatchCollection)
            {
                patch.Draw(M);
            }
        }
        public void DeleteSelectedPoints()
        {
            var temp = _pointsCollection.Where(c => c.Selected).ToList();

            foreach (var p in temp)
            {
                _pointsCollection.Remove(p);
            }

            foreach (var bezierCurve in _bezierCurveCollection)
            {
                bezierCurve.RemovePoints(temp);
            }
        }
        public void AddPointByCursor()
        {
            var point = new Point(_cursor.Coordinates.X, _cursor.Coordinates.Y, _cursor.Coordinates.Z);
            PointsCollection.Add(point);

            foreach (var curve in _bezierCurveCollection.Where(c => c.Selected))
            {
                curve.AddPoint(point);
            }
        }
        public void MouseMoveTranslate(int x, int y)
        {
            _x = x - _x0;
            _y = y - _y0;
            _x0 = x;
            _y0 = y;
        }
        #endregion Public Methods

        internal void DeleteSelectedCurves()
        {
            var temp = _bezierCurveCollection.Where(c => c.Selected).ToList();

            foreach (var curve in temp)
            {
                _bezierCurveCollection.Remove(curve);
            }
        }

        internal void DeleteSelectedPatches()
        {
            var temp = _bezierPatchCollection.Where(c => c.Selected).ToList();

            foreach (var patch in temp)
            {
                _bezierPatchCollection.Remove(patch);
            }
        }

        internal void DeleteSelectedPatchesC2()
        {
            var temp = _bezierPatchC2Collection.Where(c => c.Selected).ToList();

            foreach (var patch in temp)
            {
                _bezierPatchC2Collection.Remove(patch);
            }
        }

        internal void MouseMoveRotate(int fi, int teta)
        { //fi rotate around screen x axis
          //teta rotate around screen y axis
            _fi = fi - _fi0;
            _teta = teta - _teta0;
            _fi0 = fi;
            _teta0 = teta;

            _alphaX = 4 * _teta / 750;
            _alphaY = 4 * _fi / 1440;
            _alphaZ = 0;
        }

        public void DeleteSelectedTrimCurve()
        {
            var temp = _trimCurvesCollection.Where(c => c.Selected).ToList();

            foreach (var curve in temp)
            {
                _trimCurvesCollection.Remove(curve);
            }
        }

        public void DeleteSelectedToruses()
        {
            var temp = _torusCollection.Where(c => c.Selected).ToList();

            foreach (var torus in temp)
            {
                _torusCollection.Remove(torus);
            }
        }
    }
}