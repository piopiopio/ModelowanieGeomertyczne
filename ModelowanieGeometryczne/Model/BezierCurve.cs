using System.Collections.ObjectModel;
using ModelowanieGeometryczne.Helpers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;

namespace ModelowanieGeometryczne.Model
{
    public class BezierCurve
    {
        private ObservableCollection<Point> _pointsCollection=new ObservableCollection<Point>();
        Matrix4d projekcja = MatrixProvider.ProjectionMatrix();
        #region Private Methods
        #endregion Private Methods
        #region Public Properties
        #endregion Public Properties
        #region Private Methods
        #endregion Private Methods
        #region Public Methods


        public BezierCurve(ObservableCollection<Point> SelectedPointsCollection)
        {
            _pointsCollection = SelectedPointsCollection;
        }

        public void DrawPolyline(Matrix4d transformacja)
        {
            GL.Begin(BeginMode.Lines);

            for (int i = 0; i < _pointsCollection.Count-1; i++)
            {
                var windowCoordinates = projekcja.Multiply(transformacja.Multiply(_pointsCollection[i].Coordinates));
                GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
                windowCoordinates = projekcja.Multiply(transformacja.Multiply(_pointsCollection[i+1].Coordinates));
                GL.Vertex2(windowCoordinates.X, windowCoordinates.Y);
            }
            GL.End();
        }
        #endregion Public Methods
    }
}

