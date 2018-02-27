using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        public event PropertyChangedEventHandler RefreshTorus;
        #region Private fields
        private double _r;
        private double _R;
        private List<Vector3d> _verticesList = new List<Vector3d>();
        private List<Tuple<int, int>> _relationsList = new List<Tuple<int, int>>();
        private int _division_fi = 100;
        private int _division_teta = 100;

        #endregion Private fields

        #region Public Properties
        public double r
        {
            get { return _r; }

            set
            {
                _r = value;
                OnPropertyChanged("r");
                generateParametrization();
                Refresh();
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
                Refresh();
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
                Refresh();
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
                Refresh();
            }
        }


        #endregion Public Properties

        public Torus()
        {
            _r = 1;
            _R = 2;
            _division_fi = 100;
            _division_teta = 100;
            generateParametrization();
           
        }
       
        #region Private Methods

        private void generateParametrization()
        {
            _verticesList.Clear();
            double fi, teta;
            double deltaFi = 2*Math.PI / _division_fi;
            double deltaTeta = 2*Math.PI / _division_teta;
            Vector3d temp = new Vector3d();
            int k=0;

            _relationsList.Clear();
            _verticesList.Clear();
            for (int j = 0; j < _division_teta; j++)
            {
               
                for (int i = 0; i < _division_fi; i++)
                {
                    k = i + j * _division_fi;
                    fi = deltaFi * i;
                    teta = deltaTeta * j;
                    temp.X = (_R + _r * Math.Cos(fi)) * Math.Cos(teta);
                    temp.Y = (_R + _r * Math.Cos(fi)) * Math.Sin(teta);
                    temp.Z = _R * Math.Sin(fi);
                    _verticesList.Add(temp);

                    if (i != _division_fi && i>0)
                    {
                        _relationsList.Add(new Tuple<int, int>(k-1, k));
                    }

                    else if (i>0)
                    {
                        _relationsList.Add(new Tuple<int, int>(k, k-i));
                    }
                }
                
                

               //TODO : Data representation in two list vertices List<Vector3d> and relations List<Vector2d> _verticesList.ElementAt();
            }

        }

       
        private void Refresh()
        {   
            if (RefreshTorus != null)
                RefreshTorus(this, new PropertyChangedEventArgs("RefreshScene"));
        }
        
 
        #endregion Private Methods
        
        #region Public Methods

        public void Draw()
        {
            foreach (var v in _relationsList)
            {
                GL.Begin(BeginMode.Lines);
                GL.Color3(1.0, 1.0, 1.0);

                //for (int i = 0; i < 1000; i++)
                //{
                //    GL.Vertex3(_verticesList.ElementAt(_relationsList.ElementAt(i).Item1));
                //    GL.Vertex3(_verticesList.ElementAt(_relationsList.ElementAt(i).Item2));
                //}
                foreach (var relations in _relationsList)
                {
                    GL.Vertex3(_verticesList[relations.Item1]);
                    GL.Vertex3(_verticesList[relations.Item2]);
                }
                GL.End();
            }

        }

        #endregion Public Methods
    }
        
}

