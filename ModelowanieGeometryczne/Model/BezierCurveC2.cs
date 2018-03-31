using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using ModelowanieGeometryczne.Helpers;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ModelowanieGeometryczne.Model
{
    public class BezierCurveC2 : Curve
    {
        private ObservableCollection<Point> _additionalPointsCollection = new ObservableCollection<Point>();
        private ObservableCollection<Point> _additionalPointsCollection2 = new ObservableCollection<Point>();
        private double[] _knots;
        private const int PolynomialDegree = 3;
        private bool _isBernsteinBasis = false;
        #region Private Methods
        #endregion Private Methods
        #region Public Properties

        public bool IsBernsteinBasis
        {
            get { return _isBernsteinBasis; }
            set
            {
                _isBernsteinBasis = value;
                Refresh();
            }
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

        private void CalculateAdditionalPoints()
        {
            _additionalPointsCollection.Clear();
            for (int i = 0; i < PointsCollection.Count - 1; i++)
            {
                _additionalPointsCollection.Add(new Point(PointsCollection[i].X + (PointsCollection[i + 1].X - PointsCollection[i].X) / 3, PointsCollection[i].Y + (PointsCollection[i + 1].Y - PointsCollection[i].Y) / 3, PointsCollection[i].Z + (PointsCollection[i + 1].Z - PointsCollection[i].Z) / 3));
                _additionalPointsCollection.Add(new Point((PointsCollection[i].X + 2 * (PointsCollection[i + 1].X - PointsCollection[i].X) / 3), (PointsCollection[i].Y + 2 * (PointsCollection[i + 1].Y - PointsCollection[i].Y) / 3), (PointsCollection[i].Z + 2 * (PointsCollection[i + 1].Z - PointsCollection[i].Z) / 3)));
            }

            _additionalPointsCollection2.Clear();

            int k = 1;
            _additionalPointsCollection2.Add(new Point(_additionalPointsCollection[k].X + (_additionalPointsCollection[k + 1].X - _additionalPointsCollection[k].X) / 2, _additionalPointsCollection[k].Y + (_additionalPointsCollection[k + 1].Y - _additionalPointsCollection[k].Y) / 2, _additionalPointsCollection[k].Z + (_additionalPointsCollection[k + 1].Z - _additionalPointsCollection[k].Z) / 2));
            _additionalPointsCollection2.Add(_additionalPointsCollection[k+1]);
            for (k = 3; k < _additionalPointsCollection.Count - 3; k+=2)
            {//TODO: Zrobić generowanie punktów
                _additionalPointsCollection2.Add(_additionalPointsCollection[k]);
                _additionalPointsCollection2.Add(new Point(_additionalPointsCollection[k].X + (_additionalPointsCollection[k + 1].X - _additionalPointsCollection[k].X) / 2, _additionalPointsCollection[k].Y + (_additionalPointsCollection[k + 1].Y - _additionalPointsCollection[k].Y) / 2, _additionalPointsCollection[k].Z + (_additionalPointsCollection[k + 1].Z - _additionalPointsCollection[k].Z) / 2));
                _additionalPointsCollection2.Add(_additionalPointsCollection[k+1]);
            }

            _additionalPointsCollection2.Add(_additionalPointsCollection[k]);
            _additionalPointsCollection2.Add(new Point(_additionalPointsCollection[k].X + (_additionalPointsCollection[k + 1].X - _additionalPointsCollection[k].X) / 2, _additionalPointsCollection[k].Y + (_additionalPointsCollection[k + 1].Y - _additionalPointsCollection[k].Y) / 2, _additionalPointsCollection[k].Z + (_additionalPointsCollection[k + 1].Z - _additionalPointsCollection[k].Z) / 2));

           // _additionalPointsCollection2.Add(_additionalPointsCollection[k - 1]);
           // _additionalPointsCollection2.Add(new Point(_additionalPointsCollection[k-1].X + (_additionalPointsCollection[k].X - _additionalPointsCollection[k-1].X) / 2, _additionalPointsCollection[k-1].Y + (_additionalPointsCollection[k].Y - _additionalPointsCollection[k-1].Y) / 2, _additionalPointsCollection[k-1].Z + (_additionalPointsCollection[k].Z - _additionalPointsCollection[k-1].Z) / 2));

        }
        private void SetSplineKnots()
        {
            _knots = new double[PointsCollection.Count + PolynomialDegree + 4];
            double interval = 1 / (double)(PointsCollection.Count + PolynomialDegree + 3);
            for (int i = 0; i < PointsCollection.Count + PolynomialDegree + 4; i++)
                _knots[i] = i * interval;
        }

        public override void DrawCurve(Matrix4d transformacja)
        {
            //GL.Begin(BeginMode.Points);
            //GL.Color3(1.0, 1.0, 1.0);

            SetSplineKnots();

            //TODO: adaptacyjny
            double divisions = 0.001;

            if (IsBernsteinBasis)
            {
                CalculateAdditionalPoints();
                foreach (var p in _additionalPointsCollection)
                {
                    p.Draw(transformacja, 4);
                }
                foreach (var p in _additionalPointsCollection2)
                {
                    p.Draw(transformacja, 4);
                }
                DrawBezierCurve(transformacja, _additionalPointsCollection2);
            }
            else
            {
                GL.Begin(BeginMode.Lines);
                GL.Color3(1.0, 1.0, 1.0);
                for (double t = 0; t < 1; t += divisions)
                {
                    if (t >= _knots[3] && t <= _knots[_knots.Length - PolynomialDegree - 4])
                    {
                        //TODO: Zrobić rysowanie liniami.
                        var u = BSplinePoint(t);
                        var v = BSplinePoint(t+divisions);

                        var windowCoordinates = projekcja.Multiply(transformacja.Multiply(u.Coordinates));
                        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);

                        windowCoordinates = projekcja.Multiply(transformacja.Multiply(v.Coordinates));
                        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                    }
                }
                GL.End();
            }

        }
        public static double GetNFunctionValue(double[] knots, int i, int n, double ti)
        {
            if (n < 0)
                return 0;
            if (n == 0)
            {
                if (ti >= knots[i] && ti < knots[i + 1])
                    return 1;
                return 0;
            }

            double a = (knots[i + n] - knots[i] != 0.0) ? (ti - knots[i]) / (knots[i + n] - knots[i]) : 0;
            double b = (knots[i + n + 1] - knots[i + 1] != 0.0) ? (knots[i + n + 1] - ti) / (knots[i + n + 1] - knots[i + 1]) : 0;
            return (a * GetNFunctionValue(knots, i, n - 1, ti)) + (b * GetNFunctionValue(knots, i + 1, n - 1, ti));
        }
        private Point BSplinePoint(double t)
        {
            Point sum = new Point(0, 0, 0);

            for (int i = 0; i < PointsCollection.Count; i++)
            {
                var N = GetNFunctionValue(_knots, i, PolynomialDegree, t);
                sum.X += PointsCollection[i].X * N;
                sum.Y += PointsCollection[i].Y * N;
                sum.Z += PointsCollection[i].Z * N;
            }
            return new Point(sum.X, sum.Y, sum.Z);
        }

        public override void DrawCurveStereoscopy(Matrix4d transformacja)
        {
            throw new NotImplementedException();
        }

        public override void DrawPolyline(Matrix4d transformacja)
        {
            if (IsBernsteinBasis)
            {
                CalculateAdditionalPoints();
                GL.Begin(BeginMode.Lines);
                GL.Color3(1.0, 1.0, 1.0);

                if (PolylineEnabled)
                {
                    for (int i = 0; i < _additionalPointsCollection2.Count - 1; i++)
                    {
                        var windowCoordinates =
                            projekcja.Multiply(transformacja.Multiply(_additionalPointsCollection2[i].Coordinates));
                        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                        windowCoordinates =
                            projekcja.Multiply(transformacja.Multiply(_additionalPointsCollection2[i + 1].Coordinates));
                        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                    }
                }
                GL.End();
            }
            else
            {


                GL.Begin(BeginMode.Lines);
                GL.Color3(1.0, 1.0, 1.0);
                if (PolylineEnabled)
                {
                    for (int i = 0; i < PointsCollection.Count - 1; i++)
                    {
                        var windowCoordinates =
                            projekcja.Multiply(transformacja.Multiply(PointsCollection[i].Coordinates));
                        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                        windowCoordinates =
                            projekcja.Multiply(transformacja.Multiply(PointsCollection[i + 1].Coordinates));
                        GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                    }
                }
                GL.End();
            }
        }

        public override void DrawPolylineStereoscopy(Matrix4d transformacja)
        {
            throw new NotImplementedException();
        }
        #endregion Public Methods
    }
}

