using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ModelowanieGeometryczne.Helpers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using System.Windows.Shapes;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using System;
using System.Windows.Media.Imaging;
using System.Drawing;






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

            GL.End();


            Bitmap bmp1 = new Bitmap(RenderWidth, RenderHeight);
            System.Drawing.Imaging.BitmapData dat = bmp1.LockBits(new System.Drawing.Rectangle(0, 0, RenderWidth, RenderHeight), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            GL.ReadPixels(0, 0, RenderWidth, RenderHeight, PixelFormat.Bgra, PixelType.UnsignedByte, dat.Scan0);
            bmp1.UnlockBits(dat);
            bmp1.Save("D:\\ModelowanieGeometryczne\\a1.bmp", System.Drawing.Imaging.ImageFormat.Bmp);


            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Begin(BeginMode.Lines);
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
                        //if (point == null) continue;
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


            Bitmap bmp2 = new Bitmap(RenderWidth, RenderHeight);
            System.Drawing.Imaging.BitmapData dat2 = bmp2.LockBits(new System.Drawing.Rectangle(0, 0, RenderWidth, RenderHeight), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            GL.ReadPixels(0, 0, RenderWidth, RenderHeight, PixelFormat.Bgra, PixelType.UnsignedByte, dat2.Scan0);
            bmp2.UnlockBits(dat2);
            bmp2.Save("D:\\ModelowanieGeometryczne\\a2.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            Bitmap bmp3 = bmp2;
            Color temp3, pixeltemp, result;

            //Działa dla dowolnego koloru torusa 1 i torusa 2

            for (int i = 0; i < RenderWidth; i++)
            {
                for (int ji = 0; ji < RenderHeight; ji++)
                {
                    temp3 = bmp1.GetPixel(i, ji);
                    if (temp3.R == 0 && temp3.G == 0 && temp3.B == 0)
                    {

                    }
                    else
                    {
                        //TODO: Mieszanie koloru
                        pixeltemp = bmp3.GetPixel(i, ji);
                        result = Color.FromArgb(Math.Min(temp3.R + pixeltemp.R, 255), Math.Min(temp3.G + pixeltemp.G, 255), Math.Min(temp3.B + pixeltemp.B, 255));
                        bmp3.SetPixel(i, ji, result);
                    }
                }
            }





            dat2 = bmp2.LockBits(new System.Drawing.Rectangle(0, 0, RenderWidth, RenderHeight), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            GL.DrawPixels(RenderWidth, RenderHeight, PixelFormat.Bgra, PixelType.UnsignedByte, dat2.Scan0);
            bmp2.UnlockBits(dat2);

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
            GL.End();
            Bitmap bmp1 = new Bitmap(RenderWidth, RenderHeight);
            System.Drawing.Imaging.BitmapData dat = bmp1.LockBits(new System.Drawing.Rectangle(0, 0, RenderWidth, RenderHeight), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            GL.ReadPixels(0, 0, RenderWidth, RenderHeight, PixelFormat.Bgra, PixelType.UnsignedByte, dat.Scan0);
            bmp1.UnlockBits(dat);
            bmp1.Save("D:\\ModelowanieGeometryczne\\a1.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.Begin(BeginMode.Lines);
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


            Bitmap bmp2 = new Bitmap(RenderWidth, RenderHeight);
            System.Drawing.Imaging.BitmapData dat2 = bmp2.LockBits(new System.Drawing.Rectangle(0, 0, RenderWidth, RenderHeight), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            GL.ReadPixels(0, 0, RenderWidth, RenderHeight, PixelFormat.Bgra, PixelType.UnsignedByte, dat2.Scan0);
            bmp2.UnlockBits(dat2);
            bmp2.Save("D:\\ModelowanieGeometryczne\\a2.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            Bitmap bmp3 = bmp2;

            Color temp, pixeltemp, result;

            ////Działa dla dowolnego koloru torusa 1 i torusa 2

            for (int i = 0; i < RenderWidth; i++)
            {
                for (int j = 0; j < RenderHeight; j++)
                {
                    temp = bmp1.GetPixel(i, j);
                    if (temp.R == 0 && temp.G == 0 && temp.B == 0)
                    {

                    }
                    else
                    {
                        //TODO: Mieszanie koloru
                        pixeltemp = bmp3.GetPixel(i, j);
                        result = Color.FromArgb(Math.Min(temp.R + pixeltemp.R, 255), Math.Min(temp.G + pixeltemp.G, 255), Math.Min(temp.B + pixeltemp.B, 255));
                        bmp3.SetPixel(i, j, result);
                    }
                }
            }

            dat2 = bmp2.LockBits(new System.Drawing.Rectangle(0, 0, RenderWidth, RenderHeight), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            GL.DrawPixels(RenderWidth, RenderHeight, PixelFormat.Bgra, PixelType.UnsignedByte, dat2.Scan0);
            bmp2.UnlockBits(dat2);
        }

        #endregion Public Methods
    }
}