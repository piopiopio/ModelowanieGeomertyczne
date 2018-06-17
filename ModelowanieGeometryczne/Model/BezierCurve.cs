using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModelowanieGeometryczne.Helpers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Media;

namespace ModelowanieGeometryczne.Model
{
    public class BezierCurve : Curve
    {
        #region Private Methods
        #endregion Private Methods

        #region Public Properties
        #endregion Public Properties

        #region Private Methods
        #endregion Private Methods

        #region Public Methods

        public BezierCurve(IEnumerable<Point> points, string name=null)
        {
            CurveType = "C0";
            PointsCollection = new ObservableCollection<Point>(points);
            if (Name == null)
            {
                Name = "Bezier curve number " + CurveNumber + " type: " + CurveType;
            }
            else
            {
                Name = name;
            }
        }

        public override void DrawCurve(Matrix4d transformacja)
        {
            DrawBezierCurve(transformacja, PointsCollection);
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
         

        public override void DrawCurveStereoscopy(Matrix4d transformacja)
        {
            DrawBezierCurveStereoscopy(transformacja, PointsCollection);
        }

        public override void DrawPolylineStereoscopy(Matrix4d transformacja)
        {
            if (PolylineEnabled)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
                GL.Begin(BeginMode.Lines);
                GL.Color3(0.6, 0.0, 0.0);

                for (int i = 0; i < PointsCollection.Count - 1; i++)
                {
                    var windowCoordinates =
                        projekcjaRight.Multiply(transformacja.Multiply(PointsCollection[i].Coordinates));
                    GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                    windowCoordinates =
                        projekcjaRight.Multiply(transformacja.Multiply(PointsCollection[i + 1].Coordinates));
                    GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                }



                GL.Color3(0.0, 0.0, 0.6);

                for (int i = 0; i < PointsCollection.Count - 1; i++)
                {
                    var windowCoordinates =
                        projekcjaLeft.Multiply(transformacja.Multiply(PointsCollection[i].Coordinates));
                    GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                    windowCoordinates =
                        projekcjaLeft.Multiply(transformacja.Multiply(PointsCollection[i + 1].Coordinates));
                    GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                }


                GL.End();

            }
        }



        #endregion Public Methods
    }
}