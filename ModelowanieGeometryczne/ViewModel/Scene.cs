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

        private bool _moveSelectedPointsWithCoursor = false;
        private bool _torusEnabled = false;
        private ICommand _addBezierPatch;
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
        public ICommand AddBezierPatch { get { return _addBezierPatch ?? (_addBezierPatch = new ActionCommand(AddBezierPatchExecuted)); } }
        public ICommand UndoAllTransformation { get { return _undoAllTransformation ?? (_undoAllTransformation = new ActionCommand(UndoAllTransformationExecuted)); } }

        public void AddBezierPatchExecuted()
        {
            var patch = new BezierPatch(HorizontalPatches, VerticalPatches, PatchWidth, PatchHeight, PatchHorizontalDivision, PatchVerticalDivision, PatchesAreCylinder, Cursor.Coordinates);
            BezierPatchCollection.Add(patch);
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
            set { _patchHorizontalDivision = value; }

        }

        public int PatchVerticalDivision
        {
            get { return _patchVerticalDivision; }
            set { _patchVerticalDivision = value; }

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

        public ICommand AddBezierCurve { get { return _addBezierCurve ?? (_addBezierCurve = new ActionCommand(AddBezierCurveExecuted)); } }
        public ICommand AddBezierCurveC2 { get { return _addBezierCurveC2 ?? (_addBezierCurveC2 = new ActionCommand(AddBezierCurveC2Executed)); } }
        public ICommand AddBezierCurveC2Interpolation { get { return _addBezierCurveC2Interpolation ?? (_addBezierCurveC2Interpolation = new ActionCommand(AddBezierCurveC2InterpolationExecuted)); } }

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
            _horizontalPatches = 3;
            _verticalPatches = 3;
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
            //_bezierCurveC2Collection = new ObservableCollection<BezierCurveC2>();
            PointsCollection.Add(new Point(0, 0, 1));
            PointsCollection.Add(new Point(1, 1, 0));
            PointsCollection.Add(new Point(2, 1, 0));
            PointsCollection.Add(new Point(3, 0, 0));
            PointsCollection.Add(new Point(2, -1, 0));
            PointsCollection.Add(new Point(1, -1, 0));
            //double a = 0.25;
            //PointsCollection.Add(new Point(0.5, 0, 0));
            //PointsCollection.Add(new Point(0.5, a, 0));
            //PointsCollection.Add(new Point(a, 0.5, 0));
            //PointsCollection.Add(new Point(0, 0.5, 0));
            //PointsCollection.Add(new Point(-a, 0.5, 0));
            //PointsCollection.Add(new Point(-0.5, a, 0));
            //PointsCollection.Add(new Point(-0.5, 0, 0));
            //PointsCollection.Add(new Point(-0.5, -a, 0));
            //PointsCollection.Add(new Point(-a, -0.5, 0));
            //PointsCollection.Add(new Point(0, -0.5, 0));
            //PointsCollection.Add(new Point(a, -0.5, 0));
            //PointsCollection.Add(new Point(0.5, -a, 0));
            //PointsCollection.Add(new Point(0.5, 0, 0));
            Cursor = new Cursor();

        }
        #endregion Public Properties



        #region Private Methods

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
                patch.Draw(M);
            }



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

                foreach (var p in patch.Vertices.Where(point => point.Selected))
                {
                    p.X += dx;
                    p.Y += dy;
                    p.Z += dz;
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
                    }
                }
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