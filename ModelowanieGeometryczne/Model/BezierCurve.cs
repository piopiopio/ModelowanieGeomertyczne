using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ModelowanieGeometryczne.Helpers;
using ModelowanieGeometryczne.ViewModel;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;

namespace ModelowanieGeometryczne.Model
{
    public class BezierCurve:ViewModelBase

{

    private ObservableCollection<Point> _pointsCollection = new ObservableCollection<Point>();
    private Matrix4d projekcja = MatrixProvider.ProjectionMatrix();
    private static int CurveNumber = 0;
    private string _name;
    private bool _polylineEnabled = true;

    #region Private Methods

    #endregion Private Methods

    #region Public Properties

    public ObservableCollection<Point> Points
    {
        get { return _pointsCollection; }
    }

    #endregion Public Properties

    #region Private Methods

    #endregion Private Methods

    #region Public Methods

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

    public void DrawCurve(Matrix4d transformacja, double divisions)
    {
        GL.Begin(BeginMode.Lines);
        GL.Color3(1.0, 1.0, 1.0);
        int j = 0;
        ObservableCollection<Point> temp= new ObservableCollection<Point>();
        foreach (var p in _pointsCollection)
        {
            j++;
            temp.Add(p);
            if (j%4 == 0 || j == _pointsCollection.Count)
            {
                var point = Casteljeu(temp,0);
                var windowCoordinates = projekcja.Multiply(transformacja.Multiply(point));
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
    public void Draw(Matrix4d transformacja)
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

    #endregion Public Methods
}
}

