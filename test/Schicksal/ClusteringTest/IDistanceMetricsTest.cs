using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schicksal.Clustering;
using System;

namespace ClusteringTest
{
    [TestClass]

    public class IDistanceMetricsTest
    {
        [TestMethod]
        public void EuclidDistanceMetricsTest()
        {
            EuclidDistanceMetrics euclidian = new EuclidDistanceMetrics();
            euclidian.BeginCalculation();
            Assert.AreEqual(0,euclidian.GetResult());
            double a = 10;
            double b = 15;
            double c = 55;
            double d = 34.155;
            euclidian.AddDifference(a,b);
            euclidian.AddDifference(c, d);
            double result = euclidian.GetResult();
            bool isResultAccurate = false;
            if (Math.Abs(result-21.436278)<0.000001) { isResultAccurate = true;}
            Assert.IsTrue(isResultAccurate);
            euclidian.BeginCalculation();
            Assert.AreEqual(0, euclidian.GetResult());
        }

        [TestMethod]
        public void ManhattanDistanceMetricsTest() 
        {
            ManhattanDistanceMetrics manhattan = new ManhattanDistanceMetrics();
            manhattan.BeginCalculation();
            Assert.AreEqual(0, manhattan.GetResult());
            double a = 10;
            double b = 15;
            double c = 55;
            double d = 34.155;
            manhattan.AddDifference(a,b);
            manhattan.AddDifference(c, d);
            double result = manhattan.GetResult();
            bool isResultAccurate = false;
            if (Math.Abs(result - 25.845) < 0.000001) { isResultAccurate = true; }
            Assert.IsTrue(isResultAccurate);
            manhattan.BeginCalculation();
            Assert.AreEqual(0, manhattan.GetResult());
        }

        [TestMethod]
        public void CartisDistanceMetricsTest()
        {
            CartisDistanceMetrics сartis = new CartisDistanceMetrics();
            сartis.BeginCalculation();
            Assert.AreEqual(0, сartis.GetResult());
            double a = 10;
            double b = 15;
            double c = 55;
            double d = 34.155;
            сartis.AddDifference(a, b);
            сartis.AddDifference(c, d);
            double result = сartis.GetResult();
            bool isResultAccurate = false;
            if (Math.Abs(result - 0.433806) < 0.000001) { isResultAccurate = true; }
            Assert.IsTrue(isResultAccurate);
            сartis.BeginCalculation();
            Assert.AreEqual(0, сartis.GetResult());
        }

        [TestMethod]
        public void ChebyshevDistanceMetricsTest()
        {
            ChebyshevDistanceMetrics сhebyshev = new ChebyshevDistanceMetrics();
            сhebyshev.BeginCalculation();
            Assert.AreEqual(0, сhebyshev.GetResult());
            double a = 10;
            double b = 15;
            double c = 55;
            double d = 34.155;
            сhebyshev.AddDifference(a, b);
            сhebyshev.AddDifference(c, d);
            double result = сhebyshev.GetResult();
            bool isResultAccurate = false;
            if (Math.Abs(result - 20.845) < 0.000001) { isResultAccurate = true; }
            Assert.IsTrue(isResultAccurate);
            сhebyshev.BeginCalculation();
            Assert.AreEqual(0, сhebyshev.GetResult());
        }
    }
}
