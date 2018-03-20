using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Windows.Media.Media3D;
using ModelowanieGeometryczne.Helpers;
using ModelowanieGeometryczne.ViewModel;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using Color = System.Windows.Media.Color;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace ModelowanieGeometryczne.Model
{
    public class Point:ViewModelBase
    {
        private Vector4d _coordinates;
        private Vector4d _windowCoordinates;
        private string _name;
        private bool _selected;
        #region Private Methods
        #endregion Private Methods
        #region Public Properties

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value; 
                OnPropertyChanged("Selected");
            }
        }

        public double X
        {
            get { return _coordinates.X; }
            set
            {
                _coordinates.X = value;
                OnPropertyChanged("X");
            }
        }

        public double Y
        {
            get { return _coordinates.Y; }
            set
            {
                _coordinates.Y = value;
                OnPropertyChanged("Y");
            }

        }

        public double Z
        {
            get { return _coordinates.Z; }
            set
            {
                _coordinates.Z = value;
                OnPropertyChanged("Z");
            }
        }

        public double X_Window
        {
            get { return _windowCoordinates.X; }
            set
            {
                _windowCoordinates.X = value;
                OnPropertyChanged("X");
            }
        }

        public double Y_Window
        {
            get { return _windowCoordinates.Y; }
            set
            {
                _windowCoordinates.Y = value;
                OnPropertyChanged("Y");
            }

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
            if (_selected)
            {
                GL.Color3(0.0,1.0,0.0);
            }
            else
            {
                GL.Color3(1.0, 1.0, 1.0);
            }
            Matrix4d projekcja = MatrixProvider.ProjectionMatrix();
            _windowCoordinates = projekcja.Multiply(transformacja.Multiply(_coordinates));
            GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);
            _windowCoordinates.X = _windowCoordinates.X*1440/2;
            _windowCoordinates.Y = _windowCoordinates.Y * 750 / 2;
            GL.End();

        }

        public void DrawStereoscopy(Matrix4d transformacja)
        {

            GL.Begin(BeginMode.Points);
            GL.Color3(1.0, 0, 0);

            // TODO: zmiana odleglosciu oczu
            Matrix4d projekcja = MatrixProvider.RightProjectionMatrix();
            _windowCoordinates = projekcja.Multiply(transformacja.Multiply(_coordinates));
            GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);


            GL.Color3(0, 0, 1.0);
            projekcja = MatrixProvider.LeftProjectionMatrix();
            var windowCoordinates2 = projekcja.Multiply(transformacja.Multiply(_coordinates));
            GL.Vertex2(windowCoordinates2.X, windowCoordinates2.Y);

            _windowCoordinates = _windowCoordinates/2 + windowCoordinates2/2;


            GL.End();
              
        }
        #endregion Public Methods
    }
}

