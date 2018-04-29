using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using ModelowanieGeometryczne.Helpers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Specialized;

namespace ModelowanieGeometryczne.Model
{
    public class BezierCurveC2 : Curve
    {
        bool _chord = true;
        private ObservableCollection<Point> _pointsCollectionInterpolation = new ObservableCollection<Point>();
        private ObservableCollection<Point> _additionalPointsCollection = new ObservableCollection<Point>();
        private ObservableCollection<Point> _additionalPointsCollection2 = new ObservableCollection<Point>();
        ObservableCollection<Point> transformedProjectedPoints = new ObservableCollection<Point>();


        public ObservableCollection<Point> InterpolationPoints { get; set; }
        private double[] _knots;
        private double[] _knotsChord;
        private const int PolynomialDegree = 3;
        private bool _isBernsteinBasis = false;
        private bool _interpolation = false;
        private const int N = 3;
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


        private void CalculateInterpolationDeBoore()
        {
            InterpolationPoints = PointsCollection;

            double[][] nVectors = CalculateSegments2(InterpolationPoints.Count);


            double[][] s = new double[3][];
            for (int i = 0; i < 3; i++)
                s[i] = new double[InterpolationPoints.Count() + 2];
            for (int i = -1; i <= InterpolationPoints.Count(); i++)
            {
                if (InterpolationPoints.Count() <= 0) break;


                s[0][i + 1] = ((InterpolationPoints.ElementAt(Math.Min(Math.Max(i, 0), InterpolationPoints.Count() - 1)))).X;
                s[1][i + 1] = ((InterpolationPoints.ElementAt(Math.Min(Math.Max(i, 0), InterpolationPoints.Count() - 1)))).Y;
                s[2][i + 1] = ((InterpolationPoints.ElementAt(Math.Min(Math.Max(i, 0), InterpolationPoints.Count() - 1)))).Z;

            }

            double[][] result = { MatrixProvider.ThomasAlgorithm(nVectors[1], nVectors[2], nVectors[0], s[0]), MatrixProvider.ThomasAlgorithm(nVectors[1], nVectors[2], nVectors[0], s[1]), MatrixProvider.ThomasAlgorithm(nVectors[1], nVectors[2], nVectors[0], s[2]) };
            _pointsCollectionInterpolation.Clear();


            for (int i = 0; i < InterpolationPoints.Count() + 2; i++)
                _pointsCollectionInterpolation.Add(new Point(result[0][i], result[1][i], result[2][i]));

        }


        //private ObservableCollection<Point> CalculateInterpolationDeBoore(Matrix4d transformacja)
        //{
        //    InterpolationPoints = PointsCollection;
        //    Matrix4d projekcja = MatrixProvider.ProjectionMatrix();


        //}

        private double[,] CalculateSegments(int knotsCount)
        {
            //TODO: przerobić setsplineknots
            SetSplineKnots(knotsCount);
            return _knots.CalculateNMatrix(N, knotsCount);
        }

        private double[][] CalculateSegments2(int knotsCount)
        {
            //TODO: przerobić setsplineknots
            SetSplineKnots(knotsCount);
            return _knots.CalculateNVectors(N, knotsCount);
        }

        public BezierCurveC2(IEnumerable<Point> points, bool interpolation)
        {
            if (interpolation)
            {
                CurveType = "C2Interpolation";

            }
            else
            {

                CurveType = "C2";
            }

            Name = "Bezier curve number " + CurveNumber + " type: " + CurveType;
            _interpolation = interpolation;


            PointsCollection = new ObservableCollection<Point>(points);
            CalculateInterpolationDeBoore();
        }

        protected override void recalculatePoints()
        {
           // CalculateInterpolationDeBoore();

        }

        private void CalculateAdditionalPoints(ObservableCollection<Point> PointsCollection)
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
            _additionalPointsCollection2.Add(_additionalPointsCollection[k + 1]);
            for (k = 3; k < _additionalPointsCollection.Count - 3; k += 2)
            {
                _additionalPointsCollection2.Add(_additionalPointsCollection[k]);
                _additionalPointsCollection2.Add(new Point(_additionalPointsCollection[k].X + (_additionalPointsCollection[k + 1].X - _additionalPointsCollection[k].X) / 2, _additionalPointsCollection[k].Y + (_additionalPointsCollection[k + 1].Y - _additionalPointsCollection[k].Y) / 2, _additionalPointsCollection[k].Z + (_additionalPointsCollection[k + 1].Z - _additionalPointsCollection[k].Z) / 2));
                _additionalPointsCollection2.Add(_additionalPointsCollection[k + 1]);
            }

            _additionalPointsCollection2.Add(_additionalPointsCollection[k]);
            _additionalPointsCollection2.Add(new Point(_additionalPointsCollection[k].X + (_additionalPointsCollection[k + 1].X - _additionalPointsCollection[k].X) / 2, _additionalPointsCollection[k].Y + (_additionalPointsCollection[k + 1].Y - _additionalPointsCollection[k].Y) / 2, _additionalPointsCollection[k].Z + (_additionalPointsCollection[k + 1].Z - _additionalPointsCollection[k].Z) / 2));


        }
        private void SetSplineKnots()
        {
            _knots = new double[transformedProjectedPoints.Count + PolynomialDegree + 4];

            double interval = 1 / (double)(transformedProjectedPoints.Count + PolynomialDegree + 3);

            for (int i = 0; i < transformedProjectedPoints.Count + PolynomialDegree + 4; i++)

                _knots[i] = i * interval;

        }


        private void SetSplineKnots(int count)
        {
            _knots = new double[count + PolynomialDegree + 4];
            _knotsChord = new double[count];
            double interval = 1 / (double)(count + PolynomialDegree + 3);

            for (int i = 0; i < count + PolynomialDegree + 4; i++)
            {
                _knots[i] = i * interval;

            }

            double L = 0;
            foreach (var item in PointsCollection)
            {
                L += item.Length();
            }

            _knotsChord[0] = 0;

            for (int i = 0; i < count-1; i++)
            {
                _knotsChord[i+1] = _knotsChord[i]+(PointsCollection[i + 1].Subtract(PointsCollection[i])).Length()/L;

            }

            _knotsChord[count-1] = 1;
            //if (_chord)
            //{

            //}
            //else
            //{

            //}
        }



        public override void DrawCurve(Matrix4d transformacja)
        {
            if (_interpolation)
            {
                if (PointsCollection.Count > 1)
                {

                    // CalculateInterpolationDeBoore();
                    transformedProjectedPoints.Clear();
                    foreach (var p in _pointsCollectionInterpolation)
                    {
                        var a = projekcjaRight.Multiply(transformacja.Multiply(p));
                        transformedProjectedPoints.Add(new Point(a.X, a.Y, a.Z));
                    }

                    SetSplineKnots();

                    CalculateAdditionalPoints(_pointsCollectionInterpolation);
                    double divisions = GetDivisions(transformacja, _additionalPointsCollection2);

                    if (IsBernsteinBasis)
                    {


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

                                var u = BSplinePoint(t);
                                var v = BSplinePoint(t + divisions);

                                // var windowCoordinates = projekcja.Multiply(transformacja.Multiply(u.Coordinates));
                                GL.Vertex2(u.X, u.Y);

                                ///   windowCoordinates = projekcja.Multiply(transformacja.Multiply(v.Coordinates));
                                GL.Vertex2(v.X, v.Y);
                            }
                        }
                        GL.End();

                    }
                }
            }
            else
            {
                if (PointsCollection.Count > 3)
                {
                    transformedProjectedPoints.Clear();
                    foreach (var p in PointsCollection)
                    {
                        var a = projekcjaRight.Multiply(transformacja.Multiply(p));
                        transformedProjectedPoints.Add(new Point(a.X, a.Y, a.Z));
                    }

                    SetSplineKnots();

                    CalculateAdditionalPoints(PointsCollection);
                    double divisions = GetDivisions(transformacja, _additionalPointsCollection2);

                    if (IsBernsteinBasis)
                    {


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

                                var u = BSplinePoint(t);
                                var v = BSplinePoint(t + divisions);

                                GL.Vertex2(u.X, u.Y);

                                GL.Vertex2(v.X, v.Y);
                            }
                        }
                        GL.End();
                    }
                }
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

            for (int i = 0; i < transformedProjectedPoints.Count; i++)
            {
                var N = GetNFunctionValue(_knots, i, PolynomialDegree, t);
                sum.X += transformedProjectedPoints[i].X * N;
                sum.Y += transformedProjectedPoints[i].Y * N;
                sum.Z += transformedProjectedPoints[i].Z * N;
            }
            return new Point(sum.X, sum.Y, sum.Z);
        }

        public override void DrawCurveStereoscopy(Matrix4d transformacja)
        {
            if (_interpolation)
            {
                CalculateInterpolationDeBoore();
                transformedProjectedPoints.Clear();
                foreach (var p in _pointsCollectionInterpolation)
                {
                    var a = projekcjaRight.Multiply(transformacja.Multiply(p));
                    transformedProjectedPoints.Add(new Point(a.X, a.Y, a.Z));
                }

                SetSplineKnots();

                CalculateAdditionalPoints(_pointsCollectionInterpolation);
                double divisions = GetDivisions(transformacja, _additionalPointsCollection2);

                if (IsBernsteinBasis)
                {


                    foreach (var p in _additionalPointsCollection)
                    {
                        p.Draw(transformacja, 4);
                    }
                    foreach (var p in _additionalPointsCollection2)
                    {
                        p.Draw(transformacja, 4);
                    }
                    DrawBezierCurveStereoscopy(transformacja, _additionalPointsCollection2);
                }
                else
                {

                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
                    GL.Begin(BeginMode.Lines);
                    GL.Color3(0.6, 0.0, 0.0);
                    for (double t = 0; t < 1; t += divisions)
                    {
                        if (t >= _knots[3] && t <= _knots[_knots.Length - PolynomialDegree - 4])
                        {

                            var u = BSplinePoint(t);
                            var v = BSplinePoint(t + divisions);

                            // var windowCoordinates = projekcja.Multiply(transformacja.Multiply(u.Coordinates));
                            GL.Vertex2(u.X, u.Y);

                            ///   windowCoordinates = projekcja.Multiply(transformacja.Multiply(v.Coordinates));
                            GL.Vertex2(v.X, v.Y);
                        }
                    }

                    GL.Color3(0.0, 0.0, 0.6);
                    for (double t = 0; t < 1; t += divisions)
                    {
                        if (t >= _knots[3] && t <= _knots[_knots.Length - PolynomialDegree - 4])
                        {

                            var u = BSplinePoint(t);
                            var v = BSplinePoint(t + divisions);

                            // var windowCoordinates = projekcja.Multiply(transformacja.Multiply(u.Coordinates));
                            GL.Vertex2(u.X, u.Y);

                            ///   windowCoordinates = projekcja.Multiply(transformacja.Multiply(v.Coordinates));
                            GL.Vertex2(v.X, v.Y);
                        }
                    }
                    GL.End();

                }
            }


            else
            {
                transformedProjectedPoints.Clear();
                foreach (var p in PointsCollection)
                {
                    var a = projekcjaRight.Multiply(transformacja.Multiply(p));
                    transformedProjectedPoints.Add(new Point(a.X, a.Y, a.Z));
                }

                SetSplineKnots();

                CalculateAdditionalPoints(PointsCollection);
                double divisions = GetDivisions(transformacja, _additionalPointsCollection2);

                if (IsBernsteinBasis)
                {


                    foreach (var p in _additionalPointsCollection)
                    {
                        p.Draw(transformacja, 4);
                    }
                    foreach (var p in _additionalPointsCollection2)
                    {
                        p.Draw(transformacja, 4);
                    }
                    DrawBezierCurveStereoscopy(transformacja, _additionalPointsCollection2);
                }
                else
                {

                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
                    GL.Begin(BeginMode.Lines);
                    GL.Color3(0.6, 0.0, 0.0);
                    for (double t = 0; t < 1; t += divisions)
                    {
                        if (t >= _knots[3] && t <= _knots[_knots.Length - PolynomialDegree - 4])
                        {

                            var u = BSplinePoint(t);
                            var v = BSplinePoint(t + divisions);

                            // var windowCoordinates = projekcja.Multiply(transformacja.Multiply(u.Coordinates));
                            GL.Vertex2(u.X, u.Y);

                            ///   windowCoordinates = projekcja.Multiply(transformacja.Multiply(v.Coordinates));
                            GL.Vertex2(v.X, v.Y);
                        }
                    }

                    GL.Color3(0.0, 0.0, 0.6);
                    for (double t = 0; t < 1; t += divisions)
                    {
                        if (t >= _knots[3] && t <= _knots[_knots.Length - PolynomialDegree - 4])
                        {

                            var u = BSplinePoint(t);
                            var v = BSplinePoint(t + divisions);

                            // var windowCoordinates = projekcja.Multiply(transformacja.Multiply(u.Coordinates));
                            GL.Vertex2(u.X, u.Y);

                            ///   windowCoordinates = projekcja.Multiply(transformacja.Multiply(v.Coordinates));
                            GL.Vertex2(v.X, v.Y);
                        }
                    }
                    GL.End();

                }
            }
        }


        public override void DrawPolyline(Matrix4d transformacja)
        {
            if (_interpolation)
            {
                //  var debor = CalculateInterpolationDeBoore(transformacja);

                CalculateInterpolationDeBoore();
                var debor = _pointsCollectionInterpolation;
                if (IsBernsteinBasis)
                {
                    if (PolylineEnabled)
                    {//TODO: tu wroc
                        CalculateAdditionalPoints(_pointsCollectionInterpolation);
                        GL.Begin(BeginMode.Lines);
                        GL.Color3(1.0, 1.0, 1.0);


                        for (int i = 0; i < _additionalPointsCollection2.Count - 1; i++)
                        {
                            var windowCoordinates =
                                projekcja.Multiply(transformacja.Multiply(_additionalPointsCollection2[i].Coordinates));
                            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                            windowCoordinates =
                                projekcja.Multiply(transformacja.Multiply(_additionalPointsCollection2[i + 1].Coordinates));
                            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                        }

                        GL.End();
                    }
                }
                else
                {
                    if (PolylineEnabled)
                    {

                        GL.Begin(BeginMode.Lines);
                        GL.Color3(1.0, 1.0, 1.0);

                        for (int i = 0; i < _pointsCollectionInterpolation.Count - 1; i++)
                        {
                            var windowCoordinates =
                                projekcja.Multiply(transformacja.Multiply(_pointsCollectionInterpolation[i].Coordinates));
                            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                            windowCoordinates =
                                projekcja.Multiply(transformacja.Multiply(_pointsCollectionInterpolation[i + 1].Coordinates));
                            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                        }

                        GL.End();
                    }
                }
            }
            else
            {
                if (IsBernsteinBasis)
                {
                    if (PolylineEnabled)
                    {
                        CalculateAdditionalPoints(PointsCollection);
                        GL.Begin(BeginMode.Lines);
                        GL.Color3(1.0, 1.0, 1.0);


                        for (int i = 0; i < _additionalPointsCollection2.Count - 1; i++)
                        {
                            var windowCoordinates =
                                projekcja.Multiply(transformacja.Multiply(_additionalPointsCollection2[i].Coordinates));
                            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                            windowCoordinates =
                                projekcja.Multiply(transformacja.Multiply(_additionalPointsCollection2[i + 1].Coordinates));
                            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                        }

                        GL.End();
                    }
                }
                else
                {
                    if (PolylineEnabled)
                    {

                        GL.Begin(BeginMode.Lines);
                        GL.Color3(1.0, 1.0, 1.0);

                        for (int i = 0; i < PointsCollection.Count - 1; i++)
                        {
                            var windowCoordinates =
                                projekcja.Multiply(transformacja.Multiply(PointsCollection[i].Coordinates));
                            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                            windowCoordinates =
                                projekcja.Multiply(transformacja.Multiply(PointsCollection[i + 1].Coordinates));
                            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                        }

                        GL.End();
                    }
                }
            }
        }


        public override void DrawPolylineStereoscopy(Matrix4d transformacja)
        {
            if (_interpolation)
            {
                if (IsBernsteinBasis)
                {
                    if (PolylineEnabled)
                    {
                        CalculateAdditionalPoints(_pointsCollectionInterpolation);

                        GL.Enable(EnableCap.Blend);
                        GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
                        GL.Begin(BeginMode.Lines);
                        GL.Color3(0.6, 0.0, 0.0);


                        for (int i = 0; i < _additionalPointsCollection2.Count - 1; i++)
                        {
                            var windowCoordinates =
                                projekcjaRight.Multiply(transformacja.Multiply(_additionalPointsCollection2[i].Coordinates));
                            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                            windowCoordinates =
                                projekcjaRight.Multiply(transformacja.Multiply(_additionalPointsCollection2[i + 1].Coordinates));
                            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                        }

                        GL.Color3(0.0, 0.0, 0.6);

                        for (int i = 0; i < _additionalPointsCollection2.Count - 1; i++)
                        {
                            var windowCoordinates =
                                projekcjaLeft.Multiply(transformacja.Multiply(_additionalPointsCollection2[i].Coordinates));
                            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                            windowCoordinates =
                                projekcjaLeft.Multiply(transformacja.Multiply(_additionalPointsCollection2[i + 1].Coordinates));
                            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                        }
                        GL.End();
                    }
                }
                else
                {
                    if (PolylineEnabled)
                    {
                        GL.Enable(EnableCap.Blend);
                        GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
                        GL.Begin(BeginMode.Lines);
                        GL.Color3(0.6, 0.0, 0.0);

                        for (int i = 0; i < _pointsCollectionInterpolation.Count - 1; i++)
                        {
                            var windowCoordinates =
                                projekcjaRight.Multiply(transformacja.Multiply(_pointsCollectionInterpolation[i].Coordinates));
                            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                            windowCoordinates =
                                projekcjaRight.Multiply(transformacja.Multiply(_pointsCollectionInterpolation[i + 1].Coordinates));
                            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                        }

                        GL.Color3(0.0, 0.0, 0.6);
                        for (int i = 0; i < _pointsCollectionInterpolation.Count - 1; i++)
                        {
                            var windowCoordinates =
                                projekcjaLeft.Multiply(transformacja.Multiply(_pointsCollectionInterpolation[i].Coordinates));
                            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                            windowCoordinates =
                                projekcjaLeft.Multiply(transformacja.Multiply(_pointsCollectionInterpolation[i + 1].Coordinates));
                            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                        }

                        GL.End();
                    }
                }
            }
            else
            {
                if (IsBernsteinBasis)
                {
                    if (PolylineEnabled)
                    {
                        CalculateAdditionalPoints(PointsCollection);

                        GL.Enable(EnableCap.Blend);
                        GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
                        GL.Begin(BeginMode.Lines);
                        GL.Color3(0.6, 0.0, 0.0);


                        for (int i = 0; i < _additionalPointsCollection2.Count - 1; i++)
                        {
                            var windowCoordinates =
                                projekcjaRight.Multiply(transformacja.Multiply(_additionalPointsCollection2[i].Coordinates));
                            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                            windowCoordinates =
                                projekcjaRight.Multiply(transformacja.Multiply(_additionalPointsCollection2[i + 1].Coordinates));
                            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                        }

                        GL.Color3(0.0, 0.0, 0.6);

                        for (int i = 0; i < _additionalPointsCollection2.Count - 1; i++)
                        {
                            var windowCoordinates =
                                projekcjaLeft.Multiply(transformacja.Multiply(_additionalPointsCollection2[i].Coordinates));
                            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                            windowCoordinates =
                                projekcjaLeft.Multiply(transformacja.Multiply(_additionalPointsCollection2[i + 1].Coordinates));
                            GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                        }
                        GL.End();
                    }
                }
                else
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
            }
        }
        #endregion Public Methods
    }
}

