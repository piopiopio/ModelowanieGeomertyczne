using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModelowanieGeometryczne.Helpers;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ModelowanieGeometryczne.Model
{
    public class BezierCurveC2 : Curve
    {
        private bool _isBernsteinBasis = false; 
        #region Private Methods
        #endregion Private Methods
        #region Public Properties

        public bool IsBernsteinBasis
        {
            get { return _isBernsteinBasis; }
            set { _isBernsteinBasis = value; }
        }
        #endregion Public Properties
        #region Private Methods
        #endregion Private Methods
        #region Public Methods

        public BezierCurveC2(IEnumerable<Point> points)
        {
            CurveType = "C2";
            PointsCollection = new ObservableCollection<Point>(points);
            Name = "Bezier curve number " + CurveNumber + " type: " + CurveType;
        }

        public override void DrawCurve(Matrix4d transformacja)
        {
            //throw new NotImplementedException();
        }
        public override void DrawCurveStereoscopy(Matrix4d transformacja)
        {
            throw new NotImplementedException();
        }

        public override void DrawPolyline(Matrix4d transformacja)
        {
            GL.Begin(BeginMode.Lines);
            GL.Color3(1.0, 1.0, 1.0);
            if (PolylineEnabled)
            {
                for (int i = 0; i < PointsCollection.Count - 1; i++)
                {
                    var windowCoordinates = projekcja.Multiply(transformacja.Multiply(PointsCollection[i].Coordinates));
                    GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                    windowCoordinates = projekcja.Multiply(transformacja.Multiply(PointsCollection[i + 1].Coordinates));
                    GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                }
            }
            GL.End();
        }

        public override void DrawPolylineStereoscopy(Matrix4d transformacja)
        {
            throw new NotImplementedException();
        }
        #endregion Public Methods
    }
}

