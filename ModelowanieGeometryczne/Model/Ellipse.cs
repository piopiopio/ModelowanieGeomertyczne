using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ModelowanieGeometryczne.Helpers;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ModelowanieGeometryczne.Model
{
    public class Ellipse
    {
        private double a;
        private double b;
        private double c;
        private double m;
        private Matrix4d transformacja;
        Matrix4d D;
        private int[,] quarter = new int[,] { { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 } };
        public Ellipse()
        {
            
        }

        public List<Vector4d[]> ListaVektorowN = new List<Vector4d[]>();


        public double GetDepth(Matrix4d M, Vector4d u)
        {
            var Minv = M;
            Minv.Invert();
            var MinvTra = Minv;
            MinvTra.Transpose();
            var Dprim = MinvTra * D * Minv;
            //var Dprim =  D ;

            //Matrix4d projection = MatrixProvider.ProjectionMatrix();
            // M = projection * M;

            Vector4d w = new Vector4d();
            //w = MatrixProvider.Multiply(M, u);
            w = u;
            var delta = (Math.Pow(Dprim.M34, 2) + Math.Pow(Dprim.M43, 2) + Math.Pow(Dprim.M13, 2) * Math.Pow(w.X, 2) +
                         Math.Pow(Dprim.M31, 2) * Math.Pow(w.X, 2) +
                         Math.Pow(Dprim.M23, 2) * Math.Pow(w.Y, 2) + Math.Pow(Dprim.M32, 2) * Math.Pow(w.Y, 2) -
                         4 * Dprim.M33 * Dprim.M44 + 2 * Dprim.M34 * Dprim.M43 +
                         2 * Dprim.M13 * Dprim.M34 * w.X - 4 * Dprim.M14 * Dprim.M33 * w.X +
                         2 * Dprim.M13 * Dprim.M43 * w.X + 2 * Dprim.M31 * Dprim.M34 * w.X +
                         2 * Dprim.M31 * Dprim.M43 * w.X -
                         4 * Dprim.M33 * Dprim.M41 * w.X + 2 * Dprim.M23 * Dprim.M34 * w.Y -
                         4 * Dprim.M24 * Dprim.M33 * w.Y + 2 * Dprim.M23 * Dprim.M43 * w.Y +
                         2 * Dprim.M32 * Dprim.M34 * w.Y +
                         2 * Dprim.M32 * Dprim.M43 * w.Y - 4 * Dprim.M33 * Dprim.M42 * w.Y -
                         4 * Dprim.M11 * Dprim.M33 * Math.Pow(w.X, 2) + 2 * Dprim.M13 * Dprim.M31 * Math.Pow(w.X, 2) -
                         4 * Dprim.M22 * Dprim.M33 * Math.Pow(w.Y, 2) + 2 * Dprim.M23 * Dprim.M32 * Math.Pow(w.Y, 2) +
                         2 * Dprim.M13 * Dprim.M23 * w.X * w.Y - 4 * Dprim.M12 * Dprim.M33 * w.X * w.Y +
                         2 * Dprim.M13 * Dprim.M32 * w.X * w.Y - 4 * Dprim.M21 * Dprim.M33 * w.X * w.Y +
                         2 * Dprim.M23 * Dprim.M31 * w.X * w.Y + 2 * Dprim.M31 * Dprim.M32 * w.X * w.Y);

            var aa = (Dprim.M33);
            var bb = Dprim.M34 + Dprim.M43 + Dprim.M13 * w.X + Dprim.M31 * w.X + Dprim.M23 * w.Y + Dprim.M32 * w.Y;


            double result;

            var x1 = (-bb - Math.Pow(delta, 1.0 / 2.0)) / (2.0 * aa);
            var x2 = (-bb + Math.Pow(delta, 1.0 / 2.0)) / (2.0 * aa);

            //TODO: Co jesli porownuje NAN i liczbe
            w.Z = Math.Max(x1, x2);
            result = w.Z;

           // var n = (new Vector4d(2 * u.X * a, 2 * u.Y * b, 2 * u.Z * c,1));
            var n = (new Vector4d(2 * w.X * a, 2 * w.Y * b, 2 * w.Z * c,1));
            if (!double.IsNaN(result))
            {


                n.Normalize();
                double vectorVisualisationFactor = 2;
                Vector4d[] temp = new Vector4d[2];
                temp[0] = new Vector4d(u.X, u.Y, u.Z, 1);
                // temp[0] = (MatrixProvider.ProjectionMatrix() * M).Multiply(temp[0]);
                temp[1] = new Vector4d(u.X + vectorVisualisationFactor * n.X, u.Y + vectorVisualisationFactor * n.Y, u.Z + vectorVisualisationFactor * n.Z, 1);
                //temp[1] = (MatrixProvider.ProjectionMatrix() * M).Multiply(temp[1]);
                ListaVektorowN.Add(temp);
            }

            Vector4d v = new Vector4d(0, 0, 1, 1);
            result = Math.Pow((v.X * n.X + v.Y * n.Y + v.Z * n.Z),m);



            return result;

            //var x = 0.0;
            //var y = 0.0;
            //var bb = Dprim.M31 * x + Dprim.M32 * y + Dprim.M13 * x + Dprim.M23 * y + Dprim.M43 +
            //    Dprim.M34;
            //double c = x * (Dprim.M11 * x + y * Dprim.M21 + Dprim.M41)
            //           + y * (x * Dprim.M12 + y * Dprim.M22 + Dprim.M42)
            //           + x * Dprim.M14 + y * Dprim.M24 + Dprim.M44;
            //double fourAc = 4 * Dprim.M33 * c;
            //double delta = bb * bb - fourAc;
            //return delta;



        }

        public int trimValue(double value)
        {
            int max = 255;
            int min = 0;

            return Math.Min(Math.Max(min, (int)value), max);
        }


        public void Draw(Matrix4d M, double aIn, double bIn, double cIn, double mIn, int counter)
        {
            //intcounter-> rysowanie zaczyna się od pierwszej iteracji; dla 720 jest cpokryty każdy pixel;
            a = aIn;
            b = bIn;
            c = cIn;
            m = mIn;
            D = new Matrix4d(a, 0, 0, 0, 0, b, 0, 0, 0, 0, c, 0, 0, 0, 0, -1);

            
            transformacja = new Matrix4d(M.Row0, M.Row1, M.Row2, M.Row3);
            counter = Math.Min(counter, 720);
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.LoadIdentity();
            GL.Begin(BeginMode.Quads);
            double[] coord = new double[2];
            double delta = 0;
            Color colorOfRectangle = Color.Aqua;
            Random rnd = new Random();
            //counter = 1;//720
            for (int i = 0; i < counter + 1; i++)
            {
                for (int j = 0; j < counter + 1; j++)
                {
                    delta = 1.0 / (2.0 * (counter + 1.0));
                    // coord[0] = (1.0 / (2.0 * (i + 1))) + i / (1 * (i + 1));
                    coord[0] = delta + 2 * delta * i;
                    //coord[1] = (1.0 / (2.0 * (j + 1))) + j / (1 * (j + 1));
                    coord[1] = delta + 2 * delta * j;

                    //Powtórzenie rysowania dla każdej z ćwiartek
                    for (int k = 0; k < quarter.GetLength(0); k++)
                    {

                        colorOfRectangle = Color.FromArgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                        double tempRed = 1000 * Math.Abs(GetDepth(M, new Vector4d(quarter[k, 0] * coord[0], quarter[k, 1] * coord[1], 0, 1)));
                        if (double.IsNaN(tempRed))
                        {
                            tempRed = 0;
                        }

                        GL.Color3(Color.FromArgb(trimValue(tempRed), trimValue(tempRed), 0));
                        GL.Vertex2(quarter[k, 0] * (coord[0] - delta), quarter[k, 1] * (coord[1] - delta));
                        GL.Vertex2(quarter[k, 0] * (coord[0] + delta), quarter[k, 1] * (coord[1] - delta));
                        GL.Vertex2(quarter[k, 0] * (coord[0] + delta), quarter[k, 1] * (coord[1] + delta));
                        GL.Vertex2(quarter[k, 0] * (coord[0] - delta), quarter[k, 1] * (coord[1] + delta));

                        //GL.Vertex2(-1, -1);
                        //GL.Vertex2(0, -1);
                        //GL.Vertex2(0, 0);
                        //GL.Vertex2(-1, 0);




                    }


                }



            }





            GL.End();
            ////GL.Flush();
            //GL.Begin(BeginMode.Lines);
            //GL.Color3(Color.Azure);
            ////Point PunktTestowy1 = new Point(0, 0, 0);
            ////Point PunktTestowy2 = new Point(0, 0, 1);
            ////GL.Vertex3(new Vector3d((M.MultiplyP(PunktTestowy1)).X, M.MultiplyP(PunktTestowy1).Y, M.MultiplyP(PunktTestowy1).Z));
            ////GL.Vertex3(new Vector3d((M.MultiplyP(PunktTestowy2)).X, M.MultiplyP(PunktTestowy2).Y, M.MultiplyP(PunktTestowy2).Z));

            ////GL.Vertex4(M.Multiply(ListaVektorowN[0][0]));
            ////GL.Vertex4(M.Multiply(ListaVektorowN[0][1]));

            //foreach (var item in ListaVektorowN)
            //{
            //   // GL.Vertex4(item[0]);
            //   // GL.Vertex4(item[1]);
            //    GL.Vertex2(item[0].X, item[0].Y);
            //    GL.Vertex2(item[1].X, item[1].Y);
            //}

            ////GL.Vertex4(0.8,0,0,1);
            ////GL.Vertex4(0.8,0.01,1,1);

            //ListaVektorowN.Clear();

            //GL.End();
        }

        public bool EllipseEquation2d(double X, double Y, double Z = 0)
        {
            double epsilon = 0.1;
            if ((((X * X) / (a * a) + (Y * Y) / (b * b) + (Z * Z) / (c * c)) - 1) <= epsilon || ((((X * X) / (a * a) + (Y * Y) / (b * b) + (Z * Z) / (c * c)) - 1) >= -epsilon))
                return true;
            else
                return false;
        }
        public int[] CalculateColor()
        {
            return null;
        }
    }

}
