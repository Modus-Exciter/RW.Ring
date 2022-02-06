using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Notung.Data;
using System.Collections.Generic;
using System.Globalization;
using System;

namespace NotungTest
{
  [TestClass]
  public class MatrixTest
  {

    [TestMethod]
    public void FillMatrixWithDiagonal()
    {
      TriangleMatrix<int> matrix = new TriangleMatrix<int>(5, true);

      matrix[0, 0] = 1;
      matrix[0, 1] = 2;
      matrix[0, 2] = 3;
      matrix[0, 3] = 4;
      matrix[0, 4] = 5;
      matrix[1, 1] = 6;
      matrix[1, 2] = 7;
      matrix[3, 1] = 8;
      matrix[1, 4] = 9;
      matrix[2, 2] = 10;
      matrix[2, 3] = 11;
      matrix[2, 4] = 12;
      matrix[3, 3] = 13;
      matrix[3, 4] = 14;
      matrix[4, 4] = 15;

      Assert.AreEqual(1, matrix[0, 0]);
      Assert.AreEqual(2, matrix[0, 1]);
      Assert.AreEqual(3, matrix[0, 2]);
      Assert.AreEqual(4, matrix[0, 3]);
      Assert.AreEqual(5, matrix[0, 4]);
      Assert.AreEqual(6, matrix[1, 1]);
      Assert.AreEqual(7, matrix[1, 2]);
      Assert.AreEqual(8, matrix[1, 3]);
      Assert.AreEqual(9, matrix[1, 4]);
      Assert.AreEqual(10, matrix[2, 2]);
      Assert.AreEqual(11, matrix[3, 2]);
      Assert.AreEqual(12, matrix[2, 4]);
      Assert.AreEqual(13, matrix[3, 3]);
      Assert.AreEqual(14, matrix[3, 4]);
      Assert.AreEqual(15, matrix[4, 4]);
    }

    [TestMethod]
    public void FillMatrixWithoutDiagonal()
    {
      TriangleMatrix<int> matrix = new TriangleMatrix<int>(5, false);

      matrix[0, 1] = 2;
      matrix[0, 2] = 3;
      matrix[0, 3] = 4;
      matrix[0, 4] = 5;
      matrix[1, 2] = 7;
      matrix[3, 1] = 8;
      matrix[1, 4] = 9;
      matrix[2, 3] = 11;
      matrix[2, 4] = 12;
      matrix[3, 4] = 14;

      Assert.AreEqual(2, matrix[0, 1]);
      Assert.AreEqual(3, matrix[2, 0]);
      Assert.AreEqual(4, matrix[0, 3]);
      Assert.AreEqual(5, matrix[0, 4]);
      Assert.AreEqual(7, matrix[1, 2]);
      Assert.AreEqual(8, matrix[1, 3]);
      Assert.AreEqual(9, matrix[1, 4]);
      Assert.AreEqual(11, matrix[3, 2]);
      Assert.AreEqual(12, matrix[2, 4]);
      Assert.AreEqual(14, matrix[3, 4]);
    }

    private IEnumerable<T> GetRowData<T>(IMatrix<T> matrix, int index)
    {
      for (int i = 0; i < matrix.ColumnCount; i++)
      {
        if (i == index && matrix is TriangleMatrix<T>
          && !((TriangleMatrix<T>)matrix).WithDiagonal)
          yield return default(T);
        else
          yield return matrix[index, i];
      }
    }

    [TestMethod]
    public void MatrixRowWithDiagonal()
    {
      TriangleMatrix<int> matrix = new TriangleMatrix<int>(5, true);

      matrix[0, 0] = 1;
      matrix[0, 1] = 2;
      matrix[0, 2] = 3;
      matrix[0, 3] = 4;
      matrix[0, 4] = 5;
      matrix[1, 1] = 6;
      matrix[1, 2] = 7;
      matrix[3, 1] = 8;
      matrix[1, 4] = 9;
      matrix[2, 2] = 10;
      matrix[2, 3] = 11;
      matrix[2, 4] = 12;
      matrix[3, 3] = 13;
      matrix[3, 4] = 14;
      matrix[4, 4] = 15;

      var row = this.GetRowData(matrix, 3).ToArray();

      Assert.AreEqual(5, row.Length);

      Assert.AreEqual(4, row[0]);
      Assert.AreEqual(8, row[1]);
      Assert.AreEqual(11, row[2]);
      Assert.AreEqual(13, row[3]);
      Assert.AreEqual(14, row[4]);
    }

    [TestMethod]
    public void MatrixRowWithoutDiagonal()
    {
      TriangleMatrix<int> matrix = new TriangleMatrix<int>(5, false);

      matrix[0, 1] = 2;
      matrix[0, 2] = 3;
      matrix[0, 3] = 4;
      matrix[0, 4] = 5;
      matrix[1, 2] = 7;
      matrix[3, 1] = 8;
      matrix[1, 4] = 9;
      matrix[2, 3] = 11;
      matrix[2, 4] = 12;
      matrix[3, 4] = 14;

      var row = this.GetRowData(matrix, 3).ToArray();

      Assert.AreEqual(5, row.Length);
      Assert.AreEqual(4, row[0]);
      Assert.AreEqual(8, row[1]);
      Assert.AreEqual(11, row[2]);
      Assert.AreEqual(0, row[3]);
      Assert.AreEqual(14, row[4]);
    }


    //тест на detA = detAT 
    [TestMethod]
    public void Determinant()
    {
      RectangleMatrix<double> A = new RectangleMatrix<double>(3, 3);
      A[0, 0] = 2;
      A[0, 1] = 4;
      A[0, 2] = 6;
      A[1, 0] = 8;
      A[1, 1] = 10;
      A[1, 2] = 12;
      A[2, 0] = 14;
      A[2, 1] = 16;
      A[2, 2] = 18;
      RectangleMatrix<double> A2 = new RectangleMatrix<double>(3, 3);
      A2[0, 0] = 2;
      A2[0, 1] = 4;
      A2[0, 2] = 6;
      A2[1, 0] = 8;
      A2[1, 1] = 10;
      A2[1, 2] = 12;
      A2[2, 0] = 14;
      A2[2, 1] = 16;
      A2[2, 2] = 18;
      IMatrix<double> AT = MatrixFunctions.Transpose<double>(A);
      CultureInfo myCIintl = new CultureInfo("es-ES", false);
      double detAT = MatrixFunctions.Determinant<double>(AT, myCIintl);
      double detA2 = MatrixFunctions.Determinant<double>(A2, myCIintl);
      Assert.AreEqual(detAT, detA2);

    }
    // тест на A*A^-1 = E
    [TestMethod]
    public void MultiplyWithInvert()
    {
      RectangleMatrix<double> A = new RectangleMatrix<double>(3, 3);
      A[0, 0] = 3;
      A[0, 1] = 45;
      A[0, 2] = 32;
      A[1, 0] = 56;
      A[1, 1] = 32;
      A[1, 2] = 12;
      A[2, 0] = 46;
      A[2, 1] = 434;
      A[2, 2] = 64;
      RectangleMatrix<double> A2 = new RectangleMatrix<double>(3, 3);
      A2[0, 0] = 3;
      A2[0, 1] = 45;
      A2[0, 2] = 32;
      A2[1, 0] = 56;
      A2[1, 1] = 32;
      A2[1, 2] = 12;
      A2[2, 0] = 46;
      A2[2, 1] = 434;
      A2[2, 2] = 64;
      CultureInfo myCIintl = new CultureInfo("es-ES", false);
      IMatrix<double> invA = MatrixFunctions.Invert<double>(A, myCIintl);
      var B = MatrixFunctions.Multiply<double>(invA, A2, myCIintl);
      RectangleMatrix<double> E = new RectangleMatrix<double>(3, 3);
      E[0, 0] = 1;
      E[0, 1] = 0;
      E[0, 2] = 0;
      E[1, 0] = 0;
      E[1, 1] = 1;
      E[1, 2] = 0;
      E[2, 0] = 0;
      E[2, 1] = 0;
      E[2, 2] = 1;
      Assert.AreEqual(E[0, 0], B[0, 0]);
      Assert.AreEqual(E[0, 1], B[0, 1]);
      Assert.AreEqual(E[0, 2], B[0, 2]);
      Assert.AreEqual(E[1, 0], B[1, 0]);
      Assert.AreEqual(E[1, 1], B[1, 1]);
      Assert.AreEqual(E[1, 2], B[1, 2]);
      Assert.AreEqual(E[2, 0], B[2, 0]);
      Assert.AreEqual(E[2, 1], B[2, 1]);
      Assert.AreEqual(E[2, 2], B[2, 2]);
    }

    // тест на A*B != B*A
    [TestMethod]
    public void MultiplyDifferentMatrix()
    {
      RectangleMatrix<double> A = new RectangleMatrix<double>(3, 3);
      A[0, 0] = 42;
      A[0, 1] = 47;
      A[0, 2] = 88;
      A[1, 0] = 36;
      A[1, 1] = 56;
      A[1, 2] = 96;
      A[2, 0] = 78;
      A[2, 1] = 45;
      A[2, 2] = 81;

      RectangleMatrix<double> B = new RectangleMatrix<double>(3, 3);
      B[0, 0] = 32;
      B[0, 1] = 42;
      B[0, 2] = 47;
      B[1, 0] = 37;
      B[1, 1] = 38;
      B[1, 2] = 32;
      B[2, 0] = 89;
      B[2, 1] = 74;
      B[2, 2] = 18;

      CultureInfo myCIintl = new CultureInfo("es-ES", false);
      var C = MatrixFunctions.Multiply<double>(A, B, myCIintl);
      var D = MatrixFunctions.Multiply<double>(B, A, myCIintl);
      Assert.AreNotEqual(C[0, 0], D[0, 0]);
      Assert.AreNotEqual(C[0, 1], D[0, 1]);
      Assert.AreNotEqual(C[0, 2], D[0, 2]);
      Assert.AreNotEqual(C[1, 0], D[1, 0]);
      Assert.AreNotEqual(C[1, 1], D[1, 1]);
      Assert.AreNotEqual(C[1, 2], D[1, 2]);
      Assert.AreNotEqual(C[2, 0], D[2, 0]);
      Assert.AreNotEqual(C[2, 1], D[2, 1]);
      Assert.AreNotEqual(C[2, 2], D[2, 2]);
    }

    // тест на A(m x n) * B (m x k)
    [ExpectedException(typeof(ArgumentException))]
    [TestMethod]
    public void MultiplyMatrixWithWrongDimension()
    {
      RectangleMatrix<double> A = new RectangleMatrix<double>(3, 3);
      A[0, 0] = 42;
      A[0, 1] = 47;
      A[0, 2] = 88;
      A[1, 0] = 36;
      A[1, 1] = 56;
      A[1, 2] = 96;
      A[2, 0] = 78;
      A[2, 1] = 45;
      A[2, 2] = 81;

      RectangleMatrix<double> B = new RectangleMatrix<double>(2, 3);
      B[0, 0] = 42;
      B[0, 1] = 47;
      B[0, 2] = 88;
      B[1, 0] = 36;
      B[1, 1] = 56;
      B[1, 2] = 96;

      CultureInfo myCIintl = new CultureInfo("es-ES", false);
      MatrixFunctions.Multiply(A, B, myCIintl);
    }
  }
}