using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Windows.Media.Media3D;
using ModelowanieGeometryczne.Helpers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using Color = System.Windows.Media.Color;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace ModelowanieGeometryczne.Model
{
    public class Point
    {
        private Vector4d _coordinates;
        private string _name;
        #region Private Methods
        #endregion Private Methods
        #region Public Properties

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public double X
        {
            get { return _coordinates.X; }
        }

        public double Y
        {
            get { return _coordinates.Y; }
        }

        public double Z
        {
            get { return _coordinates.Z; }
        }

        public Vector4d Coordinates
        {
            get { return _coordinates;}
            set { _coordinates = value; }
        }
        #endregion Public Properties

        public Point(double x, double y, double z)
        {
            _coordinates.X = x;
            _coordinates.Y = y;
            _coordinates.Z = z;
            _coordinates.W = 1;
            _name = DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString();
        }

        
        #region Private Methods
        #endregion Private Methods
        #region Public Methods

        public void Draw(Matrix4d transformacja)
        {
            //Matrix4d projekcja = MatrixProvider.ProjectionMatrix(100);
            GL.Begin(BeginMode.Points);
            GL.Color3(1.0, 1.0, 1.0);
            Matrix4d projekcja = MatrixProvider.ProjectionMatrix();
            var avertex = transformacja.Multiply(_coordinates);
            var vertex = projekcja.Multiply(avertex);
            GL.Vertex2(vertex.X, vertex.Y);
            GL.End();

        }

        public void DrawStereoscopy(Matrix4d transformacja)
        {

            GL.Begin(BeginMode.Points);
            GL.Color3(1.0, 0, 0);

            // TODO: zmiana odleglosciu oczu
            Matrix4d projekcja = MatrixProvider.RightProjectionMatrix();
            var avertex = transformacja.Multiply(_coordinates);
            var vertex = projekcja.Multiply(avertex);
            GL.Vertex2(vertex.X, vertex.Y);


            GL.Color3(0, 0, 1.0);
            projekcja = MatrixProvider.LeftProjectionMatrix();
            avertex = transformacja.Multiply(_coordinates);
            vertex = projekcja.Multiply(avertex);
            GL.Vertex2(vertex.X, vertex.Y);

            GL.End();
              
        }
        #endregion Public Methods
    }
}

