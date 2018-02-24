using OpenTK.Graphics.OpenGL;

namespace ModelowanieGeometryczne.ViewModel
{
    public class Scene : ViewModelBase
    {
        #region Private Fields

        private double _height;
        private double _width;
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
        #endregion Public Properties
        #region Private Methods
        #endregion Private Methods
        #region Public Methods
        #endregion Public Methods

        public Scene()
        {
            DefineDrawingMode();
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
        internal void Render(double scale)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

          
        //    CreateProjectionMatrices();
            
          
            GL.LoadIdentity();
            GL.Scale(scale, scale, scale);
            DrawAxis();
            GL.Begin(BeginMode.Lines);
            GL.Color4(0, 0, 255, 0.1);
            GL.Vertex3(0,0, 0);
            GL.Vertex3(1, 2, 0);
            GL.Vertex3(1, 2, 0);
            GL.Vertex3(1, 0, 0);
            GL.End();
            
            GL.Flush();
        }

        private void SetViewPort()
        {
            GL.Viewport(0, 0, (int)_width, (int)_height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
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
    }
}

