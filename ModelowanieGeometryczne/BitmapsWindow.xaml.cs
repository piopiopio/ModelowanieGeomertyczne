using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace ModelowanieGeometryczne
{
    /// <summary>
    /// Interaction logic for BitmapsWindow.xaml
    /// </summary>
    public partial class BitmapsWindow : Window
    {
        //public BitmapsWindow()
        //{
        //    InitializeComponent();
        //}
        private const double BitmapSize = 400;
        public List<double[]> PointsList;
        public double[] SideDeteriminantPoint;

        public BitmapsWindow(List<double[]> p, double[] SideDeteriminant)
        {
            PointsList = p;
            InitializeComponent();
            SideDeteriminantPoint = SideDeteriminant;
        }

        private void CanvasRight_OnLoaded(object sender, RoutedEventArgs e)
        {
            //Ellipse EllipsePoint = new Ellipse();
            //EllipsePoint.Stroke = brush;
            //EllipsePoint.Width = 3;
            //EllipsePoint.Height = 3;
            //EllipsePoint.StrokeThickness = 2;

            //Canvas.SetLeft(EllipsePoint, BitmapSize * item[0]);
            //Canvas.SetTop(EllipsePoint, BitmapSize * item[1]);
            //CanvasLeft.Children.Add(EllipsePoint);
            /// 
            var brush = new SolidColorBrush(Colors.Black);
            foreach (var item in PointsList)
            {
                Ellipse EllipsePoint = new Ellipse();
                EllipsePoint.Stroke = brush;
                EllipsePoint.Width = 10;
                EllipsePoint.Height = 10;
                EllipsePoint.StrokeThickness = 5;

                Canvas.SetLeft(EllipsePoint, BitmapSize * item[0]);
                Canvas.SetTop(EllipsePoint, BitmapSize * item[1]);
                CanvasLeft.Children.Add(EllipsePoint);

                EllipsePoint = new Ellipse();
                EllipsePoint.Stroke = brush;
                EllipsePoint.Width = 10;
                EllipsePoint.Height = 10;
                EllipsePoint.StrokeThickness = 5;

                Canvas.SetLeft(EllipsePoint, BitmapSize * item[2]);
                Canvas.SetTop(EllipsePoint, BitmapSize * item[3]);
                CanvasRight.Children.Add(EllipsePoint);

                //Line L = new Line();
                //L.Stroke = new SolidColorBrush(Colors.Black);
                //L.X1 = 0;
                //L.X2 = 0;
                //L.Y1 = 0;
                //L.Y2 = 400;
                //L.StrokeThickness = 2;
                //CanvasRight.Children.Add(L);
            }



            //////Ellipse EllipsePoint1 = new Ellipse();
            //////EllipsePoint1.Stroke = brush;
            //////EllipsePoint1.Width = 10;
            //////EllipsePoint1.Height = 10;
            //////EllipsePoint1.StrokeThickness = 5;

            //////Canvas.SetLeft(EllipsePoint1, BitmapSize * SideDeteriminantPoint[0]);
            //////Canvas.SetTop(EllipsePoint1, BitmapSize * SideDeteriminantPoint[1]);
            //////CanvasLeft.Children.Add(EllipsePoint1);

            //////EllipsePoint1 = new Ellipse();
            //////EllipsePoint1.Stroke = brush;
            //////EllipsePoint1.Width = 10;
            //////EllipsePoint1.Height = 10;
            //////EllipsePoint1.StrokeThickness = 5;

            //////Canvas.SetLeft(EllipsePoint1, BitmapSize * SideDeteriminantPoint[2]);
            //////Canvas.SetTop(EllipsePoint1, BitmapSize * SideDeteriminantPoint[3]);
            //////CanvasRight.Children.Add(EllipsePoint1);

            //Line L;
            Rectangle R;
            //double epsilonToSnapBoder = 0.009;
            //for (int i = 0; i < PointsList.Count; i++)
            //{
            //R = new Rectangle();
            //R.StrokeThickness = 2; //Ustawić jako proporcjonalna do newton forward step
            //R.Width = 400;
            //R.Height = 400;
            //R.Stroke = brush;
            //CanvasLeft.Children.Add(R);
            //R = new Rectangle();
            //R.StrokeThickness = 1; //Ustawić jako proporcjonalna do newton forward step
            //R.Width = 400;
            //R.Height = 400;
            //R.Stroke = brush;
            //CanvasRight.Children.Add(R);

            //    //if (Math.Abs(PointsList[i][0]) >= epsilonToSnapBoder &&
            //    //    Math.Abs(PointsList[i][0] - 1) >= epsilonToSnapBoder &&
            //    //    Math.Abs(PointsList[i][1]) >= epsilonToSnapBoder &&
            //    //    Math.Abs(PointsList[i][1] - 1) >= epsilonToSnapBoder)
            //    //{
            //    L = new Line();
            //    L.Stroke = brush;
            //    L.StrokeThickness = 2;
            //    L.X1 = PointsList[i][0] * BitmapSize;
            //    L.Y1 = PointsList[i][1] * BitmapSize;

            //    //if (Math.Abs(PointsList[i][0]) >= epsilonToSnapBoder)// &&
            //    //    Math.Abs(PointsList[i][0] - 1) >= epsilonToSnapBoder &&
            //    //    Math.Abs(PointsList[i][1]) >= epsilonToSnapBoder &&
            //    //    Math.Abs(PointsList[i][1] - 1) >= epsilonToSnapBoder)
            //    if (Math.Abs(PointsList[i][0]) >= epsilonToSnapBoder &&
            //        Math.Abs(PointsList[i][0] - 1) >= epsilonToSnapBoder &&
            //        Math.Abs(PointsList[i][1]) >= epsilonToSnapBoder &&
            //        Math.Abs(PointsList[i][1] - 1) >= epsilonToSnapBoder)
            //    {
            //        if (i < PointsList.Count - 1)
            //        {
            //            L.X2 = PointsList[i + 1][0] * BitmapSize;
            //            L.Y2 = PointsList[i + 1][1] * BitmapSize;

            //        }

            //        else
            //        {
            //            L.X2 = PointsList[0][0] * BitmapSize;
            //            L.Y2 = PointsList[0][1] * BitmapSize;
            //        }

            //        CanvasLeft.Children.Add(L);
            //    }


            //else if (Math.Abs(PointsList[i][0]) < epsilonToSnapBoder)
            //{
            //    L.X2 = 0;
            //    L.Y2 = PointsList[i][1] * BitmapSize;
            //    brush = new SolidColorBrush(Colors.Aqua);
            //}

            //else if (Math.Abs(PointsList[i][0] - 1) < epsilonToSnapBoder)
            //{
            //    L.X2 = BitmapSize;
            //    L.Y2 = PointsList[i][1] * BitmapSize;
            //    brush = new SolidColorBrush(Colors.Beige);
            //}

            //else if (Math.Abs(PointsList[i][1]) < epsilonToSnapBoder)
            //{
            //    L.X2 = PointsList[i][0] * BitmapSize;
            //    L.Y2 = 0;
            //    brush = new SolidColorBrush(Colors.Chocolate);
            //}

            //else if (Math.Abs(PointsList[i][1]-1) < epsilonToSnapBoder)
            //{
            //    L.X2 = PointsList[i][0] * BitmapSize;
            //    L.Y2 = BitmapSize;
            //    brush = new SolidColorBrush(Colors.DarkBlue);
            //}

            // L.Stroke = brush;
            // CanvasLeft.Children.Add(L);
            //brush = new SolidColorBrush(Colors.Black);
            CanvasLeft.InvalidateVisual();

        }

        System.Drawing.Bitmap ConvertCanvasToBitmap(bool left)
        {
            //using (MemoryStream outStream = new MemoryStream())
            //{
            //    RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
            //        //(int)CanvasLeft.Width,
            //        400,
            //        // (int)CanvasLeft.Height,
            //        400,
            //        96d,
            //        96d,
            //        PixelFormats.Pbgra32);
            //    if (left)
            //    {
            //        renderBitmap.Render(CanvasLeft);
            //    }
            //    else
            //    {
            //        renderBitmap.Render(CanvasRight);
            //    }

            //    var temp = new WriteableBitmap(renderBitmap);
            //    BitmapEncoder enc = new BmpBitmapEncoder();
            //    enc.Frames.Add(BitmapFrame.Create((BitmapSource)temp));
            //    enc.Save(outStream);
            //    System.Drawing.Bitmap bmp;
            //    bmp = new System.Drawing.Bitmap(outStream);
            //    var c1 = bmp.GetPixel(0, 0);
            //    var c2 = bmp.GetPixel(10, 10);
            //            bmp.Save("D:\\copy back to c\\a.bmp");
            //    return bmp;
            //}
            var width = CanvasLeft.ActualWidth;
            var height = CanvasLeft.ActualHeight;
            var mXdpi = 96;
            var mYdpi = 96;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)width, (int)height, mXdpi, mYdpi, System.Windows.Media.PixelFormats.Default);
            rtb.Render(CanvasLeft);

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            MemoryStream stream = new MemoryStream();
            pngEncoder.Save(stream);
            Bitmap bitmap = new Bitmap(stream);
            bitmap.Save("D:\\copy back to c\\a.bmp");
            //using (var fs = System.IO.File.OpenWrite("D:\\copy back to c\\fs.png"))
            //{
            //    pngEncoder.Save(fs);
            //}



            //RenderTargetBitmap bmpRen = new RenderTargetBitmap(width, height, mXdpi, mYdpi, PixelFormats.Default);
            //bmpRen.Render(CanvasLeft);

            //MemoryStream stream = new MemoryStream();
            //BitmapEncoder encoder = new BmpBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(bmpRen));
            //encoder.Save(stream);

            //Bitmap bitmap = new Bitmap(stream);
            //bitmap.Save("D:\\copy back to c\\a.bmp");

            return bitmap;
        }

        //void floodFill(System.Drawing.Bitmap Bitmap, int positionX, int positionY, int colorToChange, int newColor)
        //{
        //    if (positionY <= (BitmapSize - 1) && positionY >= 0 && positionX <= (BitmapSize - 1) && positionX >= 0)
        //    {
        //        if (Bitmap.GetPixel(positionX, positionY).A == colorToChange)
        //        {
        //            Bitmap.SetPixel(positionX, positionY, System.Drawing.Color.FromArgb((byte)newColor, 0, 0, 0));
        //            floodFill(Bitmap, positionX - 1, positionY, 255, 0);
        //            floodFill(Bitmap, positionX, positionY - 1, 255, 0);
        //            floodFill(Bitmap, positionX + 1, positionY, 255, 0);
        //            floodFill(Bitmap, positionX, positionY + 1, 255, 0);
        //        }
        //        else
        //        {
        //            //MessageBox.Show("czarny");
        //        }
        //    }

        //}


        bool getPixelFromLimitedBitmab(System.Drawing.Bitmap Bitmap, int positionX, int positionY, int colorToChange)
        {
            if (positionY <= (BitmapSize - 1) && positionY >= 0 && positionX <= (BitmapSize - 1) &&
                positionX >= 0)
            {
                if (Bitmap.GetPixel(positionX, positionY).A == colorToChange)
                {
                    return true;
                }
            }
            return false;

        }
        void floodFill(System.Drawing.Bitmap Bitmap, int positionX, int positionY, int colorToChange, int newColor)
        {
            var toFill = new List<int[]>();

            toFill.Add(new int[2] { positionX, positionY });

            while (toFill.Any())
            {
                var p = toFill[0];
                toFill.RemoveAt(0);
                Bitmap.SetPixel(p[0], p[1], System.Drawing.Color.FromArgb((byte)newColor, 0, 0, 0));

                //if (Bitmap.GetPixel(p[0]+1, p[1]).A == colorToChange && !toFill.Any(t => t[0]==(p[0]+1)))
                if (getPixelFromLimitedBitmab(Bitmap, p[0] + 1, p[1], colorToChange) && !toFill.Any(t => (t[0] == (p[0] + 1)) && (t[1] == p[1])))
                {
                    toFill.Add(new int[2] { p[0] + 1, p[1] });
                }
                if (getPixelFromLimitedBitmab(Bitmap, p[0] - 1, p[1], colorToChange) && !toFill.Any(t => (t[0] == (p[0] - 1)) && (t[1] == p[1])))
                {
                    toFill.Add(new int[2] { p[0] - 1, p[1] });
                }
                if (getPixelFromLimitedBitmab(Bitmap, p[0], p[1] + 1, colorToChange) && !toFill.Any(t => (t[0] == p[0]) && (t[1] == (p[1] + 1))))
                {
                    toFill.Add(new int[2] { p[0], p[1] + 1 });
                }
                if (getPixelFromLimitedBitmab(Bitmap, p[0], p[1] - 1, colorToChange) && !toFill.Any(t => (t[0] == p[0]) && (t[1] == (p[1] - 1))))
                {
                    toFill.Add(new int[2] { p[0], p[1] - 1 });
                }
            }



        }

        // }
        //L = new Line();
        //L.X1 = PointsList[PointsList.Count-1][0] * BitmapSize;
        //L.Y1 = PointsList[PointsList.Count-1][1] * BitmapSize;
        //L.X2 = PointsList[0][0] * BitmapSize;
        //L.Y2 = PointsList[0][1] * BitmapSize;
        //CanvasLeft.Children.Add(L);

        //}
        private void CanvasLeft_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Drawing.Bitmap Bitmap = ConvertCanvasToBitmap(true);
            //Bitmap.SetPixel((int)(BitmapSize * SideDeteriminantPoint[0]), (int)(BitmapSize * SideDeteriminantPoint[1]), System.Drawing.Color.FromArgb(0, 0, 0, 0));
            floodFill(Bitmap, (int)(BitmapSize * SideDeteriminantPoint[0]), (int)(BitmapSize * SideDeteriminantPoint[1]), 0, 255);
            ImageBrush ib = new ImageBrush();
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(Bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );

            ib.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(Bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            CanvasLeft.Background = ib;
        }
    }
}
