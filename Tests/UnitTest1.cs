using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelowanieGeometryczne.Helpers;
using ModelowanieGeometryczne.Model;
using System;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            double[] a = new double[3] { 1, 1, 1 };
            double[] b = new double[3] { 0, 0, 2 };
            double[] c = new double[3] { 1, 0, 0 };
            double[] f = new double[3] { 3, 2, 4 };

            var x = MatrixProvider.ThomasAlgorithm(a, b, c, f);
            Console.WriteLine(x);
        }

        [TestMethod]
        public void TestMethod2()
        {
            double[] a = new double[2] { 1, 1 };
            double[] b = new double[2] { 0, 2 };
            double[] c = new double[2] { 3, 0 };
            double[] f = new double[2] { 7, 4 };

            var x = MatrixProvider.ThomasAlgorithm(a, b, c, f);
            Console.WriteLine(x);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Patch a = new Patch();
            a.u = 2;
            a.v = 3;
            a.CalculateParametrizationVectors();
            
        }

        [TestMethod]
        public void TestMethod4()
        {
            var G = new double[,] { { 1, 2, 3, 4 },{ 5, 6, 7, 8 }, { 9, 10, 11, 12 }, { 13, 14, 15, 16 } };
            var Bu = new double[] { 1, 2, 3, 4 };
            var result = MatrixProvider.Multiply(Bu, G, Bu);
        }
    }
}
