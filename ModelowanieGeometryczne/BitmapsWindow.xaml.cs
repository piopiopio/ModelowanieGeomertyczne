using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

        public BitmapsWindow(List<double[]> p)
        {
            PointsList = p;
            InitializeComponent();
        }

        private void CanvasRight_OnLoaded(object sender, RoutedEventArgs e)
        {
            var brush = new SolidColorBrush(Colors.Black);
            foreach (var item in PointsList)
            {
                Ellipse EllipsePoint = new Ellipse();
                EllipsePoint.Stroke = brush;
                EllipsePoint.Width = 3;
                EllipsePoint.Height = 3;
                EllipsePoint.StrokeThickness = 2;

                Canvas.SetLeft(EllipsePoint, BitmapSize * item[0]);
                Canvas.SetTop(EllipsePoint, BitmapSize * item[1]);
                CanvasLeft.Children.Add(EllipsePoint);

                EllipsePoint = new Ellipse();
                EllipsePoint.Stroke = brush;
                EllipsePoint.Width = 3;
                EllipsePoint.Height = 3;
                EllipsePoint.StrokeThickness = 2;

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
        }
    }
}
