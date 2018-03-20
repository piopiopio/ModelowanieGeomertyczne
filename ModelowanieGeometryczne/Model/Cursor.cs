using System;
using System.Collections.Generic;
using ModelowanieGeometryczne.Helpers;
using ModelowanieGeometryczne.ViewModel;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ModelowanieGeometryczne.Model
{
    public class Cursor : ViewModelBase
    {
        private Matrix4d _projection, _projectionLeft, _projectionRight;
        private double size = 0.5;
        private Vector4d _coordinates;
        private Vector4d _windowCoordinate;
        private List<Vector4d> vertices = new List<Vector4d>();
        #region Private Methods
        #endregion Private Methods
        #region Public Properties
        public string CursorCoordinates
        {
            get { return "Cursor coordinates: X:" + String.Format("{0:0.##}",_coordinates.X) + " Y:" + String.Format("{0:0.##}",_coordinates.Y) + " Z:" + String.Format("{0:0.##}",_coordinates.Z); }
            //get { return "Cursor coordinates: X:" + _coordinates.X + " Y:" + _coordinates.Y + " Z:" + _coordinates.Z; }
        }

        public string CursorWindowCoordinates
        {   //TODO: Hardcoded
            //Zmienić na parametryczne rozdzielczość okna
            get { return "Cursor window coordinates: X:" + String.Format("{0:0.##}",_windowCoordinate.X) + " Y:" + String.Format("{0:0.##}",_windowCoordinate.Y) + " Z:" + String.Format("{0:0.##}",_windowCoordinate.Z); }
        
       
        }
        #endregion Public Properties

        public Vector4d WindowCoordinate
        {
            get { return _windowCoordinate;}
            set
            {
                _windowCoordinate = value;
                OnPropertyChanged("CursorWindowCoordinates");
            }
        }
        public Vector4d Coordinates
        {
            get { return _coordinates; }
            set
            {
                _coordinates = value;
                CreateVertices();
                OnPropertyChanged("CursorCoordinates");
             
            }
        }
        #region Private Methods
        #endregion Private Methods
        #region Public Methods


        public Cursor()
        {
            _coordinates = new Vector4d(0, 0, 0, 1);
            _windowCoordinate = new Vector4d(0, 0, 0, 1);
            CreateVertices();

            _projection = MatrixProvider.ProjectionMatrix();
            _projectionLeft = MatrixProvider.LeftProjectionMatrix();
            _projectionRight = MatrixProvider.RightProjectionMatrix();
        }


        public void CreateVertices()
        {
            vertices.Clear();
            vertices.Add(new Vector4d(-size + _coordinates.X, _coordinates.Y, _coordinates.Z, 1));
            vertices.Add(new Vector4d(size + _coordinates.X, _coordinates.Y, _coordinates.Z, 1));
            vertices.Add(new Vector4d(_coordinates.X, -size + _coordinates.Y, _coordinates.Z, 1));
            vertices.Add(new Vector4d(_coordinates.X, size + _coordinates.Y, _coordinates.Z, 1));
            vertices.Add(new Vector4d(_coordinates.X, _coordinates.Y, -size + _coordinates.Z, 1));
            vertices.Add(new Vector4d(_coordinates.X, _coordinates.Y, size + _coordinates.Z, 1));
        }



        public void DrawStereoscopy(Matrix4d transformacja)
        {
            GL.Begin(BeginMode.Lines);

            GL.Color3(1.0, 0.0, 0.0);

            foreach (var v in vertices)
            {
                var vertex = transformacja.Multiply(v);
                GL.Vertex2(_projectionRight.Multiply(vertex).X, _projectionRight.Multiply(vertex).Y);

            }


            GL.Color3(0.0, 0.0, 1.0);
            foreach (var v in vertices)
            {
                var vertex = transformacja.Multiply(v);
                GL.Vertex2(_projectionLeft.Multiply(vertex).X, _projectionLeft.Multiply(vertex).Y);
            }


            GL.End();

            var temp = _projection.Multiply(transformacja.Multiply(_coordinates));
            WindowCoordinate = new Vector4d(temp.X * 720, temp.Y * 375, 0, 0);
        }

        public void Draw(Matrix4d transformacja)
        {
            GL.Begin(BeginMode.Lines);

            GL.Color3(1.0, 0.0, 0.0);
            var vertex = transformacja.Multiply(vertices[0]);
            GL.Vertex2(_projection.Multiply(vertex).X, _projection.Multiply(vertex).Y);
            vertex = transformacja.Multiply(vertices[1]);
            GL.Vertex2(_projection.Multiply(vertex).X, _projection.Multiply(vertex).Y);

            GL.Color3(0.0, 1.0, 0.0);
            vertex = transformacja.Multiply(vertices[2]);
            GL.Vertex2(_projection.Multiply(vertex).X, _projection.Multiply(vertex).Y);
            vertex = transformacja.Multiply(vertices[3]);
            GL.Vertex2(_projection.Multiply(vertex).X, _projection.Multiply(vertex).Y);

            GL.Color3(0.0, 0.0, 1.0);
            vertex = transformacja.Multiply(vertices[4]);
            GL.Vertex2(_projection.Multiply(vertex).X, _projection.Multiply(vertex).Y);
            vertex = transformacja.Multiply(vertices[5]);
            GL.Vertex2(_projection.Multiply(vertex).X, _projection.Multiply(vertex).Y);

            GL.End();
            var temp = _projection.Multiply(transformacja.Multiply(_coordinates));
            WindowCoordinate = new Vector4d(temp.X * 720, temp.Y * 375,0,0);


        }

        #endregion Public Methods
    }
}

