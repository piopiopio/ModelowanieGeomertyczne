using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;

using ModelowanieGeometryczne.ViewModel;
using OpenTK;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ModelowanieGeometryczne.Model
{
    public class Torus : ViewModelBase
    {
        private double _r;
        private double _R;
        //private double _l;
        //private double _v;
        private List<List<Vector3d>> _verticesList = new List<List<Vector3d>>();
        private List<Vector3d> _relationsList = new List<Vector3d>();
        private int _division_fi = 100;
        private int _division_teta = 100;
        #region Private Methods

        public Torus()
        {
            _r = 1;
            _R = 2;
            _division_fi = 100;
            _division_teta = 100;
         
           
        }
        #endregion Private Methods
        #region Public Properties
        
        public double r
        {
            get { return _r; }

            set
            {
                _r = value;
                OnPropertyChanged("r");
                generateParametrization();

            }


        }

        void generateParametrization()
        {
            _verticesList.Clear();
            double fi, teta;
            double deltaFi = 2*Math.PI / _division_fi;
            double deltaTeta = 2*Math.PI / _division_teta;
            Vector3d temp = new Vector3d();

            for (int j = 0; j < _division_teta; j++)
            {
                var loop = new List<Vector3d>();
                for (int i = 0; i < _division_fi; i++)
                {
                    fi = deltaFi * i;
                    teta = deltaTeta * j;
                    temp.X = (_R + _r * Math.Cos(fi)) * Math.Cos(teta);
                    temp.Y = (_R + _r * Math.Cos(fi)) * Math.Sin(teta);
                    temp.Z = _R * Math.Sin(fi);
                    loop.Add(temp);
                }
                _verticesList.Add(loop);
            }

        }

        public void Draw()
        {
            foreach (var v in _verticesList)
            {
                GL.Begin(BeginMode.LineLoop);
                GL.Color3(1.0, 0.0, 0.0);
                foreach (var vertex in v)
                {
                    GL.Vertex3(vertex);
                }
                GL.End();
            }

        }
        public double R
        {
            get { return _R; }
            set
            {
                _R = value;
                OnPropertyChanged("R");
                generateParametrization();
            }
        }

        public int Division_fi
        {
            get { return _division_fi; }
            set
            {
                _division_fi = value;
                OnPropertyChanged("Division_fi");
                generateParametrization();
            }
        }

        public int Division_teta
        {
            get { return _division_teta; }
            set
            {
                _division_teta = value;
                OnPropertyChanged("Division_teta");
                generateParametrization();
            }
        }
        #endregion Public Properties
        #region Private Methods
        #endregion Private Methods
        #region Public Methods
        #endregion Public Methods
    }
}

