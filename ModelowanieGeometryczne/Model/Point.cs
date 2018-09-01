using System;
//using System.Collections.Specialized;
//using System.Drawing;
//using System.Windows.Media.Media3D;
using ModelowanieGeometryczne.Helpers;
using ModelowanieGeometryczne.ViewModel;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using OpenTK.Platform;
//using Color = System.Windows.Media.Color;
//using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using System.Diagnostics;

namespace ModelowanieGeometryczne.Model
{
    [DebuggerDisplay("X = {X} Y= {Y} Z={Z}")]
    public class Point : ViewModelBase
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
            get { return _coordinates; }
            set { _coordinates = value; }
        }


        public Vector4d WindowCoordinates
        {
            get { return new Vector4d(X_Window, Y_Window, 0, 1); }
            set { }
        }
        #endregion Public Properties

        public Point(double x, double y, double z, string name = null)
        {
            _coordinates.X = x;
            _coordinates.Y = y;
            _coordinates.Z = z;
            _coordinates.W = 1;
            if (name == null)
            {
                _name = DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString();
            }
        }


        #region Private Methods
        #endregion Private Methods
        #region Public Methods

        public void Draw(Matrix4d transformacja, int size = 4, double red = 1, double green = 1, double blue=1)
        {
            //Matrix4d projekcja = MatrixProvider.ProjectionMatrix(100);
            GL.Enable(EnableCap.VertexProgramPointSize);
            GL.PointSize(size);
            GL.Begin(BeginMode.Points);
            

            if (_selected)
            {
                GL.Color3(0.0, 1.0, 0.0);
            }
            else
            {
                GL.Color3(red, green, blue);
            }
            
            Matrix4d projection = MatrixProvider.ProjectionMatrix();
            _windowCoordinates = projection.Multiply(transformacja.Multiply(_coordinates));
            GL.Vertex2(_windowCoordinates.X, _windowCoordinates.Y);
            _windowCoordinates.X = _windowCoordinates.X * 1440 / 2;
            _windowCoordinates.Y = _windowCoordinates.Y * 750 / 2;

            GL.End();

        }

        public void DrawStereoscopy(Matrix4d transformacja)
        {
            double StereoscopyMin = 1;
            GL.Enable(EnableCap.VertexProgramPointSize);
            GL.PointSize(3);
            GL.Begin(BeginMode.Points);


            //zmiana odleglosciu oczu
            Matrix4d projekcja = MatrixProvider.RightProjectionMatrix();
            var a = projekcja.Multiply(transformacja.Multiply(_coordinates));




            projekcja = MatrixProvider.LeftProjectionMatrix();
            var b = projekcja.Multiply(transformacja.Multiply(_coordinates));

            var c = a - b;
            c.X = c.X * 1440 / 2;
            c.Y = c.Y * 750 / 2;
            if (c.Length < StereoscopyMin)
            {
                GL.Color3(1.0, 0.0, 1.0);
                GL.Vertex2((a.X + b.X) / 2, (a.Y + b.Y) / 2);
            }
            else
            {
                GL.Color3(1.0, 0, 0);
                GL.Vertex2(a.X, a.Y);
                GL.Color3(0, 0, 1.0);
                GL.Vertex2(b.X, b.Y);
            }

            _windowCoordinates.X = (a.X / 2 + b.X / 2) * 1440 / 2;
            _windowCoordinates.Y = (a.Y / 2 + b.Y / 2) * 750 / 2;

            GL.End();
        }

        public Point Subtract(Point a)
        {
            return new Point(Coordinates.X - a.Coordinates.X, Coordinates.Y - a.Coordinates.Y, Coordinates.Z - a.Coordinates.Z);
        }
        public Point Add(Point a)
        {
            return new Point(Coordinates.X + a.Coordinates.X, Coordinates.Y + a.Coordinates.Y, Coordinates.Z + a.Coordinates.Z);
        }
        public double Length()
        {
            return Math.Sqrt(Coordinates.X * Coordinates.X + Coordinates.Y * Coordinates.Y + Coordinates.Z * Coordinates.Z);
        }
        public Point Multiply(double a)
        {
            return new Point(Coordinates.X * a, Coordinates.Y * a, Coordinates.Z * a);
        }

        #endregion Public Methods
        public static Point operator *(double a, Point v1)
        {
            return new Point(v1.X*a, v1.Y * a, v1.Z * a) ;
        }
        public static Point operator *(Point v1, double a)
        {
            return new Point(v1.X * a, v1.Y * a, v1.Z * a);
        }
        public static Point operator /( Point v1, double a)
        {
            return new Point(v1.X / a, v1.Y / a, v1.Z / a);
        }

        public static Point operator +(Point v1, Point v2)
        {
            return new Point(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Point operator + (Point v1, Vector3d v2)
        {
            return new Point(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Point operator -(Point v1, Point v2)
        {
            return new Point(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public Point Abs()
        {
            return new Point(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));
        }

        public Vector3d GetPointAsVector3D()
        {
            return new Vector3d(X, Y, Z);
        }

        public static Point GetPointFromVector3d(Vector3d v)
        {
            return new Point(v.X, v.Y, v.Z);
        }

        public static Point GetPointFromVector4d(Vector4d v)
        {
            return new Point(v.X, v.Y, v.Z);
        }

        public static Vector3d CrossProduct(Point vector1, Point vector2)
        {//Point as vector3d
            Vector3d result=new Vector3d();
            result.X = vector1.Y * vector2.Z - vector1.Z * vector2.Y;
            result.Y = vector1.Z * vector2.X - vector1.X * vector2.Z;
            result.Z = vector1.X * vector2.Y - vector1.Y * vector2.X;
            return result;
        }
    }
}

