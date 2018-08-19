﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Navigation;
using ModelowanieGeometryczne.Helpers;
using ModelowanieGeometryczne.Model;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Cursor = ModelowanieGeometryczne.Model.Cursor;
using BezierCurve = ModelowanieGeometryczne.Model.BezierCurve;

namespace ModelowanieGeometryczne.ViewModel
{
    public class Scene : ViewModelBase
    {
        public event PropertyChangedEventHandler RefreshScene;

        #region Private Fields
       
        private Cursor _cursor;
        private Torus _torus;
        private double _height;
        private double _width;
        private double _x, _y, _x0, _y0, _alphaX, _alphaY, _alphaZ, _fi, _teta, _fi0, _teta0;
        private double _scale;
        private Matrix4d M;
        private Matrix4 _projection;
        private bool _stereoscopy;
        Tuple<int, int> _mouseCoordinates;
        private ObservableCollection<Point> _pointsCollection;//
        private ObservableCollection<Curve> _bezierCurveCollection;//
        private ObservableCollection<BezierPatch> _bezierPatchCollection;
        private ObservableCollection<BezierPatchC2> _bezierPatchC2Collection;
        private ObservableCollection<GregoryPatch> _gregoryPatchCollection;

        private bool _moveSelectedPointsWithCoursor = false;
        private bool _torusEnabled = false;
        private ICommand _gregoryMergePoints;
        private ICommand _clearScene;
        private ICommand _addBezierPatch;
        private ICommand _addGregoryPatch;
        private ICommand _addBezierPatchC2;
        private ICommand _addBezierCurve;
        private ICommand _addBezierCurveC2;
        private ICommand _addBezierCurveC2Interpolation;
        private ICommand _addPoints;
        private ICommand _undoAllTransformation;

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



        public ICommand AddPointsCommand { get { return _addPoints ?? (_addPoints = new ActionCommand(AddSelectedPointsExecuted)); } }

        public ICommand UndoAllTransformation { get { return _undoAllTransformation ?? (_undoAllTransformation = new ActionCommand(UndoAllTransformationExecuted)); } }

        public ICommand AddBezierPatch { get { return _addBezierPatch ?? (_addBezierPatch = new ActionCommand(AddBezierPatchExecuted)); } }

        public ICommand AddGregoryPatch { get { return _addGregoryPatch ?? (_addBezierPatch = new ActionCommand(AddGregoryPatchExecuted)); } }
        public ICommand AddBezierPatchC2 { get { return _addBezierPatchC2 ?? (_addBezierPatchC2 = new ActionCommand(AddBezierPatchC2Executed)); } }


        public ImportExport ExchangeObject;

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
            GregoryPatchCollection.Add(new GregoryPatch(BezierPatchCollection));
            Refresh();
        }

        private void UndoAllTransformationExecuted()
        {
            M = Matrix4d.Identity;
            _scale = 0.1;
            Refresh();
        }

        public bool PatchesAreCylinder
        {
            get { return _patchesAreCylinder; }
            set { _patchesAreCylinder = value; }
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

        }
        #endregion Public Properties



        #region Private Methods
        public void ClearScene()
        {
            PointsCollection.Clear();
            BezierPatchC2Collection.Clear();
            BezierPatchCollection.Clear();
            BezierCurveCollection.Clear();
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

            GL.Flush();

        }

        private void SetViewPort()
        {
            GL.Viewport(0, 0, 1440, 750);
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
        }

        internal void SetCurrentRotation(int fi, int teta)
        {
            _fi0 = fi;
            _teta0 = teta;
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

        public void SelectPointByMouse()
        {
            const double epsilon = 15;
            Vector4d c = new Vector4d(_x0 - 1440.0 / 2.0, _y0 - 750.0 / 2.0, 0, 0);
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