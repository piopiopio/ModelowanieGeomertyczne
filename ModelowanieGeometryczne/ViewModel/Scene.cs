using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Navigation;
using ModelowanieGeometryczne.Helpers;
using ModelowanieGeometryczne.Model;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ModelowanieGeometryczne.ViewModel
{
    public class Scene : ViewModelBase
    {
        public event PropertyChangedEventHandler RefreshScene;

        #region Private Fields
        private Torus _torus;
        private double _height;
        private double _width;
        private double _x, _y, _x0, _y0, _alphaX, _alphaY, _alphaZ;
        private double _scale;
        private Matrix4d M;
        private Matrix4d _projection;
        private bool _stereoscopy;
        private ObservableCollection<Point> _pointsCollection;// = new ObservableCollection<Point>();
        //private List<Point> _pointsCollection = new List<Point>();



        #endregion Private Fields

        #region Public Properties

        public ObservableCollection<Point> PointsCollection 
        {
            get { return _pointsCollection;}
            set
            {
                _pointsCollection=value;
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

        public Scene()
        {
            M = Matrix4d.Identity;
            // DefineDrawingMode();
            _scale = 0.1;
            _x = -2*700;
            _y = 2*400;
            _alphaX = 0;
            _alphaY = 0;
            _alphaZ = 0;
            M = Matrix4d.Identity;
            Torus = new Torus();
           // _projection = MatrixProvider.ProjectionMatrix(100);
            //M = _projection;

            PointsCollection=new ObservableCollection<Point>();
            PointsCollection.Add(new Point(20,30,0));
            
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
            var translateMatrix = MatrixProvider.TranslateMatrix(_x / 700, -_y / 400, 0);
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
           
            


        
            if (Stereoscopy)
            {
               _torus.DrawStereoscopy(M);
               
            }
            else
            {
                _torus.Draw(M);
              
            }
        



            GL.Flush();

        }

        private void SetViewPort()
        {
            GL.Viewport(0, 0, 1440, 750);
          //  GL.Viewport(0, 0, 750, 750);
        }

        private void InitializeLights()
        {
            GL.Light(LightName.Light0, LightParameter.Ambient, new[] { 0.2f, 0.2f, 0.2f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Diffuse, new[] { 0.3f, 0.3f, 0.3f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Specular, new[] { 0.8f, 0.8f, 0.8f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Position, new[] { 0.0f, 5.0f, 10.0f, 1.0f });
            GL.Enable(EnableCap.Light0);
        }

        private void DrawAxis()
        {

            GL.Begin(BeginMode.Lines);

            // draw line for x axis
            GL.Color3(1.0, 0.0, 0.0);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(1, 0.0, 0.0);

            GL.Vertex3(1.1, 0.0, 0.0);
            GL.Vertex3(1.2, -0.1, 0.0);
            GL.Vertex3(1.1, -0.1, 0.0);
            GL.Vertex3(1.2, 0.0, 0.0);

            // draw line for z axis
            GL.Color3(0.0, 1.0, 0.0);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(0.0, 1, 0.0);

            GL.Vertex3(0.0, 1.1, 0);
            GL.Vertex3(0.0, 1.2, 0);
            GL.Vertex3(0.1, 1.3, 0);
            GL.Vertex3(0.0, 1.2, 0);
            GL.Vertex3(-0.1, 1.3, 0);
            GL.Vertex3(0.0, 1.2, 0);

            // draw line for y axis
            GL.Color3(0.0, 0.0, 1.0);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(0.0, 0.0, 1.0);

            GL.Vertex3(0.2, 0.0, 1);
            GL.Vertex3(0.1, 0.0, 1);
            GL.Vertex3(0.1, 0.0, 1);
            GL.Vertex3(0.2, 0.1, 1);
            GL.Vertex3(0.2, 0.1, 1);
            GL.Vertex3(0.1, 0.1, 1);

            GL.End();

        }

        internal void SetCurrentCoordinate(int x, int y)
        {
            _x0 = x;
            _y0 = y;
        }

        #endregion Private Methods
        #region Public Methods
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

