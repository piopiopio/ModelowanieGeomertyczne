using System;
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
        #endregion Private Fields

        #region Public Properties

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
                var scaleMatrix = MatrixProvider.ScaleMatrix(_scale);
                M = M * scaleMatrix;
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
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            //    CreateProjectionMatrices();


            GL.LoadIdentity();
            //GL.MultMatrix(ref M);

            //Skasowac
            //_scale = 1;
            //_x = 0;
            //_y = 0;
            var scaleMatrix = MatrixProvider.ScaleMatrix(_scale);
            var translateMatrix = MatrixProvider.TranslateMatrix(_x / 50, -_y / 50, 0);
            var rotate = MatrixProvider.RotateXMatrix(_alphaX) * MatrixProvider.RotateYMatrix(_alphaY) * MatrixProvider.RotateZMatrix(_alphaZ);
            M = scaleMatrix * translateMatrix * rotate;
            //GL.MultMatrix(ref scaleMatrix);
            //GL.MultMatrix(ref translateMatrix);
            //GL.MultMatrix(ref rotate);
            GL.MultMatrix(ref M);
            DrawAxis();

            _torus.Draw();
            var projectionMatrix = MatrixProvider.ProjectionMatrix(200.0);
            GL.MultMatrix(ref projectionMatrix);
            GL.Begin(BeginMode.Lines);
            GL.Color4(0, 0, 255, 0.1);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(1, 2, 0);
            GL.Vertex3(1, 2, 0);
            GL.Vertex3(1, 0, 0);
            GL.End();

            GL.Flush();


        }

        private void SetViewPort()
        {
            GL.Viewport(0, 0, 1440, 750);
            // GL.MatrixMode(MatrixMode.Projection);
            //  GL.LoadIdentity();
        }

        //private void CreateProjectionMatrices()
        //{
        //    GL.MatrixMode(MatrixMode.Modelview);
        //    GL.LoadIdentity();

        //    InitializeLights();

        //    //// Move the box to the center of the control 
        //    //GL.Translate(0, -0.5, 0);
        //    //// User transformations
        //    //GL.Rotate(rotation.X, 1, 0, 0);
        //    //GL.Rotate(rotation.Y, 0, 1, 0);
        //    double scale = 0.1;
        //    GL.Scale(scale, scale, scale);
        //    //// Enable transforming the box around its center
        //    //GL.Translate(-_lookAt.X, -_lookAt.Y, -_lookAt.Z);

        //    //GL.Translate(Translation.X, 0, Translation.Y);
        //}

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
            // GL.PushMatrix();
            //GL.LoadIdentity();

            //  GL.Translate(-0.9, -0.9, 0.0);
            //GL.Rotate(Rotation.X, 1.0, 0.0, 0.0);
            //GL.Rotate(Rotation.Y, 0.0, 1.0, 0.0);

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
            // GL.PopMatrix();
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

            var translateMatrix = MatrixProvider.TranslateMatrix(_x / 50, -_y / 50, 0);
            M = M * translateMatrix;
        }
        #endregion Public Methods

        public Scene()
        {
            M = Matrix4d.Identity;
           // DefineDrawingMode();
            _scale = 0.1;
            _x = 0;
            _y = 0;
            _alphaX = 0;
            _alphaY = 0;
            _alphaZ = 0;
            Torus = new Torus();

        }





        



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

