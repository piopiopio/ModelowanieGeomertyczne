using System;
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
using BezierCurve = ModelowanieGeometryczne.Model.BezierCurve;
using Cursor = ModelowanieGeometryczne.Model.Cursor;

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
        private double _x, _y, _x0, _y0, _alphaX, _alphaY, _alphaZ;
        private double _scale;
        private Matrix4d M;
        private Matrix4 _projection;
        private bool _stereoscopy;
        Tuple<int, int> _mouseCoordinates;
        private ObservableCollection<Point> _pointsCollection;//
        private ObservableCollection<BezierCurve> _bezierCurveCollection;//
        
        //private List<Point> _pointsCollection = new List<Point>();
        private ObservableCollection<Point> _selectedPointsCollection;
        private bool _moveSelectedPointsWithCoursor = false;
        private bool _torusEnabled = false;
        private ICommand _addBezierCurve;


        #endregion Private Fields

        #region Public Properties

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



        //public ObservableCollection<Point> SelectedPointsCollection
        //{
        //    get { return _selectedPointsCollection; }
        //    set
        //    {
        //        _selectedPointsCollection = value;
        //    }
        //}


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
                //Refresh();
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


        public Scene()
        {
            M = Matrix4d.Identity;
            // DefineDrawingMode();
            _scale = 0.1;
            //_x = -2*700;
            //_y = 2*400;
            _x = 0;
            _y = 0;
            _alphaX = 0;
            _alphaY = 0;
            _alphaZ = 0;
            M = Matrix4d.Identity;
            Torus = new Torus();
            // _projection = MatrixProvider.ProjectionMatrix(100);
            //M = _projection;
            _selectedPointsCollection = new ObservableCollection<Point>();
            PointsCollection = new ObservableCollection<Point>();
            _bezierCurveCollection = new ObservableCollection<BezierCurve>();
            PointsCollection.Add(new Point(0, 0, 10));
            PointsCollection.Add(new Point(0, 0, 0));
            PointsCollection.Add(new Point(1, 1, -1));

            Cursor = new Cursor();

        }
        #endregion Public Properties

        #region Private Methods
        private void Refresh()
        {
            if (RefreshScene != null)
                RefreshScene(this, new PropertyChangedEventArgs("RefreshScene"));
        }


        private void DefineDrawingMode()
        {
            GL.ShadeModel(ShadingModel.Smooth);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
            GL.DepthFunc(DepthFunction.Notequal);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.Enable(EnableCap.Normalize);
        }
        internal void Render()
        {   //TODO: Render
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadIdentity();
            var scaleMatrix = MatrixProvider.ScaleMatrix(_scale);
            var translateMatrix = MatrixProvider.TranslateMatrix(_x / 720, -_y / 450, 0);
            var rotate = MatrixProvider.RotateXMatrix(_alphaX) * MatrixProvider.RotateYMatrix(_alphaY) * MatrixProvider.RotateZMatrix(_alphaZ);

            //TODO: Matrix multiplication
            M = scaleMatrix * translateMatrix * rotate * M;

            _scale = 1;
            _alphaX = 0;
            _alphaY = 0;
            _alphaZ = 0;
            _x = 0;
            _y = 0;
            //GL.MultMatrix(ref _projection);
            //GL.MultMatrix(ref M);




            //TODO: wywoływanie rysowania torusa    
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
                curve.DrawPolyline(M);
            }

            GL.Flush();

        }

        private void SetViewPort()
        {
            GL.Viewport(0, 0, 1440, 750);
            //  GL.Viewport(0, 0, 750, 750);
        }

        private void AddBezierCurveExecuted()
        {
            _bezierCurveCollection.Add(new BezierCurve(_selectedPointsCollection));
        }




        internal void SetCurrentCoordinate(int x, int y)
        {
            _x0 = x;
            _y0 = y;
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
            foreach (var p in _selectedPointsCollection)
            {
                p.X += dx;
                p.Y += dy;
                p.Z += dz;
            }
        }

        public void SelectPointByCursor()
        {
            //  _selectedPointsCollection.Clear();
            const double epsilon = 0.2;
            var c = _cursor.Coordinates;
            var temp = c;
            foreach (var p in _pointsCollection)
            {
                temp = new Vector4d(c.X - p.X, c.Y - p.Y, c.Z - p.Z, 0);

                if (temp.Length < epsilon)
                {
                    if (_selectedPointsCollection.Contains(p))
                    {
                        p.Selected = false;
                        _selectedPointsCollection.Remove(p);
                    }
                    else
                    {
                        p.Selected = true;
                        _selectedPointsCollection.Add(p);
                    }

                }
            }

        }

        public void SelectPointByMouse()
        {
            //  _selectedPointsCollection.Clear();
            const double epsilon = 20;
            Vector4d c = new Vector4d(_x0 - 1440.0 / 2.0, _y0 - 750.0 / 2.0, 0, 0);
            var temp = c;
            foreach (var p in _pointsCollection)
            {
                temp = new Vector4d(c.X - p.X_Window, -c.Y - p.Y_Window, 0, 0);

                if (temp.Length < epsilon)
                {
                    if (_selectedPointsCollection.Contains(p))
                    {
                        p.Selected = false;
                        _selectedPointsCollection.Remove(p);
                    }
                    else
                    {
                        p.Selected = true;
                        _selectedPointsCollection.Add(p);
                    }

                }

            }

        }
        public void DeleteSelectedPoints()
        {
            ObservableCollection<Point> _pointsToDelete = new ObservableCollection<Point>();
            foreach (var p in _pointsCollection)
            {
                if (p.Selected)
                {
                    _pointsToDelete.Add(p);
                }
            }
            foreach (var p in _pointsToDelete)
            {
                if (p.Selected)
                {
                    _pointsCollection.Remove(p);
                }
            }

        }
        public void AddPointByCursor()
        {
            PointsCollection.Add(new Point(_cursor.Coordinates.X, _cursor.Coordinates.Y, _cursor.Coordinates.Z));
        }
        public void MouseMoveTranslate(int x, int y)
        {
            _x = x - _x0;
            _y = y - _y0;
            _x0 = x;
            _y0 = y;

        }
        #endregion Public Methods



        //public void MouseMoveRotate(System.Windows.Forms.MouseEventArgs e)
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        var translation = Engine3D.Translation;
        //        translation.X += 0.4 * (e.X - PreviousMousePosition.X);
        //        translation.Y += 0.4 * (e.Y - PreviousMousePosition.Y);
        //        Engine3D.Translation = translation;
        //    }

        //    if (e.Button == MouseButtons.Right)
        //    {
        //        var rotation = Engine3D.Rotation;
        //        rotation.Y += 0.4 * (e.X - PreviousMousePosition.X);
        //        rotation.X += 0.4 * (e.Y - PreviousMousePosition.Y);
        //        if (rotation.X < 0)
        //            rotation.X = 0;
        //        else if (rotation.X > 90)
        //            rotation.X = 90;
        //        Engine3D.Rotation = rotation;
        //    }

        //    PreviousMousePosition = new Point(e.X, e.Y);
        //    Render();
        //}

    }
}

