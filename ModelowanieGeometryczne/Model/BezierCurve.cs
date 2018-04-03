using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModelowanieGeometryczne.Helpers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using System;
using System.Drawing;
using System.Linq;

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

        public BezierCurve(IEnumerable<Point> points)
        {
            CurveType = "C0";
            PointsCollection = new ObservableCollection<Point>(points);
            Name = "Bezier curve number " + CurveNumber + " type: "+CurveType;
        }

        public override void DrawCurve(Matrix4d transformacja)
        {
            DrawBezierCurve(transformacja, PointsCollection);
        }

        public override void DrawCurveStereoscopy(Matrix4d transformacja)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
            GL.Begin(BeginMode.Lines);
            GL.Color3(0.6, 0.0, 0.0);
            int j = 0;

            ObservableCollection<Point> temp = new ObservableCollection<Point>();
            foreach (var p in PointsCollection)
            {
                j++;
                temp.Add(p);
                if (j % 4 == 0 || p == PointsCollection.Last())
                {
                    double length = 0;
                    for (int i = 0; i < j - 1; i++)
                    {
                        Vector4d a = projekcja.Multiply(transformacja.Multiply(temp[i + 1])) - projekcja.Multiply(transformacja.Multiply(temp[i]));
                        a.X *= 1440;
                        a.Y *= 750;
                        length += a.Length;
                    }
                    var point = Casteljeu(temp, 0);
                    var windowCoordinates = projekcjaRight.Multiply(transformacja.Multiply(point));
                    double divisions = 1 / length;

                    for (double t = divisions / 2; t <= 1; t += divisions / 2)
                    {
                        point = Casteljeu(temp, t);
                        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                        windowCoordinates = projekcjaRight.Multiply(transformacja.Multiply(point));
                        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                    }
                    temp.Clear();
                    temp.Add(p);
                    j = 1;
                }


            }

            GL.Color3(0.0, 0.0, 0.6);
            j = 0;
            ObservableCollection<Point> temp2 = new ObservableCollection<Point>();
            foreach (var p in PointsCollection)
            {
                j++;
                temp2.Add(p);
                if (j % 4 == 0 || p == PointsCollection.Last())
                {
                    double length = 0;
                    for (int i = 0; i < j - 1; i++)
                    {
                        Vector4d a = projekcja.Multiply(transformacja.Multiply(temp2[i + 1])) - projekcja.Multiply(transformacja.Multiply(temp2[i]));
                        a.X *= 1440;
                        a.Y *= 750;
                        length += a.Length;
                    }
                    var point = Casteljeu(temp2, 0);
                    var windowCoordinates = projekcjaLeft.Multiply(transformacja.Multiply(point));
                    double divisions = 1 / length;
                    for (double t = divisions / 2; t <= 1; t += divisions / 2)
                    {
                        point = Casteljeu(temp2, t);
                        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                        windowCoordinates = projekcjaLeft.Multiply(transformacja.Multiply(point));
                        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);


                    }
                    temp2.Clear();
                    temp2.Add(p);
                    j = 1;
                }
            }
            GL.End();
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
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
            GL.Begin(BeginMode.Lines);
            GL.Color3(0.6, 0.0, 0.0);
            if (PolylineEnabled)
            {
                for (int i = 0; i < PointsCollection.Count - 1; i++)
                {
                    var windowCoordinates = projekcjaRight.Multiply(transformacja.Multiply(PointsCollection[i].Coordinates));
                    GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                    windowCoordinates = projekcjaRight.Multiply(transformacja.Multiply(PointsCollection[i + 1].Coordinates));
                    GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                }
            }





          
            
            GL.Color3(0.0, 0.0, 0.6);
            if (PolylineEnabled)
            {
                for (int i = 0; i < PointsCollection.Count - 1; i++)
                {
                    var windowCoordinates = projekcjaLeft.Multiply(transformacja.Multiply(PointsCollection[i].Coordinates));
                    GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                    windowCoordinates = projekcjaLeft.Multiply(transformacja.Multiply(PointsCollection[i + 1].Coordinates));
                    GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                }
            }

            GL.End();



            





        }

        #endregion Public Methods
    }
}