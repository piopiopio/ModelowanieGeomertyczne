using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ModelowanieGeometryczne.Helpers;
using ModelowanieGeometryczne.ViewModel;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using System;
using System.Windows.Media.Imaging;






namespace ModelowanieGeometryczne.Model
{
    public class BezierCurve : ViewModelBase
    {
        public event PropertyChangedEventHandler RefreshScene;
        private ObservableCollection<Point> _pointsCollection = new ObservableCollection<Point>();
        private Matrix4d projekcja = MatrixProvider.ProjectionMatrix();
        private Matrix4d projekcjaLeft = MatrixProvider.LeftProjectionMatrix();
        private Matrix4d projekcjaRight = MatrixProvider.RightProjectionMatrix();
        private static int CurveNumber = 0;
        private string _name;
        private ICommand _removePoints;
        private bool _polylineEnabled = true;
        private bool _selected;

        #region Private Methods

        private void Refresh()
        {
            if (RefreshScene != null)
                RefreshScene(this, new PropertyChangedEventArgs("RefreshScene"));
        }

        #endregion Private Methods

        #region Public Properties

        public ICommand RemovePointsCommand { get { return _removePoints ?? (_removePoints = new ActionCommand(RemoveSelectedPoints)); } }


        private void RemoveSelectedPoints()
        {
            var temp = _pointsCollection.Where(c => c.Selected).ToList();

            foreach (var point in temp)
            {
                _pointsCollection.Remove(point);
            }

            Refresh();
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

        public ObservableCollection<Point> Points
        {
            get { return _pointsCollection; }
        }

        #endregion Public Properties

        #region Private Methods

        #endregion Private Methods

        #region Public Methods

        public void AddPoint(Point point)
        {
            _pointsCollection.Add(point);
        }
        public bool PolylineEnabled
        {
            get { return _polylineEnabled; }
            set
            {
                _polylineEnabled = value;
                // OnPropertyChanged("PolylineEnabled");     
            }
        }

        public string Name
        {
            get { return (_name); }
            set { _name = value; }
        }

        public BezierCurve(IEnumerable<Point> points)
        {

            _pointsCollection = new ObservableCollection<Point>(points);
            CurveNumber++;
            _name = "Bezier curve number " + CurveNumber;
        }

        private Vector4d Casteljeu(ObservableCollection<Point> points, double t)
        {
            int counter = points.Count;
            while (points.Count < 4)
                points.Add(points.Last());
            var xValues = new Vector4d(points[0].X, points[1].X, points[2].X, points[3].X);
            var yValues = new Vector4d(points[0].Y, points[1].Y, points[2].Y, points[3].Y);
            var zValues = new Vector4d(0, 0, 0, 0);

            for (int i = 0; i < counter; i++)
            {
                xValues.X = xValues.X * (1 - t) + (xValues.Y * t);
                xValues.Y = xValues.Y * (1 - t) + (xValues.Z * t);
                xValues.Z = xValues.Z * (1 - t) + (xValues.W * t);

                yValues.X = yValues.X * (1 - t) + (yValues.Y * t);
                yValues.Y = yValues.Y * (1 - t) + (yValues.Z * t);
                yValues.Z = yValues.Z * (1 - t) + (yValues.W * t);

                zValues.X = zValues.X * (1 - t) + (zValues.Y * t);
                zValues.Y = zValues.Y * (1 - t) + (zValues.Z * t);
                zValues.Z = zValues.Z * (1 - t) + (zValues.W * t);
            }

            return new Vector4d(xValues.X, yValues.X, zValues.X, 1);
        }

        public void DrawCurve(Matrix4d transformacja)
        {
            GL.Begin(BeginMode.Lines);
            GL.Color3(1.0, 1.0, 1.0);
            int j = 0;
            ObservableCollection<Point> temp = new ObservableCollection<Point>();
            foreach (var p in _pointsCollection)
            {
                j++;
                temp.Add(p);
                if (j % 4 == 0 || p == _pointsCollection.Last())
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
                    var windowCoordinates = projekcja.Multiply(transformacja.Multiply(point));
                    double divisions = 1 / length;
                    for (double t = divisions / 2; t <= 1; t += divisions / 2)
                    {
                        point = Casteljeu(temp, t);
                        //if (point == null) continue;
                        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                        windowCoordinates = projekcja.Multiply(transformacja.Multiply(point));
                        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                    }
                    temp.Clear();
                    temp.Add(p);
                    j = 1;
                }


            }

            GL.End();
        }
        public void DrawPolyline(Matrix4d transformacja)
        {
            GL.Begin(BeginMode.Lines);
            GL.Color3(1.0, 1.0, 1.0);
            if (_polylineEnabled)
            {
                for (int i = 0; i < _pointsCollection.Count - 1; i++)
                {
                    var windowCoordinates = projekcja.Multiply(transformacja.Multiply(_pointsCollection[i].Coordinates));
                    GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                    windowCoordinates = projekcja.Multiply(transformacja.Multiply(_pointsCollection[i + 1].Coordinates));
                    GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                }
            }
            GL.End();
        }

        public void DrawPolylineStereoscopy(Matrix4d transformacja)
        {   ////TODO: Nie działa, bezsensu zasadza działania proby !!!!!
        //    //GL.Begin(BeginMode.Lines);
        //    //GL.Color3(0.0, 0.0, 1.0);
        //    //ObservableCollection<Vector4d> FirstCurveVertices = new ObservableCollection<Vector4d>();
        //    //if (_polylineEnabled)
        //    //{
        //    //    for (int i = 0; i < _pointsCollection.Count - 1; i++)
        //    //    {
        //    //        Vector4d windowCoordinates = projekcjaRight.Multiply(transformacja.Multiply(_pointsCollection[i].Coordinates));
        //    //        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
        //    //        FirstCurveVertices.Add(windowCoordinates);
        //    //        windowCoordinates = projekcjaRight.Multiply(transformacja.Multiply(_pointsCollection[i + 1].Coordinates));
        //    //        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
        //    //    }
        //    //    //TODO: add last one: FirstCurveVertices.Add(windowCoordinates)

        //    const int renderWidth = 1440;
        //    const int renderHeight = 750;
        //    GL.Begin(BeginMode.Lines);
        //    GL.Color3(0.6, 0, 0);

        //    // TODO: zmiana odleglosciu oczu
        //    Matrix4d projekcja = projekcjaRight;
        //    for (int i = 0; i < _pointsCollection.Count - 1; i++)
        //    {
        //        //var avertex = transformacja.Multiply(_verticesList[relations.Item1]);
        //        //var avertex2 = transformacja.Multiply(_verticesList[relations.Item2]);
        //        //var dx = projekcja.M13 * avertex.Z;
        //        //var dx2 = projekcja.M13 * avertex2.Z;
        //        //var vertex = projekcja.Multiply(avertex);
        //        //var vertex2 = projekcja.Multiply(avertex2);
        //        //GL.Vertex2(vertex.X, vertex.Y);
        //        //GL.Vertex2(vertex2.X,vertex2.Y);

        //            Vector4d windowCoordinates = projekcja.Multiply(transformacja.Multiply(_pointsCollection[i].Coordinates));
        //            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
        //            windowCoordinates = projekcja.Multiply(transformacja.Multiply(_pointsCollection[i + 1].Coordinates));
        //            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
        //    }
        //    GL.End();

        //    Bitmap bmp1 = new Bitmap(renderWidth, renderHeight);
        //    System.Drawing.Imaging.BitmapData dat = bmp1.LockBits(new Rectangle(0, 0, renderWidth, renderHeight), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
        //    GL.ReadPixels(0, 0, renderWidth, renderHeight, PixelFormat.Bgra, PixelType.UnsignedByte, dat.Scan0);
        //    bmp1.UnlockBits(dat);
        //    //bmp1.Save("D:\\ModelowanieGeometryczne\\a1.bmp", ImageFormat.Bmp);

        //    GL.Clear(ClearBufferMask.ColorBufferBit);


        //    GL.Begin(BeginMode.Lines);
        //    GL.Color3(0, 0, 0.9);
        //    //todo : zmniejszyc e zwiekszyc r
        //    //TODO: Zrobić stałą wartosc
        //    for (int i = 0; i < _pointsCollection.Count - 1; i++)
        //    {

        //        //var avertex = transformacja.Multiply(_verticesList[relations.Item1]);
        //        //var avertex2 = transformacja.Multiply(_verticesList[relations.Item2]);
        //        //var dx = projekcja.M13 * avertex.Z;
        //        //var dx2 = projekcja.M13 * avertex2.Z;
        //        //var vertex = projekcja.Multiply(avertex);
        //        //var vertex2 = projekcja.Multiply(avertex2);
        //        //GL.Vertex2(vertex.X, vertex.Y);
        //        //GL.Vertex2(vertex2.X, vertex2.Y);
        //            Vector4d windowCoordinates = projekcja.Multiply(transformacja.Multiply(_pointsCollection[i].Coordinates));
        //            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
        //            windowCoordinates = projekcja.Multiply(transformacja.Multiply(_pointsCollection[i + 1].Coordinates));
        //            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);

        //    }
        //    GL.End();

        //    Bitmap bmp2 = new Bitmap(renderWidth, renderHeight);
        //    System.Drawing.Imaging.BitmapData dat2 = bmp2.LockBits(new Rectangle(0, 0, renderWidth, renderHeight), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
        //    GL.ReadPixels(0, 0, renderWidth, renderHeight, PixelFormat.Bgra, PixelType.UnsignedByte, dat2.Scan0);
        //    bmp2.UnlockBits(dat2);
        //   // bmp2.Save("D:\\ModelowanieGeometryczne\\a2.bmp", ImageFormat.Bmp);
            
        //   GL.Clear(ClearBufferMask.ColorBufferBit);

        //   Bitmap bmp3=bmp2;

        //   Color temp, pixeltemp, result;

        //   ////Działa dla dowolnego koloru torusa 1 i torusa 2

        //   for (int i = 0; i < renderWidth; i++)
        //   {
        //       for (int j = 0; j < renderHeight; j++)
        //       {
        //           temp = bmp1.GetPixel(i, j);
        //           if (temp.R == 0 && temp.G == 0 && temp.B == 0)
        //           {
       
        //           }
        //           else
        //           {
        //               //TODO: Mieszanie koloru
        //               pixeltemp = bmp3.GetPixel(i, j);
        //               result = Color.FromArgb(Math.Min(temp.R + pixeltemp.R, 255), Math.Min(temp.G + pixeltemp.G, 255), Math.Min(temp.B + pixeltemp.B, 255));
        //               bmp3.SetPixel(i, j, result);
        //           }
        //       }
        //   }

 

        //   dat2 = bmp2.LockBits(new Rectangle(0, 0, renderWidth, renderHeight), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
        //   GL.DrawPixels(renderWidth, renderHeight, PixelFormat.Bgra, PixelType.UnsignedByte, dat2.Scan0);
        //   bmp2.UnlockBits(dat2);
            }

            //if (_polylineEnabled)
            //{
            //    for (int i = 0; i < _pointsCollection.Count - 1; i++)
            //    {
            //        GL.Color3(1.0, 0.0, 0.0);

            //        var windowCoordinates = projekcjaLeft.Multiply(transformacja.Multiply(_pointsCollection[i].Coordinates));

            //        foreach (var v in FirstCurveVertices)
            //        {
            //            Vector4d a = v - windowCoordinates;
            //            if (a.Length < 0.01)
            //            {
            //                GL.Color3(1.0, 0.0, 1.0);
            //            }

            //        }

            //        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
            //        windowCoordinates = projekcjaLeft.Multiply(transformacja.Multiply(_pointsCollection[i + 1].Coordinates));
            //        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
            //    }
            //}

            
        


        internal void RemovePoints(List<Point> points)
        {
            foreach (var point in points)
            {
                if (_pointsCollection.Contains(point))
                {
                    _pointsCollection.Remove(point);
                }
            }
        }

        public void DrawCurveStereoscopy(Matrix4d transformacja)
        {
            //ObservableCollection<Vector4d> FirstCurveVertices = new ObservableCollection<Vector4d>();
            //GL.Begin(BeginMode.Lines);
            //GL.Color3(1.0, 0.0, 0.0);
            //int j = 0;
            //ObservableCollection<Point> temp = new ObservableCollection<Point>();
            //foreach (var p in _pointsCollection)
            //{
            //    j++;
            //    temp.Add(p);
            //    if (j % 4 == 0 || p == _pointsCollection.Last())
            //    {

            //        double length = 0;
            //        for (int i = 0; i < j - 1; i++)
            //        {
            //            Vector4d a = projekcja.Multiply(transformacja.Multiply(temp[i + 1])) - projekcja.Multiply(transformacja.Multiply(temp[i]));
            //            a.X *= 1440;
            //            a.Y *= 750;
            //            length += a.Length;
            //        }
            //        double divisions = 1 / length;


            //        var point = Casteljeu(temp, 0);
            //        var windowCoordinates = projekcjaRight.Multiply(transformacja.Multiply(point));
            //        for (double t = divisions / 2; t <= 1; t += divisions / 2)
            //        {
            //            point = Casteljeu(temp, t);
            //            //if (point == null) continue;
            //            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
            //            FirstCurveVertices.Add(windowCoordinates);
            //            windowCoordinates = projekcjaRight.Multiply(transformacja.Multiply(point));
            //            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);

            //        }

            //        point = Casteljeu(temp, 0);
            //        windowCoordinates = projekcjaLeft.Multiply(transformacja.Multiply(point));
            //        for (double t = divisions / 2; t <= 1; t += divisions / 2)
            //        {
            //            GL.Color3(0.0, 0.0, 1.0);
            //            foreach (var v in FirstCurveVertices)
            //            {
            //                Vector4d a = v - windowCoordinates;
            //                if (a.Length < 0.01)
            //                {
            //                    GL.Color3(1.0, 0.0, 1.0);
            //                }

            //            }
            //            point = Casteljeu(temp, t);
            //            //if (point == null) continue;
            //            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);     
            //            windowCoordinates = projekcjaLeft.Multiply(transformacja.Multiply(point));
            //            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);

            //        }
            //        temp.Clear();
            //        temp.Add(p);
            //        j = 1;
            //    }


            //}

            //GL.End();
        }

        #endregion Public Methods


    }
}