using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Navigation;
using ModelowanieGeometryczne.Helpers;
using ModelowanieGeometryczne.Model;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Cursor = ModelowanieGeometryczne.Model.Cursor;
using BezierCurve = ModelowanieGeometryczne.Model.BezierCurve;
using Point = ModelowanieGeometryczne.Model.Point;

namespace ModelowanieGeometryczne.ViewModel
{
    public class Scene : ViewModelBase
    {
        public event PropertyChangedEventHandler RefreshScene;

        #region Private Fields

        private int EllipseCounter = 1;


        private Cursor _cursor;
        private Torus _torus;
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
        public Ellipse YellowEllipse = new Ellipse();

        private bool _moveSelectedPointsWithCoursor = false;
        private bool _torusEnabled = false;
        private ICommand _gregoryMergePoints;
        private ICommand _clearScene;
        private ICommand _addBezierPatch;
        private ICommand _addGregoryPatch;
        private ICommand _addBezierPatchC2;
        private ICommand _trimPatches;
        private ICommand _addBezierCurve;
        private ICommand _addBezierCurveC2;
        private ICommand _addBezierCurveC2Interpolation;
        private ICommand _addPoints;
        private ICommand _undoAllTransformation;

        private bool _showDescentGradientsSteps = true;
        public bool ShowDescentGradientsSteps
        {
            get { return _showDescentGradientsSteps; }
            set
            {
                _showDescentGradientsSteps = value;
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


        public ImportExport ExchangeObject;

        public double[] StartingParametrization;
        public List<Point[]> pointsStartingTrimming = new List<Point[]>();
        public void TrimPatchesExecuted()
        {
            TrimCurvesCollection.Add(new TrimCurve());
            double[] t = new double[4] { 0.1, 0.1, 0.1, 0.1 };
            BezierPatch BezierPatch1, BezierPatch2;
            List<BezierPatch> BPList = new List<BezierPatch>();

            foreach (var item in BezierPatchCollection)
            {
                //if (item.Selected) BPList.Add(item);
                BPList.Add(item);
            }

            if (BPList.Count == 2)
            {
                Point cursorCenterPoint = new Point(Cursor.Coordinates.X, Cursor.Coordinates.Y, Cursor.Coordinates.Z);
                StartingParametrization = TrimCurvesCollection[0].SearchStartingPointsForGradientDescentMethod(cursorCenterPoint, BPList[0].GetAllPointsInOneArray(), BPList[1].GetAllPointsInOneArray());
                TrimCurvesCollection[0].CalclulateTrimmedCurve(StartingParametrization, BPList[0], BPList[1]);
                pointsStartingTrimming = TrimCurvesCollection[0].PointsHistoryGradientDescent;

                Refresh();
            }
            else
            {
                MessageBox.Show("Za mało wybranych powierzcni C0");
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

        internal void DeleteGregoryPatches()
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

        public Torus Torus
        {
            get
            {
                return _torus;
            }
            set
            {
                _torus = value;
                OnPropertyChanged("Torus");
            }
        }


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
            Torus = new Torus();
            PointsCollection = new ObservableCollection<Point>();
            _bezierCurveCollection = new ObservableCollection<Curve>();
            BezierPatchCollection = new ObservableCollection<BezierPatch>();
            BezierPatchC2Collection = new ObservableCollection<BezierPatchC2>();
            GregoryPatchCollection = new ObservableCollection<GregoryPatch>();
            TrimCurvesCollection = new ObservableCollection<TrimCurve>();

            ExchangeObject = new ImportExport(BezierPatchC2Collection, BezierPatchCollection, PointsCollection, _bezierCurveCollection);
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
        }
        public void LoadScene()
        {

            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Open";
            op.Filter = "Load users|*.json; *.json";
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //try
                //{
                // DeserializeDataSet(op.FileName);
                ClearScene();
                ExchangeObject.LoadJson(op.FileName);
                //}
                //catch
                //{
                //    System.Windows.MessageBox.Show("File read error");
                //}
            }

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
            if (RefreshScene != null)
                RefreshScene(this, new PropertyChangedEventArgs("RefreshScene"));
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

            if (ShowDescentGradientsSteps)
            {
                foreach (var item in pointsStartingTrimming)
                {
                    item[0].Draw(M, 10, 1, 0, 0);
                    item[1].Draw(M, 10, 0, 0, 1);



                    TrimCurvesCollection[0].NewtonPointToGo.Draw(M, 20, 1, 1, 1);
                    //TrimCurvesCollection[0].NewtonStartPoint.Draw(M, 20, 1, 0.5, 0);
                }

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
                YellowEllipse.Draw(M, EllipseA, EllipseB, EllipseC, EllipseM, EllipseCounter);
            }

            //wywoływanie rysowania torusa    
            if (TorusEnabled)
            {
                if (Stereoscopy)
                {
                    _torus.DrawStereoscopy(M);
                }
                else
                {
                    _torus.Draw(M);
                }
            }

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
                //punkty kontrolne
                //foreach (var item in patch.ControlArrayC1)
                //{
                //    for (int i = 1; i < 6; i++)
                //    {

                //        item[0][i].Draw(M, 5, 1, 0, 0);
                //        item[1][i].Draw(M, 5, 1, 0, 0);
                //    }
                //}



            }

            //// Draw GregoryPatch
            // foreach (var item in P3)
            // {
            //     item[0].Draw(M, 4, 0.6, 0, 0.3);
            //     item[1].Draw(M, 4, 0, 0.6, 0.3);
            // }

            // //foreach (var item in Q)
            // //{
            // //    item.Draw(M, 4, 0.3, 0, 0.6);

            // //}

            // if (P0 != null)
            // {
            //     P0.Draw(M, 4, 0, 1, 0);
            // }

            // foreach (var item in P1)
            // {
            //     item.Draw(M, 4, 0, 0, 1);

            // }
            // foreach (var item in PointsOnBoundaryAndC1ConditionPoints)
            // {
            //     for (int i = 0; i < item.GetLength(0); i++)
            //     {
            //         for (int j = 0; j < item.GetLength(1); j++)
            //         {
            //             item[i, j].Draw(M, 4, 1, 0, 0);
            //         }

            //     }

            // }
            // foreach (var item in UsedToCalculateRestOfPoints)
            // {
            //     item.Draw(M, 0, 1, 0);
            // }

            // foreach (var item in RestOfPoints)
            // {
            //     item.Draw(M, 0, 0, 1);
            // }
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
    }
}